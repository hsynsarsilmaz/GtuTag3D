# GtuTag3D

GtuTag3D is an online multiplayer game where 12 players, divided into 4 different teams, compete to win by freezing members of opposing teams. The game map is a 3D virtual representation of the first floor of the GTU Computer Engineering building.

## Gameplay and Rules

- **Controls**: Players move using standard WASD controls and use the SPACE key to freeze other players.
- **Objective**: The main goal is to "freeze" players from opposing teams. If two players from the same team catch a solo member of another team, either of them can freeze the solo player and earn one point.
- **Game Music**: An 8-bit remix of "Little Dark Age" by MGMT is played at the start of the game, which also serves as a timer. When the song ends, the team with the highest score is declared the winner.

## Technical Details

- **Server/Client Model**: The game uses a server/client architecture.
- **Server**: Developed with Python.
- **Client**: Developed using the Unity Engine and C#.

## How to Run the Game

### For Windows Users

1. **Download the Builds**: 
   - Get the latest builds from the [release page](https://github.com/hsynsarsilmaz/GtuTag3D/releases/tag/v1.0).
   
2. **Run the Server**:
   - Extract and run `GtuTag3D_Server.zip`. The server will run on `localhost`.

3. **Run the Client**:
   - Extract and run `GtuTag3D_Client.zip`. The client will connect to `localhost` by default.
   - Note: This game requires 12 players, so you'll need to run 12 client instances on the same machine for the game to start.

4. **Multiple Computers**:
   - If you want to run the game on more than one computer, find the server's IP using the `ipconfig` command on Windows.
   - Modify the `ipconfig.conf` file in the `GTU Tag 3D_Data` subdirectory to match the server's IP address. Ensure that only the IP address is in the file, as any other content may break the game.

### For Non-Windows Users

While GtuTag3D is designed for Windows, it should work on other operating systems due to Unity's multi-platform support. However, this is not guaranteed.

1. **Clone the Repository**:
   - Clone the repository to your machine.

2. **Run the Server**:
   - Navigate to the `Server` subdirectory and run the server using the following command:
     ```bash
     python3 Server/Server.py
     ```

3. **Build the Client**:
   - Use Unity Editor to build the project for your operating system.
   - You can also run the client directly from the Unity Engine, but this will only support one instance at a time.

## Screenshots

| Main Menu | Login Page |
|-----------|------------|
| ![Game Menu](https://raw.githubusercontent.com/hsynsarsilmaz/GtuTag3D/main/Assets/Images/ss1.png) | ![Login Page](https://raw.githubusercontent.com/hsynsarsilmaz/GtuTag3D/main/Assets/Images/ss2.png) |

| Lobby | Gameplay |
|-------|----------|
| ![Lobby](https://raw.githubusercontent.com/hsynsarsilmaz/GtuTag3D/main/Assets/Images/ss3.png) | ![Gameplay](https://raw.githubusercontent.com/hsynsarsilmaz/GtuTag3D/main/Assets/Images/ss4.png) |
