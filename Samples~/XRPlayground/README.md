# XR Playground

A small scene for checking that XR tracking, hands, body collision, and
Infinadeck locomotion all work together, built on the XR Interaction Toolkit
reference rig. Targets Windows PCVR (tested with a Galaxy XR used as the
PCVR headset, and XRI 3.1); it should behave the same on any OpenXR runtime.

## What it shows

- The **XR Origin Hands (XR Rig)** prefab from the XR Interaction Toolkit
  samples: headset tracking, controller models, full hand and finger meshes,
  and automatic switching between controllers and hands.
- XRI's built-in **locomotion**: left thumbstick to move, right thumbstick to
  turn, plus teleport.
- **Body collision** ([BodyPushback.cs](Scripts/BodyPushback.cs)): XRI's
  CharacterController already blocks stick locomotion, but physically walking
  or leaning still lets the camera pass through geometry. This script keeps
  the capsule under the headset and pushes the rig back out of walls.
- **Coin collecting** ([CoinCollectors.cs](Scripts/CoinCollectors.cs)): touch
  the spinning coins with a palm or controller to collect them. A floating
  scoreboard keeps count.
- **Infinadeck locomotion** ([InfinadeckSetup.cs](Scripts/InfinadeckSetup.cs)):
  the `[Infinadeck]` prefab is in the scene, wired to the XRI rig at runtime.
  On Windows with the Infinadeck Desktop Application running, walking on the
  treadmill moves the XR Origin just like the thumbstick does.

## Setup (import order matters)

1. Install and configure **OpenXR** in *Project Settings > XR Plug-in
   Management* (Windows tab). Enable the **Hand Tracking Subsystem** OpenXR
   feature and an interaction profile matching your controllers. When the
   headset reaches the PC through a streaming app, that app must forward
   hand tracking for the hand visuals to appear.
2. Install **`com.unity.xr.hands`** and import its **HandVisualizer** sample
   (it provides the hand meshes the rig uses).
3. Install **`com.unity.xr.interaction.toolkit`** (3.x) and import its
   **Starter Assets** sample, then its **Hands Interaction Demo** sample.
4. Import this sample.

The scene references the rig prefab from the Hands Interaction Demo, so steps
2 and 3 must be complete before the scene will load with an intact rig.

## Notes

- The two sample scripts locate the rig through `XROrigin` at runtime, so you
  can swap in any other XR Origin based rig (for example the plain Starter
  Assets rig, controllers only) without touching the scene wiring.
- Coins are plain trigger colliders, deliberately independent of XRI's
  interactor stack, so collection works identically with controllers, hands,
  or any custom rig.
- The `[Infinadeck]` prefab starts inactive. `InfinadeckSetup` assigns its
  Camera Rig and Headset references to the XR Origin and then activates it,
  so `Core` boots with the right rig without any manual inspector wiring.
  No treadmill is required to run the scene: without the Infinadeck Desktop
  Application running, the plugin just shows its connection notice. (The
  native API is Windows only; on any non-Windows platform the prefab stays
  inactive and everything else works as usual.)
