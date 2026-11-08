# SpareSquare

SpareSquare is a Unity puzzle game for Android. Players rotate groups of four tiles to complete patterns on a square board, earn points, and try to beat their best scores.

## Game modes

- **Unranked** — play at your own pace or choose a timed session. Scores are saved locally and shown in the unranked leaderboard.
- **Ranked** — sign in with Google, submit scores to the online leaderboard, and compete with other players.
- **Board sizes** — select square boards from 2×2 through 9×9.
- **Audio controls** — adjust menu and gameplay audio from the game UI.

## Built with

- Unity `6000.3.8f1`
- Unity UI (uGUI)
- Google Sign-In for Unity
- UnityWebRequest for the ranked account and leaderboard API

## Open the project

1. Install Unity `6000.3.8f1` with Android Build Support, including the Android SDK and NDK and OpenJDK modules.
2. Clone this repository and open its root folder in Unity Hub.
3. Let Unity resolve packages and import assets.
4. Open `Assets/Scenes/First Theme Blue/MainMenu.unity` to work on the active menu scene. The gameplay scenes are `Assets/Scenes/First Theme Blue/UnrankedGame.unity` and `Assets/Scenes/First Theme Blue/RankedGame.unity`.
5. Use **File → Build Profiles** to select Android and configure the app before building.

## Project layout

- `Assets/Scenes/First Theme Blue/` — active menu, unranked, and ranked scenes.
- `Assets/Script/` — puzzle board, tile interaction, game flow, menu, leaderboard, and local score scripts. Main components include `PuzzleBoard`, `PuzzleGameController`, `MainMenuController`, and `LeaderboardController`.
- `Assets/RankedAccountController.cs` — Google sign-in and ranked account API integration.
- `Assets/RankedScoreService.cs` — ranked score submission and leaderboard retrieval.
- `Assets/Prefab/` — reusable game and score UI prefabs.
- `Assets/GUI/` — interface artwork and fonts.
- `Assets/GoogleSignIn/` — Google Sign-In plugin and Android support files.
- `Packages/manifest.json` — Unity package dependencies.

## Service configuration

Ranked play depends on Google Sign-In and the project’s score/account API. Set the Google web client ID and API connection settings on the corresponding components in the Unity Inspector, and configure the Android OAuth client with the app’s package name and signing certificate.

Do not put private service credentials, service account keys, or privileged server API tokens in a Unity client build. Store server credentials on the backend and have the game call the backend.

Unranked scores and player preferences are stored locally with Unity `PlayerPrefs`.

## Notes

The repository also contains duplicate and sample scenes that are not in the active Android build scene list. Review the Build Profiles scene list and Android player settings before creating a release build.
