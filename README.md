# SpareSquare

<p align="center">
  <strong>A colorful pattern-matching puzzle built with Unity.</strong><br />
  Rotate, swap, and line up tile patterns to build your score.
</p>

<p align="center">
  <img alt="Unity 6000.3.8f1" src="https://img.shields.io/badge/Unity-6000.3.8f1-black?logo=unity" />
  <img alt="Platform Android" src="https://img.shields.io/badge/Platform-Android-3DDC84?logo=android&logoColor=white" />
  <img alt="Genre Puzzle" src="https://img.shields.io/badge/Genre-Puzzle-6C63FF" />
</p>

SpareSquare is a mobile puzzle game where each tile is made of colorful shapes. Make matches across the board, build your score, and choose between relaxed solo play and ranked competition.

## Contents

- [Highlights](#highlights)
- [How to play](#how-to-play)
- [Game modes](#game-modes)
- [Technology](#technology)
- [Get started](#get-started)
- [Project structure](#project-structure)
- [Services and configuration](#services-and-configuration)
- [Saved data](#saved-data)
- [Build for Android](#build-for-android)

## Highlights

- Pattern-based puzzle play with **shape, color, and type** matches.
- Square boards from **2×2 to 9×9**.
- Tile swapping and tile rotation.
- Timed and untimed sessions.
- Local high scores for unranked play.
- Google sign-in and online ranked scoreboards.
- Adjustable music and sound effects.

## How to play

1. Choose a game mode and board size from the main menu.
2. Swap neighboring tiles by dragging them to another board position.
3. Tap a tile to rotate its four pattern sections.
4. Match pattern attributes across neighboring edges to earn points.
5. Keep playing to improve your score, or submit it when the round ends.

## Game modes

| Mode | What to expect |
| --- | --- |
| **Unranked** | Play at your own pace or select a timed round. Scores are saved locally and displayed in the unranked leaderboard. |
| **Ranked** | Sign in with Google and submit scores to the online leaderboard. |

The menu offers board sizes from **2×2 through 9×9**. Timed options are selected before starting a round; the untimed option lets you play without a countdown.

## Technology

- **Unity `6000.3.8f1`**
- **C#** gameplay and UI logic
- **Unity UI (uGUI)**
- **Google Sign-In for Unity**
- **UnityWebRequest** for ranked account and leaderboard requests
- **Unity `PlayerPrefs`** for local preferences and unranked scores

## Get started

### Requirements

- Unity `6000.3.8f1`
- Android Build Support for Android builds, including the Android SDK, NDK, and OpenJDK modules
- Git, to clone the repository

### Open in Unity

1. Clone this repository.
2. In Unity Hub, choose **Add project from disk** and select the repository folder.
3. Open the project with Unity `6000.3.8f1`.
4. Let Unity import assets and resolve packages.
5. Open the active menu scene: `Assets/Scenes/First Theme Blue/MainMenu.unity`.

## Project structure

| Path | Purpose |
| --- | --- |
| `Assets/Scenes/First Theme Blue/` | Active menu, unranked, and ranked game scenes. |
| `Assets/Script/PuzzleBoard.cs` | Creates and configures the selected square board. |
| `Assets/Script/Dot.cs` | Tile interaction, pattern matching, and score calculation. |
| `Assets/Script/PuzzleGameController.cs` | Gameplay flow, timer, moves, score, and round-end behavior. |
| `Assets/Script/MainMenuController.cs` | Menu options, mode selection, board size, and audio settings. |
| `Assets/Script/LeaderboardController.cs` | Displays locally stored unranked leaderboard entries. |
| `Assets/RankedAccountController.cs` | Google sign-in and ranked account API integration. |
| `Assets/RankedScoreService.cs` | Ranked score submission and online leaderboard retrieval. |
| `Assets/Prefab/` | Reusable gameplay and score UI prefabs. |
| `Assets/GUI/` | Interface art, sprites, and fonts. |
| `Assets/GoogleSignIn/` | Google Sign-In plugin files. |
| `Packages/manifest.json` | Unity package dependencies. |

## Services and configuration

Ranked play relies on Google Sign-In and the project’s account and score API. Configure the Google web client ID and API connection settings on the corresponding components in the Unity Inspector. The Android OAuth client must also match the app’s package name and signing certificate.

Keep private service credentials, service account keys, and privileged server tokens on the backend. A value embedded in a Unity client can be extracted from the distributed app, so client-side configuration should not be treated as secret storage.

## Saved data

Unranked scores, selected options, and audio preferences are stored locally with Unity `PlayerPrefs`. Ranked scores are sent to the configured online service.

## Build for Android

1. Open **File → Build Profiles**.
2. Select or add the **Android** profile and switch the project to Android if needed.
3. Confirm the active scenes are listed in this order:
   - `Assets/Scenes/First Theme Blue/MainMenu.unity`
   - `Assets/Scenes/First Theme Blue/UnrankedGame.unity`
   - `Assets/Scenes/First Theme Blue/RankedGame.unity`
4. Review the Android package identifier, orientation, icons, signing setup, and other Player settings for your release.
5. Choose **Build** or **Build and Run**.

The repository also includes duplicate and sample scenes for development. They are not part of the active Android build scene list.
