# Unity Project
Hazardifier depends on a Unity project for the custom claymore object, this project depends on a free but not distributable M18 Claymore mine. 

You will need to "purchase" the following free asset with your Unity account: https://assetstore.unity.com/packages/3d/props/guns/m18-claymore-game-ready-165374

To set up the Unity project, follow these steps:
1. In Unity Hub, click Add -> Add project from disk
2. Select the `UnityProject\HazardifierMine` folder
3. Load the imported project
4. In Unity, select Window -> Package Manager
5. Find the M18 Claymore asset, and import it
6. In the project asset list, select the M18 Claymore/Textures, and set the "Max Size" of all textures to 1024 to save on bundle size
7. From the project asset list, drag the Scenes/MineScene scene into the project, and remove the sample scene
8. If the AssetBundles window tab isn't open, select Window -> AssetBundle Browser
9. In the AssetBundles tab, select the Build tab, and hit Build
