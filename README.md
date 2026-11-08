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
4. Open `Assets/Scenes/MainMenu 1.unity` to work on the active menu scene. Gameplay scenes are in `Assets/Scenes/` and `Assets/Scenes/First Theme Blue/`.
5. Use **File → Build Profiles** (or **Build Settings** in older Unity workflows) to select Android and configure the app before building.

## Project layout

- `Assets/Scenes/` — menu and gameplay scenes, including ranked and unranked layouts.
- `Assets/Script/` — board setup, tile/pattern interaction, scoring, menus, and leaderboard UI logic.
- `Assets/Prefab/` — reusable game and score UI prefabs.
- `Assets/GUI/` — interface artwork and fonts.
- `Assets/GoogleSignIn/` — Google Sign-In plugin and Android support files.
- `Packages/manifest.json` — Unity package dependencies.

## Service configuration

Ranked play depends on Google Sign-In and the project’s score/account API. Configure the Google web client ID and API connection settings on the corresponding scene components in the Unity Inspector, and provide any required Android OAuth/package signing configuration. Do not commit private API tokens or signing credentials to the repository.

Unranked scores and player preferences are stored locally with Unity `PlayerPrefs`.

## Notes

The project contains sample scenes and plugin files alongside the game scenes. Check the Build Profiles scene list and Android player settings before creating a release build.
