# Online Multiplayer UNO

A **four-player online UNO game prototype** developed independently in **C# and Unity**, using **Photon PUN** for multiplayer rooms, networked card objects and synchronisation of game events.

Created as an **Extended Project Qualification (EPQ)**, this project explores the challenges of adapting a turn-based card game for multiple clients: keeping turns and card collections consistent, maintaining a player-specific view of the table, and coordinating game rules over a network.

> **Repository note:** This repository is a **source-code archive** of the game's Unity C# scripts, not a complete Unity project. Scenes, prefabs, materials, Photon configuration and other assets are not included, so it cannot be launched directly from a fresh clone.

## Features

- **Four-player rooms:** Create or join a named Photon room with a maximum of four players.
- **Networked deck and hands:** Create the deck, shuffle and distribute seven starting cards per player, then synchronise card and hand changes between clients.
- **Turn-based gameplay:** Track the active player and play direction; check whether a card can legally be played and update the local interface accordingly.
- **Action cards:** Support Draw Two, Reverse, Skip, Wild and Wild Draw Four, including colour selection and a stacking mechanic for draw cards.
- **UNO interactions:** Declare UNO and call out a player who has one card remaining without declaring it, with a draw-card penalty in the call-out logic.
- **End-of-game flow:** Detect the game-ending condition and use Photon room properties to share player-placement information for the finish screen.

## Multiplayer design

The game uses **Photon PUN Remote Procedure Calls (RPCs)** and **PhotonView ownership** to coordinate shared game events without a dedicated custom game server.

1. **Room creation and joining:** `CreateAndJoinRoom.cs` creates or joins rooms. Photon assigns each participant an actor number, which the game uses to identify their seat and hand.
2. **Deck setup:** In `DeckManager.cs`, the Photon **Master Client initiates** card creation and shuffling, then broadcasts the shuffled order to other clients. Each client maintains its own corresponding card collections.
3. **Card ownership and visibility:** Cards are represented by networked objects. Ownership is transferred when cards are dealt or drawn, while their face-up/face-down presentation depends on the local player.
4. **Synchronised actions:** RPCs propagate events such as playing a card, changing the current colour, modifying hands, advancing turns and changing play direction.
5. **Local table layout:** Card positions and player seating are calculated relative to the local client's actor number, giving each player their own view of the same match.

**Design scope:** This is a client-side multiplayer prototype using Photon-hosted networking services. The repository does not contain a dedicated authoritative game server, and it does not demonstrate comprehensive handling of disconnections, reconnections or malicious clients.

## Key implementation files

| Script | Responsibility |
| --- | --- |
| `ConnectToServer.cs`, `CreateAndJoinRoom.cs` | Photon connection, lobby and room flow |
| `GameManager.cs` | Turn order, active colour, game state and placement calculation |
| `DeckManager.cs` | Deck creation, shuffling, dealing and synchronisation |
| `Card.cs` | Card representation, playability, interaction and networked card updates |
| `ActionCard.cs` | Special-card effects and draw-card stacking |
| `Player.cs`, `PlayerBehaviour.cs` | Player identity, card drawing and UNO status |
| `ColourChoicePopUpPanel.cs`, `CallOutPopUpPanel.cs` | Wild-card colour selection and UNO call-out UI |
| `WinningScenePrefabChange.cs` | Player-placement display on the finish screen |

## Technologies

- **C#** — game logic and component scripts
- **Unity** — game objects, scenes, UI and client-side interaction
- **Photon PUN** — matchmaking/rooms, RPCs, networked objects and ownership transfer

## Running the game

The files in this repository **are not sufficient to build or run the original game on their own**. The original Unity project would also require its scenes, prefabs, materials, UI configuration and Photon PUN settings (including a valid Photon App ID). In particular, the scripts reference scene objects such as `EGO` and asset names loaded from Unity `Resources` folders.

The code is provided for **review of the networking and gameplay implementation** rather than as a standalone playable release. No tested installation procedure is provided for reconstructing the full project from these scripts alone.

## Project background

Built independently for an EPQ to investigate multiplayer application development through a familiar turn-based game. The main technical focus was **synchronising distributed game actions and client-side state**: deck order, card ownership, player hands, turn progression and player-specific presentation.

This is an **educational fan-made prototype**, not an official UNO product or an affiliation with its rights holders.
