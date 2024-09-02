import socket
import signal
import sys
import json
import sqlite3
from datetime import datetime
from _thread import *
from random import randint
import random


def signal_handler(signal, frame):
    print('Exiting ...')
    sock.close()
    sys.exit(0)


def establishConnections(port: int):
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    try:
        sock.bind(("", port))
    except socket.error as e:
        print(str(e))
        exit()

    sock.listen()
    print("Server Started...")
    return sock


def worker(conn, addr):
    global players, isBegin, activeGame
    db = sqlite3.connect("gtutag3d.db")
    db.execute("PRAGMA foreign_keys = 1")  # Enable foreign keys
    sql = db.cursor()

    myid = -1
    myteam = -1
    readyCount = 0
    print("Connected to:", addr)
    while True:
        request = conn.recv(3500).decode()
        if not request:
            break
        else:
            request = json.loads(request)

        if request["type"] == "Hello":
            response = {
                "type": "Hello",
                "response": "Hello from server"
            }
            conn.sendall(json.dumps(response).encode())

        elif request["type"] == "Signup":
            sql.execute(
                "SELECT * FROM players WHERE username = ?", (request["username"],)
            )
            result = sql.fetchall()
            if len(result) > 0:
                conn.sendall("Taken".encode())
            else:
                sql.execute(
                    "INSERT INTO players (username, password, wins, loses) VALUES (?, ?, ?, ?)",
                    (request["username"], request["password"], 0, 0),
                )
                db.commit()
                conn.sendall("Done".encode())

        elif request["type"] == "Login":
            sql.execute(
                "SELECT * FROM players WHERE username = ? AND password = ?",
                (request["username"], request["password"]),
            )
            result = sql.fetchall()
            if len(result) > 0:
                myid = result[0][0]
                players[myid] = {
                    "id": myid,
                    "username": result[0][1],
                    "status": "login",
                    "pos": [133, -23, 61, 0, 0, 0],
                    "f": False
                }
                conn.sendall("Done".encode())
            else:
                conn.sendall("Failed".encode())

        elif request["type"] == "Create":
            if activeGame != -1:
                response = {
                    "status": "Failed",
                    "activeGame": -1,
                    "team": -1
                }
                conn.sendall(json.dumps(response).encode())
                continue
            now = datetime.now()
            current_time = now.strftime("%Y-%m-%d %H:%M:%S")
            sql.execute(
                "INSERT INTO games (date, players, winners) VALUES (?, ?, ?)",
                (current_time, "N N N N N N N N N N N N", "N N N"),
            )
            db.commit()
            sql.execute("SELECT id FROM games ORDER BY id DESC LIMIT 1")
            result = sql.fetchall()
            while True:
                team = randint(1, 4)
                if len(teams[team]) < 3:
                    teams[team].append(myid)
                    myteam = team
                    break
            players[myid]["status"] = "ready"
            activeGame = result[0][0]
            response = {
                "status": "Success",
                "activeGame": activeGame,
                "team": myteam
            }
            conn.sendall(json.dumps(response).encode())

        elif request["type"] == "isBegin":
            for player in players.values():
                if player["status"] == "ready":
                    readyCount += 1
            if readyCount == 12:
                counter = 0
                for player in players.values():
                    player["pos"] = [player["pos"][0] +
                                     counter * 20, -23, 61, 0, 0, 0]
                    counter += 1
                conn.sendall("Yes".encode())
            else:
                conn.sendall("No".encode())
                readyCount = 0

        elif request["type"] == "Lobby":
            if isBegin:
                response = {
                    "type": "Begin",
                }
                conn.sendall(json.dumps(response).encode())
            else:
                response = {
                    "type": "Lobby",
                }
                counter = 0
                for team in teams:
                    if len(teams[team]) > 0:
                        for player in teams[team]:
                            id = "player" + str(counter)
                            newPlayer = {
                                "team": team,
                                "username": players[player]["username"],
                                "status": players[player]["status"]
                            }
                            response[id] = json.dumps(newPlayer)
                            counter += 1
                    else:
                        id = "player" + str(counter)
                        newPlayer = {
                            "team": -1,
                            "username": "Empty",
                            "status": "Empty"
                        }
                        response[id] = json.dumps(newPlayer)
                        counter += 1
                while counter < 12:
                    id = "player" + str(counter)
                    newPlayer = {
                        "team": -1,
                        "username": "Empty",
                        "status": "Empty"
                    }
                    response[id] = json.dumps(newPlayer)
                    counter += 1
                conn.sendall(json.dumps(response).encode())

        elif request["type"] == "Join":
            sql.execute("SELECT id FROM games WHERE id = ?", (request["id"],))
            result = sql.fetchall()
            if len(result) == 0:
                response = {
                    "status": "Failed",
                    "activeGame": -1,
                    "team": -1
                }
                conn.sendall(json.dumps(response).encode())
            elif activeGame != result[0][0]:
                response = {
                    "status": "Active",
                    "activeGame": -1,
                    "team": -1
                }
                conn.sendall(json.dumps(response).encode())
            else:
                players[myid]["status"] = "ready"
                while True:
                    team = randint(1, 4)
                    if len(teams[team]) < 3:
                        teams[team].append(myid)
                        myteam = team
                        break
                response = {
                    "status": "Success",
                    "activeGame": result[0][0],
                    "team": team
                }
                conn.sendall(json.dumps(response).encode())

        elif request["type"] == "Status":
            if players[myid]["status"] == "ready":
                players[myid]["status"] = "notready"
            else:
                players[myid]["status"] = "ready"
            conn.sendall("Done".encode())

        elif request["type"] == "Start":
            isBegin = True
            conn.sendall("Done".encode())

        elif request["type"] == "getPlayers":
            response = {
                "myid": myid,
            }
            counter = 0
            for team in teams:
                for player in teams[team]:
                    id = "player" + str(counter)
                    players[player]["pos"][0] = positions[counter][0]
                    players[player]["pos"][1] = positions[counter][1]
                    players[player]["pos"][2] = positions[counter][2]
                    newPlayer = {
                        "id": players[player]["id"],
                        "username": players[player]["username"],
                        "x": players[player]["pos"][0],
                        "y": players[player]["pos"][1],
                        "z": players[player]["pos"][2],
                        "rx": players[player]["pos"][3],
                        "ry": players[player]["pos"][4],
                        "rz": players[player]["pos"][5],
                        "team": team
                    }
                    response[id] = json.dumps(newPlayer)
                    counter += 1
            while counter < 12:
                id = "player" + str(counter)
                newPlayer = {
                    "id": -1,
                    "username": "Empty",
                    "x": -1,
                    "y": -1,
                    "z": -1,
                    "rx": -1,
                    "ry": -1,
                    "rz": -1,
                    "team": -1
                }
                response[id] = json.dumps(newPlayer)
                counter += 1
            conn.sendall(json.dumps(response).encode())

        elif request["type"] == "mypos":
            players[myid]["pos"] = [
                request["x"], request["y"], request["z"], request["rx"], request["ry"], request["rz"]]
            response = {
                "t1": scores[0],
                "t2": scores[1],
                "t3": scores[2],
                "t4": scores[3],
            }
            counter = 0
            for team in teams:
                for player in teams[team]:
                    id = "player" + str(counter)
                    newPlayer = {
                        "id": players[player]["id"],
                        "x": players[player]["pos"][0],
                        "y": players[player]["pos"][1],
                        "z": players[player]["pos"][2],
                        "rx": players[player]["pos"][3],
                        "ry": players[player]["pos"][4],
                        "rz": players[player]["pos"][5],
                        "f": players[player]["f"]
                    }
                    response[id] = json.dumps(newPlayer)
                    counter += 1
            while counter < 12:
                id = "player" + str(counter)
                newPlayer = {
                    "id": -1,
                    "x": -1,
                    "y": -1,
                    "z": -1,
                    "rx": -1,
                    "ry": -1,
                    "rz": -1,
                    "f": False,
                }
                response[id] = json.dumps(newPlayer)
                counter += 1
            conn.sendall(json.dumps(response).encode())

        elif request["type"] == "freeze":
            players[request["id"]]["f"] = True
            scores[request["team"]-1] += 1
            conn.sendall("Done".encode())

    conn.close()
    db.close()


sock = establishConnections(3389)
signal.signal(signal.SIGINT, signal_handler)
isBegin = False
activeGame = -1
players = {}
positions = [
    [131, -23, 144],
    [34, -23, 71],
    [-77, -23, 74],
    [-278, -23, 100],
    [-269, -23, -548],
    [-410, -23, -686],
    [-70, -23, -549],
    [61, -23, -693],
    [80, -23, -686],
    [177, -23, -402],
    [154, -23, -78],
    [32, -23, -443],
]
random.shuffle(positions)
scores = [0, 0, 0, 0]
teams = {
    1: [],
    2: [],
    3: [],
    4: []
}

while True:
    conn, addr = sock.accept()
    start_new_thread(worker, (conn, addr))
