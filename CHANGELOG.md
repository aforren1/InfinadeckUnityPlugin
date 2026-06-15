# Changelog

All notable changes to this package are documented here. This project adheres to
[Semantic Versioning](https://semver.org/).

## [4.0.1]

Major modernization release. **Breaking** — targets Unity 6.x, URP/HDRP, and the Input System.

### Added
- Unity Package Manager (UPM) support: `package.json`, `Runtime`/`Editor` assembly definitions
  (`Infinadeck`, `Infinadeck.Editor`), and a Package Manager **Demo Scene** sample.
- URP and HDRP variants of the two custom shaders (`Unlit/InfRefObjShader`,
  `Transparent/Cutout/TransparentInf`) as multi-SubShader assets.

### Changed
- **Namespaces & names:** every type now lives in the `Infinadeck` namespace, and the
  `Infinadeck`/`Infina` prefixes were dropped (e.g. `InfinadeckCore` → `Infinadeck.Core`,
  `InfinadeckLocomotion` → `Infinadeck.Locomotion`, `InfinaDATA` → `Infinadeck.Data`,
  `InfinaKEYBIND` → `Infinadeck.Keybind`). The native API wrapper `Infinadeck.Infinadeck`
  is now `Infinadeck.Sdk`, and `InfinadeckInitError` is now `Infinadeck.InitError`.
- **Input:** all keyboard handling moved from the legacy Input Manager to the Input System.
  `keybinds.ini` now uses Input System `Key` names. Legacy `UnityEngine.KeyCode` names
  (e.g. `Alpha1`, `Keypad1`, `Return`, `BackQuote`, `LeftControl`) are still accepted for
  backward compatibility, but new defaults are written as `Digit1`, `Numpad1`, `Enter`, etc.
- Replaced deprecated `Object.FindObjectOfType` with `Object.FindFirstObjectByType` and
  `[ExecuteInEditMode]` with `[ExecuteAlways]`.
- Consolidated the duplicate read-only inspector attribute into a single
  `Infinadeck.ReadOnlyInEditorAttribute` (`[ReadOnlyInEditor]`).

### Requirements / migration notes
- Unity **6000.0+** with **URP or HDRP** (the Built-in Render Pipeline is no longer targeted).
  Run the pipeline material converter once after install to upgrade the remaining
  Standard/legacy-shader art assets to your chosen pipeline.
- Set **Player Settings ▸ Active Input Handling** to *Input System Package* (or *Both*); the
  package compiles against `ENABLE_INPUT_SYSTEM` and errors clearly if it is missing.
- The package depends on `com.unity.inputsystem` and `com.unity.ugui`, pulled in automatically.

## [3.3.1]
- Final Built-in Render Pipeline / legacy Input release (pre-modernization baseline).
