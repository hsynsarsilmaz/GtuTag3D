## Project Definition

GtuTag3D is a online multiplayer game, which 12 players in 4 different teams tries to win by freezing other teams' players. The game map is a 3D virtualization of GTU Computer Engineering building 1st floor. 

## Gameplay and Rules

Players move with standart WASD controls and use SPACE for freezing other players. The aim of the game is 'freeze' other players. If a 2 player of a same team cathces a solo member of another team, either player of 2 member team freeze the solo one and gain one point. When the game starts a 8 bit remix of Little Dark Age by MGMT is played this song also denotes the game time. When the song over the team with most score is declared as winner

## Techical Details
The game uses Server/Client model. The server is developed with Python, the client is developed with Unity Engine using C#.

## How to Run the Game
To run the game firstly the server must be started. Originally the server was running on a Google Cloud Virtual Machine but the source code is modified to run in localhost. Firstly a MYSQL server must be running and necessarsy database and tables must be created this could be done by running these commands.
```sql
CREATE DATABASE gtutag3d;

CREATE TABLE games (
    id INT AUTO_INCREMENT PRIMARY KEY,
    date DATETIME NOT NULL,
    players VARCHAR(255) NOT NULL,
    winners VARCHAR(255) NOT NULL
);

CREATE TABLE players (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(255) NOT NULL,
    password VARCHAR(255) NOT NULL,
    wins INT NOT NULL,
    loses INT NOT NULL
);
```
Once the server is running clients can connect and play the game, the final build of the client can be downloaded [here](https://www.mediafire.com/file/81qltqi6o4ji63c/GtuTag3D-Build.zip/file). Or it can be built with Unity Engine using source code.

For game to start 12 clients must be connected to server, for testing purposes build can be executed 12 times and with logging in as 12 different uses gameplay can be tested with one machine.
