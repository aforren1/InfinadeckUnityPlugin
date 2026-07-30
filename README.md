# Infinadeck Unity Plugin

Unity plugin for implementing Infinadeck features and assets in PCVR projects. It translates
outputs from the Infinadeck Desktop Application into true 1:1 motion in your virtual reality
project.

Designed for anyone with access to an Infinadeck omnidirectional treadmill, or anyone building VR
apps with the Infinadeck in mind. Lightweight but feature-full and simple to set up.

- **Core** (`Infinadeck.Core`) loads and unloads the other modules as needed to keep the project
  lightweight.
- **Locomotion** (`Infinadeck.Locomotion`) provides 1:1 motion mapping with no configuration.
- **Reference Objects** (`Infinadeck.ReferenceObjects`) render the treadmill in VR in real time,
  providing grounding elements for comfort.
- **Splashscreen** (`Infinadeck.Splashscreen`) confirms the treadmill is connected and working.

## Requirements

- **Unity 6.x** (`6000.0`+)
- **Universal Render Pipeline (URP)** (the Built-in Render Pipeline and HDRP are not supported)
- **Active Input Handling** set to *Input System Package* (or *Both*) in
  *Project Settings ▸ Player ▸ Other Settings*
- Windows PCVR (the native `InfinadeckAPI.dll` is Windows x64)

The package depends on `com.unity.inputsystem` and `com.unity.ugui`; Package Manager installs them
automatically.

## Installation (UPM)

Package Manager ▸ **Add package from git URL…**:

```
https://github.com/wti-brainworks/infinadeckunityplugin.git#modernize-upm-unity6
```

…or add to your project's `Packages/manifest.json`:

```json
"com.infinadeck.unityplugin": "https://github.com/wti-brainworks/infinadeckunityplugin.git#modernize-upm-unity6"
```

For local development, use **Add package from disk…** and select this folder's `package.json`.

### After installing

1. **Samples (optional).** In Package Manager, select this package ▸ **Samples** ▸ import
   *Demo Scene* (legacy treadmill demo) or *XR Playground* (headset/controller/hand tracking test scene
   with body collision and coin collecting, built on the XR Interaction Toolkit reference rig;
   see the sample's README for the required XRI and XR Hands sample imports).
2. Drag the **`[Infinadeck]`** prefab (`Runtime/Prefabs`) into your scene and assign your
   Camera Rig and Headset references on the `Infinadeck.Core` component.

## Keybinds

Keyboard controls are read through the Input System and configured in
`Documents/My Games/Infinadeck/Config/keybinds.ini`. As of 4.0.0 the file uses Input System `Key`
names (e.g. `Digit1`, `Numpad1`, `Enter`). Legacy `UnityEngine.KeyCode` names (`Alpha1`, `Keypad1`,
`Return`, `BackQuote`, `LeftControl`) are still accepted for backward compatibility.

Built-in shortcuts: `Ctrl+I` toggles the plugin, `Esc` quits, `=` hides the on-screen
notifications.

## Documentation

Advanced documentation, including implementation instructions and the full keyboard reference, is
in the PDF under `Documentation~/` in this package. See `CHANGELOG.md` for the 4.0.0 migration
notes.
