using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

#if !ENABLE_INPUT_SYSTEM
#error Infinadeck requires the Input System package. In Player Settings, set "Active Input Handling" to "Input System Package" (or "Both").
#endif

/**
 * ------------------------------------------------------------
 * Main script for management of the Infinadeck plugin.
 * https://github.com/Infinadeck/InfinadeckUnityPlugin
 * Created by Griffin Brunner @ Infinadeck, 2019-2022
 * Attribution required.
 * ------------------------------------------------------------
 */

namespace Infinadeck
{
public class Core : MonoBehaviour
{
    readonly string pluginVersion = "4.0.0";
    public string pluginVersionForEditorReference;
    [ReadOnlyInEditor] public GameObject refObjects;
    [ReadOnlyInEditor] public GameObject locomotion;
    [ReadOnlyInEditor] public GameObject splashScreen;
    [ReadOnlyInEditor] public GameObject demo;
    private ReferenceObjects refObjectsScript;
    private Locomotion locomotionScript;
    private Splashscreen splashScreenScript;
    private Demo demoScript;
    public Interpreter interpreter;

    public bool autoStart = true;
    private bool booted = false;
    public bool firstLevel = true;
    public bool movementLevel = true;
    public bool guaranteeDestroyOnLoad = false;
    public bool showCollisions = false;
    public bool showTreadmillVelocity = false;
    private bool initialized = false;
    private bool pluginActive = false;
    private bool keybindsActive = false;
    private bool demoActive = false;
    private bool hideActive = false;
    public string guiOutput;
    private DateTime _keybindsSyncedAt;
    private string _keybindsErrorInfo = "";
    private string _keybindsSectionStr = "";

    public Data preferences;
    public Dictionary<string, Data.DataEntry> defaultPreferences;
    public Data keybinds;
    public Dictionary<string, Data.DataEntry> defaultKeybinds;
    public Data gamePreferences;
    public Dictionary<string, Data.DataEntry> defaultGamePreferences;


    public GameObject cameraRig;
    public GameObject headset;
    public float speedGain = 1;
    public Vector3 originOffsetPosition;
    public Vector3 originOffsetRotation = Vector3.zero;
    public Vector3 originOffsetScale = Vector3.one;

    private Texture2D textBG;

    private void Reset() { pluginVersionForEditorReference = pluginVersion; }
    private void OnValidate() { pluginVersionForEditorReference = pluginVersion; }

    /**
     * Runs upon the moment of creation of this object.
     */
    void Awake()
    {
        // Delete Placement Geometry
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }

        interpreter = this.gameObject.AddComponent<Interpreter>();
        interpreter.enabled = false;

        // Initialize Preferences
        preferences = this.gameObject.AddComponent<Data>();
        preferences.fileLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/Infinadeck/Config/";
        preferences.fileName = "settings.ini";
        defaultPreferences = new Dictionary<string, Data.DataEntry>
        {
            // 000: General Preferences
            { "pluginEnabled", new Data.DataEntry { EntryName = "000 General", EntryValue = "true" } },
            { "hideNotifications", new Data.DataEntry { EntryName = "000 General", EntryValue = "false" } },
            { "demoMode", new Data.DataEntry { EntryName = "000 General", EntryValue = "false" } },
            { "keyboardInputEnabled", new Data.DataEntry { EntryName = "000 General", EntryValue = "true" } },
            { "rightHandDominant", new Data.DataEntry { EntryName = "000 General", EntryValue = "true" } },
            { "rightFootDominant", new Data.DataEntry { EntryName = "000 General", EntryValue = "true" } },
            
            // 010: Reference Object General Preferences
            { "overrideTreadmillPosition", new Data.DataEntry { EntryName = "010 ReferenceObject - General", EntryValue = "true" } },
            { "overrideX", new Data.DataEntry { EntryName = "010 ReferenceObject - General", EntryValue = "0.0000" } },
            { "overrideY", new Data.DataEntry { EntryName = "010 ReferenceObject - General", EntryValue = "0.0000" } },
            { "overrideZ", new Data.DataEntry { EntryName = "010 ReferenceObject - General", EntryValue = "1.0000" } },

            // 011: Reference Object #1- Ring Preferences
            { "ringVisibility", new Data.DataEntry { EntryName = "011 ReferenceObject01 - Ring", EntryValue = "true" } },
            { "ringModel", new Data.DataEntry { EntryName = "011 ReferenceObject01 - Ring", EntryValue = "1" } },
            { "ringDiameter", new Data.DataEntry { EntryName = "011 ReferenceObject01 - Ring", EntryValue = "1.2954" } },
            { "ringThickness", new Data.DataEntry { EntryName = "011 ReferenceObject01 - Ring", EntryValue = ".0381" } },

            // 012: Reference Object #2- Center Mark Preferences
            { "centerVisibility", new Data.DataEntry { EntryName = "012 ReferenceObject02 - CenterMark", EntryValue = "true" } },
            { "centerModel", new Data.DataEntry { EntryName = "012 ReferenceObject02 - CenterMark", EntryValue = "0" } },

            // 013: Reference Object #3- Edge Preferences
            { "edgeVisibility", new Data.DataEntry { EntryName = "013 ReferenceObject03 - Edge", EntryValue = "true" } },
            { "walkingSurfaceWidth", new Data.DataEntry { EntryName = "013 ReferenceObject03 - Edge", EntryValue = "1.2192" } },
            { "walkingSurfaceEdgeThickness", new Data.DataEntry { EntryName = "013 ReferenceObject03 - Edge", EntryValue = ".03" } },

            // 014: Reference Object #4- (In Engine) Deck Preferences
            { "deckVisibility", new Data.DataEntry { EntryName = "014 ReferenceObject04 - Deck", EntryValue = "false" } },
            { "deckHeadingVisibility", new Data.DataEntry { EntryName = "014 ReferenceObject04 - Deck", EntryValue = "false" } },

            // 015: Reference Object #5- Dynamic Panel Preferences
            { "dynamicRingPanel", new Data.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "false" } },
            { "panelWidthM", new Data.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "0.15" } },
            { "panelHeightM", new Data.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "0.05" } },
            { "panelDiameterM", new Data.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "1.3" } },
            { "bandThicknessPercent", new Data.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "6" } },
            { "topBoundaryThicknessPercent", new Data.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "3" } },
            { "bottomBoundaryThicknessPercent", new Data.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "4" } },
            { "dynamicBackdrop", new Data.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "false" } },
            { "panelPalette", new Data.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "0" } },
            { "colorblindMode", new Data.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "false" } },
            { "dynamicColorblindElements", new Data.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "true" } },
            { "dynamicColorblindFrames", new Data.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "2000" } },
            { "maxTreadmillSpeedMetersPerSecond", new Data.DataEntry { EntryName = "015 ReferenceObject05 - DynamicPanel", EntryValue = "2" } },

            // 019: Reference Object Reference Info
            { "ringModel_AS_0", new Data.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "minimum_detail:_48_verts_(96_tris)" } },
            { "ringModel_AS_1", new Data.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "low_detail:_128_verts_(256_tris)" } },
            { "ringModel_AS_2", new Data.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "medium_detail:_256_verts_(512_tris)" } },
            { "ringModel_AS_3", new Data.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "high_detail:_512_verts_(1024_tris)" } },
            { "ringModel_AS_4", new Data.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "maximum_detail:_1024_verts_(2048_tris)" } },
            { "ringModel_AS_5", new Data.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "ludicrous_detail:_4096_verts_(8192_tris)" } },
            { "centerModel_AS_0", new Data.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "target" } },
            { "centerModel_AS_1", new Data.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "cross" } },
            { "centerModel_AS_2", new Data.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "extended" } },
            { "centerModel_AS_3", new Data.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "feet" } },
            { "centerModel_AS_4", new Data.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "star" } },
            { "panelPalette_AS_0", new Data.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "black-BG_grey-boundary_white-band" } },
            { "panelPalette_AS_1", new Data.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "grey-BG_white-boundary_synced-band" } },
            { "panelPalette_AS_2", new Data.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "synced-BG_white-boundary_white-band" } },
            { "panelPalette_AS_3", new Data.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "no-BG_white-boundary_synced-band" } },
            { "panelPalette_AS_4", new Data.DataEntry { EntryName = "019 Reference: ReferenceObject Profiles", EntryValue = "no-BG_white-boundary_white-band" } },

            // 700: Operational
            { "crashCheck", new Data.DataEntry { EntryName = "700 Operational", EntryValue = "false" } }
        };
        preferences.all = defaultPreferences;
        preferences.Initialize(false); // keep any existing settings

        keybinds = this.gameObject.AddComponent<Data>();
        keybinds.fileLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/Infinadeck/Config/";
        keybinds.fileName = "keybinds.ini";
        defaultKeybinds = new Dictionary<string, Data.DataEntry>
        {
            // 999: Keybind Reference Info
            { "FUNC", new Data.DataEntry { EntryName = "999 Reference: Keybind Profiles", EntryValue = "F1-F2-F3-F4-F5-F6-F7-F8-F9-F10-F11-F12" } },
            { "1234", new Data.DataEntry { EntryName = "999 Reference: Keybind Profiles", EntryValue = "Digit1-Digit2-Digit3-Digit4-Digit5-Digit6-Digit7-Digit8-Digit9-Digit0-Minus-Equals" } },
            { "#PAD", new Data.DataEntry { EntryName = "999 Reference: Keybind Profiles", EntryValue = "Numpad1-Numpad2-Numpad3-Numpad4-Numpad5-Numpad6-Numpad7-Numpad8-Numpad9-NumpadDivide-NumpadMultiply-NumpadMinus" } },
            { "STND", new Data.DataEntry { EntryName = "999 Reference: Keybind Profiles", EntryValue = "LeftShift-LeftCtrl-LeftAlt-Space-RightShift-RightCtrl-RightAlt-Enter-Backquote-Tab-Backslash-Backspace" } },
            { "CPAD", new Data.DataEntry { EntryName = "999 Reference: Keybind Profiles", EntryValue = "LeftArrow-DownArrow-RightArrow-UpArrow-Delete-End-PageDown-Insert-Home-PageUp-ScrollLock-Pause" } },
            { "QWER", new Data.DataEntry { EntryName = "999 Reference: Keybind Profiles", EntryValue = "Q-W-E-R-T-Y-U-I-O-P-LeftBracket-RightBracket" } },
            { "ASDF", new Data.DataEntry { EntryName = "999 Reference: Keybind Profiles", EntryValue = "A-S-D-F-G-H-J-K-L-Semicolon-Quote-Slash" } },
            { "Custom", new Data.DataEntry { EntryName = "999 Reference: Keybind Profiles", EntryValue = "karses_the_12_keys_listed_in_the_keybindProfile" } }
        };
        keybinds.all = defaultKeybinds;
        keybinds.Initialize(false);// keep any existing settings


        gamePreferences = this.gameObject.AddComponent<Data>();
        string[] projectPath = Application.dataPath.Split('/');
        string projectName = projectPath[projectPath.Length - 2];
        gamePreferences.fileLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/Infinadeck/Games/" + projectName + "/";
        gamePreferences.fileName = "gameSettings.ini";
        defaultGamePreferences = new Dictionary<string, Data.DataEntry>
        {
            // 800: General Game Preferences
            { "gameOverrideX", new Data.DataEntry { EntryName = "800 General Game Preferences", EntryValue = "0.0000" } },
            { "gameOverrideY", new Data.DataEntry { EntryName = "800 General Game Preferences", EntryValue = "0.0000" } },
            { "gameOverrideZ", new Data.DataEntry { EntryName = "800 General Game Preferences", EntryValue = "0.0000" } }
        };
        gamePreferences.all = defaultGamePreferences;
        gamePreferences.Initialize(false); // keep any existing settings

        textBG = Resources.Load("Textures/Inf_blackBG") as Texture2D;

        InputCheck();

        if (preferences.ReadBool("crashCheck"))
        {
            Debug.Log("INFINADECK NOTICE: Prior project crashed or closed without calling OnApplicationQuit on InfinadeckCore; starting in Safe Mode. Press Ctrl+I to enable the Infinadeck Plugin.");
            preferences.Write("pluginEnabled", "false");
            interpreter.enabled = false;
        }

        else
        {
            preferences.Write("crashCheck", "true");
            if (autoStart) { Boot(); }
            else { Debug.Log("INFINADECK NOTICE: 'Auto Start' is disabled. Please start Infinadeck Plugin by calling the Boot() function on the instance of [Infinadeck] in your Scene."); }
            interpreter.enabled = true;
        }
    }

    /**
     * Core Boot function. Only manually call to initialize the setup when AutoStart is disabled, or to re-enable after calling Shutdown.
     */
    public void Boot()
    {
        if (!booted)
        {
            booted = true;
            StartCoroutine(InitializeWithErrorChecks());
            StartCoroutine(SpawnSubcomponents());
        }
        else Debug.LogWarning("INFINADECK WARNING: Infinadeck Plugin is already booted; if manual boot is desired, un-check 'Auto Start' on the instance of [Infinadeck] in your Scene.");
    }

    /**
     * Core Shutdown function. Only manually call to stop existing threads- removes treadmill code entirely.
     */
    public void Shutdown()
    {
        if (booted)
        {
            booted = false;
            foreach (Transform child in this.transform)
            {
                Destroy(child.gameObject);
            }
        }
        else Debug.LogWarning("INFINADECK WARNING: Infinadeck Plugin is not currently booted; shutdown request ignored.");
    }

    private void OnApplicationQuit()
    {
        preferences.Write("crashCheck", "false");
        preferences.SaveSettings();
    }

    /**
     * Initialization of the Infinadeck plugin, parenting it to the appropriate components, along with error checking.
     */
    private IEnumerator InitializeWithErrorChecks()
    {
        if (!cameraRig)
        {
            Debug.LogWarning("INFINADECK WARNING: No CameraRig Reference Assigned, Assuming Parented to CameraRig");
            if (this.transform.parent == null)
            {
                Debug.LogWarning("INFINADECK WARNING: No CameraRig Reference Assigned and No Parent, Self is CameraRig");
                cameraRig = this.gameObject;
            }
            else { cameraRig = this.transform.parent.gameObject; }
        }
        else
        {
            this.transform.parent = cameraRig.transform;
        }
        this.transform.localPosition = originOffsetPosition;
        this.transform.localRotation = Quaternion.Euler(originOffsetRotation);
        this.transform.localScale = originOffsetScale;

        if (!headset)
        {
            Debug.LogWarning("INFINADECK WARNING: No Headset Reference Assigned, Assuming Main Camera is Correct");
            headset = Camera.main.gameObject;
        }
        initialized = true;
        yield return null;
    }

    /**
     * Spawn the individual elements of the Infinadeck plugin, based on the needs of the current scene.
     */
    private IEnumerator SpawnSubcomponents()
    {
        while (!initialized) { yield return new WaitForSeconds(1f); }
        if (firstLevel) // Only spawn the following if actually needed this level
        {
            //Spawn Splashscreen
            splashScreen = Instantiate(Resources.Load("RuntimePrefabs/InfinadeckSplashscreen") as GameObject, transform.position, Quaternion.identity);
            splashScreen.transform.parent = this.transform;
            splashScreenScript = splashScreen.GetComponent<Splashscreen>();
            splashScreenScript.headset = headset;
            splashScreenScript.referenceRig = this.gameObject;
            splashScreenScript.pluginVersion.text = "Plugin Version: " + pluginVersion;
            splashScreenScript.interpreter = interpreter;
        }

        // Spawn Reference Objects
        refObjects = Instantiate(Resources.Load("RuntimePrefabs/InfinadeckReferenceObjects") as GameObject, transform.position, Quaternion.identity);
        refObjects.transform.parent = this.transform;
        refObjectsScript = refObjects.GetComponent<ReferenceObjects>();
        refObjectsScript.referenceRig = this.gameObject;
        refObjectsScript.preferences = preferences;
        refObjectsScript.gamePreferences = gamePreferences;
        refObjectsScript.interpreter = interpreter;

        if (movementLevel) // Only spawn the following if actually needed this level
        {
            // Spawn Locomotion
            locomotion = Instantiate(Resources.Load("RuntimePrefabs/InfinadeckLocomotion") as GameObject, transform.position, Quaternion.identity);
            locomotion.transform.parent = this.transform;
            locomotionScript = locomotion.GetComponent<Locomotion>();
            locomotionScript.cameraRig = cameraRig;
            locomotionScript.referenceRig = this.gameObject;
            locomotionScript.speedGain = speedGain;
            locomotionScript.refObjects = refObjects.GetComponent<ReferenceObjects>();
            locomotionScript.showCollisions = showCollisions;
            locomotionScript.showTreadmillVelocity = showTreadmillVelocity;
            locomotionScript.interpreter = interpreter;
        }

        // Spawn Demo
        demo = Instantiate(Resources.Load("RuntimePrefabs/InfinaDEMO") as GameObject, transform.position, Quaternion.identity);
        demo.transform.parent = this.transform;
        demoScript = demo.GetComponent<Demo>();
        demoScript.referenceRig = this.gameObject;
        demoScript.interpreter = interpreter;
        demoScript.preferences = preferences;
    }

    /**
     * Runs whenever the object is enabled.
     */
    void OnEnable()
    {
        SceneManager.sceneUnloaded += LevelChange;
    }

    /**
     * Runs whenever the object is disabled.
     */
    void OnDisable()
    {
        SceneManager.sceneUnloaded -= LevelChange;
    }

    /**
     * Runs when the level is changing or reloading.
     */
    private void LevelChange(Scene scene)
    {
        if (guaranteeDestroyOnLoad) { Destroy(this.gameObject); }
    }

    private void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb != null && kb.iKey.wasPressedThisFrame && (kb.leftCtrlKey.isPressed || kb.rightCtrlKey.isPressed))
        {
            EnableOrDisable();
        }
        else if (kb != null && kb.escapeKey.wasPressedThisFrame) // Exits the game.
        {
            Application.Quit();
        }
        else if (kb != null && kb.equalsKey.wasPressedThisFrame) // Hides notifications.
        {
            if (hideActive) { preferences.Write("hideNotifications", "false"); }
            else { preferences.Write("hideNotifications", "true"); }
        }

        else
        {
            pluginActive = preferences.ReadBool("pluginEnabled");
            keybindsActive = preferences.ReadBool("keyboardInputEnabled");
            demoActive = preferences.ReadBool("demoMode");
            hideActive = preferences.ReadBool("hideNotifications");

            if (!pluginActive)
            {
                if (refObjects && refObjects.activeSelf) { refObjects.SetActive(false); }
                if (locomotion && locomotion.activeSelf) { locomotion.SetActive(false); }
                if (demo && demo.activeSelf) { demo.SetActive(false); }
            }
            else
            {
                interpreter.enabled = true;
                if (refObjects && !refObjects.activeSelf) { refObjects.SetActive(true); }
                if (locomotion && !locomotion.activeSelf) { locomotion.SetActive(true); }
                if (demo)
                {
                    if (!demo.activeSelf && demoActive) { demo.SetActive(true); }
                    else if (demo.activeSelf && !demoActive) { demo.SetActive(false); }
                }
                
                if (kb != null && kb.anyKey.wasPressedThisFrame)
                {
                    InputCheck();
                }
            }
        }
    }

    public void InputCheck()
    {
        if (IsActionPressed("ReloadCurrentLevel", "901- Treadmill", "LeftShift"))
        {
            if (guaranteeDestroyOnLoad) {
                preferences.Write("crashCheck", "false");
                preferences.SaveSettings();
            }  
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (IsActionPressed("StopTreadmill", "901- Treadmill", "Space"))
        {
            interpreter.StopTreadmill();
        }
        if (IsActionPressed("StartTreadmill", "901- Treadmill", "RightShift"))
        {
            interpreter.StartTreadmillUserControl();
        }
        if (IsActionPressed("ImportPreferences", "901- Treadmill", "Backslash"))
        {
            ImportPreferences();
        }
        if (IsActionPressed("ResetPreferences", "901- Treadmill", "Backspace"))
        {
            ResetPreferences();
        }

        if (IsActionPressed("ToggleDeckRing", "902- Reference Objects", "Q"))
        {
            if (refObjectsScript) refObjectsScript.ToggleDeckRing();
        }
        if (IsActionPressed("ToggleDeckEdge", "902- Reference Objects", "W"))
        {
            if (refObjectsScript) refObjectsScript.ToggleDeckEdge();
        }
        if (IsActionPressed("ToggleDeckCenter", "902- Reference Objects", "E"))
        {
            if (refObjectsScript) refObjectsScript.ToggleDeckCenter();
        }
        if (IsActionPressed("ToggleReferencePanel", "902- Reference Objects", "R"))
        {
            if (refObjectsScript) refObjectsScript.ToggleReferencePanel();
        }
        if (IsActionPressed("ToggleInEngineDeck", "902- Reference Objects", "T"))
        {
            if (refObjectsScript) refObjectsScript.ToggleInEngineDeck();
        }
        if (IsActionPressed("ToggleHeading", "902- Reference Objects", "Y"))
        {
            if (refObjectsScript) refObjectsScript.ToggleHeading();
        }
        if (IsActionPressed("ToggleColorblind", "902- Reference Objects", "U"))
        {
            if (refObjectsScript) refObjectsScript.ToggleColorblind();
        }
        if (IsActionPressed("CyclePanelTheme", "902- Reference Objects", "I"))
        {
            if (refObjectsScript) refObjectsScript.CyclePanelTheme();
        }
        if (IsActionPressed("CycleDeckCenter", "902- Reference Objects", "O"))
        {
            if (refObjectsScript) refObjectsScript.CycleDeckCenter();
        }

        if (IsActionPressed("SetTimer1Minute", "903- Demo", "Digit1"))
        {
            if (demoScript) demoScript.SetTheTimer(60);
        }
        if (IsActionPressed("SetTimer2Minute", "903- Demo", "Digit2"))
        {
            if (demoScript) demoScript.SetTheTimer(120);
        }
        if (IsActionPressed("SetTimer3Minute", "903- Demo", "Digit3"))
        {
            if (demoScript) demoScript.SetTheTimer(180);
        }
        if (IsActionPressed("SetTimer4Minute", "903- Demo", "Digit4"))
        {
            if (demoScript) demoScript.SetTheTimer(240);
        }
        if (IsActionPressed("SetTimer5Minute", "903- Demo", "Digit5"))
        {
            if (demoScript) demoScript.SetTheTimer(300);
        }
        if (IsActionPressed("SetTimer6Minute", "903- Demo", "Digit6"))
        {
            if (demoScript) demoScript.SetTheTimer(360);
        }
        if (IsActionPressed("SetTimer7Minute", "903- Demo", "Digit7"))
        {
            if (demoScript) demoScript.SetTheTimer(420);
        }
        if (IsActionPressed("SetTimer8Minute", "903- Demo", "Digit8"))
        {
            if (demoScript) demoScript.SetTheTimer(480);
        }
        if (IsActionPressed("SetTimer9Minute", "903- Demo", "Digit9"))
        {
            if (demoScript) demoScript.SetTheTimer(540);
        }
        if (IsActionPressed("SetTimer10Minute", "903- Demo", "Digit0"))
        {
            if (demoScript) demoScript.SetTheTimer(600);
        }
        if (IsActionPressed("ToggleDemoMode", "903- Demo", "Minus"))
        {
            if (demoScript) demoScript.ToggleDemoMode();
        }
    }

    public bool IsActionPressed(string funcName, string funcGroup, string defaultKey)
    {
        if (!keybinds.all.ContainsKey(funcName)) // if the function is not one we are already watching for:
        {
            // set the default values in keybinds.all
            keybinds.all.Add(funcName, new Data.DataEntry { EntryName = funcGroup, EntryValue = defaultKey, WriteFlag = true });
            // tell keybinds to reimport, now that it knows about this keybind, to grab the user preferred keybind if it exists
            keybinds.LoadSettings();
        }
        if (Keyboard.current == null) return false;
        Key key = ParseKey(keybinds.ReadString(funcName));
        if (key == Key.None) return false;
        return Keyboard.current[key].wasPressedThisFrame;
    }

    /// <summary>
    /// Converts a key name from keybinds.ini into an Input System <see cref="Key"/>.
    /// Accepts current Input System names (e.g. "Digit1", "Numpad1", "Enter") and also maps
    /// legacy UnityEngine.KeyCode names (e.g. "Alpha1", "Keypad1", "Return", "BackQuote")
    /// for backward compatibility with older config files.
    /// </summary>
    public Key ParseKey(string keyName)
    {
        if (string.IsNullOrEmpty(keyName)) return Key.None;
        if (Enum.TryParse(keyName, true, out Key key)) return key;

        // Legacy UnityEngine.KeyCode -> Input System Key fallbacks
        switch (keyName)
        {
            case "Alpha0": return Key.Digit0;
            case "Alpha1": return Key.Digit1;
            case "Alpha2": return Key.Digit2;
            case "Alpha3": return Key.Digit3;
            case "Alpha4": return Key.Digit4;
            case "Alpha5": return Key.Digit5;
            case "Alpha6": return Key.Digit6;
            case "Alpha7": return Key.Digit7;
            case "Alpha8": return Key.Digit8;
            case "Alpha9": return Key.Digit9;
            case "Keypad0": return Key.Numpad0;
            case "Keypad1": return Key.Numpad1;
            case "Keypad2": return Key.Numpad2;
            case "Keypad3": return Key.Numpad3;
            case "Keypad4": return Key.Numpad4;
            case "Keypad5": return Key.Numpad5;
            case "Keypad6": return Key.Numpad6;
            case "Keypad7": return Key.Numpad7;
            case "Keypad8": return Key.Numpad8;
            case "Keypad9": return Key.Numpad9;
            case "KeypadDivide": return Key.NumpadDivide;
            case "KeypadMultiply": return Key.NumpadMultiply;
            case "KeypadMinus": return Key.NumpadMinus;
            case "KeypadPlus": return Key.NumpadPlus;
            case "KeypadEnter": return Key.NumpadEnter;
            case "KeypadPeriod": return Key.NumpadPeriod;
            case "Return": return Key.Enter;
            case "BackQuote": return Key.Backquote;
            case "LeftControl": return Key.LeftCtrl;
            case "RightControl": return Key.RightCtrl;
            case "Tilde": return Key.Backquote;
            default:
                Debug.LogWarning("INFINADECK WARNING: Unrecognized key name '" + keyName + "' in keybinds.ini; ignoring.");
                return Key.None;
        }
    }

    /**
     * Enables the plugin if not running; Disables it if running. The only user accessible way to toggle the plugin, shy of modifying settings.ini
     */
    public void EnableOrDisable()
    {
        if (preferences.ReadBool("pluginEnabled")) { preferences.Write("pluginEnabled", "false"); }
        else { preferences.Write("pluginEnabled", "true"); if (!booted) Boot(); }
    }

    /**
     * Imports the preferences from the settings file.
     */
    public void ImportPreferences()
    {
        preferences.LoadSettings();
        keybinds.LoadSettings();
        gamePreferences.LoadSettings();
    }

    /**
     * Resets the settings file to the default preferences.
     */
    public void ResetPreferences()
    {
        preferences.all = defaultPreferences;
        foreach (KeyValuePair<string, Data.DataEntry> pref in preferences.all)
        {
            pref.Value.WriteFlag = true;
        }
        keybinds.all = defaultKeybinds;
        foreach (KeyValuePair<string, Data.DataEntry> pref in keybinds.all)
        {
            pref.Value.WriteFlag = true;
        }
        gamePreferences.all = defaultGamePreferences;
        foreach (KeyValuePair<string, Data.DataEntry> pref in gamePreferences.all)
        {
            pref.Value.WriteFlag = true;
        }
    }

    /**
     * Graphical elements for usage clarity.
     */
    void OnGUI()
    {
        if (!hideActive)
        {
            if (interpreter.Connected)
            {
                if (pluginActive) // IDA present, Plugin is Enabled
                {
                    if (keybindsActive)
                    {
                        if (keybinds.lastSync != _keybindsSyncedAt)
                        {
                            _keybindsSyncedAt = keybinds.lastSync;
                            string tread = "", refobj = "", demo = "";
                            foreach (KeyValuePair<string, Data.DataEntry> pref in keybinds.all)
                            {
                                if (pref.Value.EntryName == "901- Treadmill") tread += pref.Value.EntryValue + " to " + pref.Key + "\n";
                                else if (pref.Value.EntryName == "902- Reference Objects") refobj += pref.Value.EntryValue + " to " + pref.Key + "\n";
                                else if (pref.Value.EntryName == "903- Demo") demo += pref.Value.EntryValue + " to " + pref.Key + "\n";
                            }
                            _keybindsSectionStr = "Escape to QuitGame, Ctrl+I to TogglePlugin, = to HideGUI\n"
                                + "\n[Treadmill]\n" + tread
                                + "\n[Reference Objects]\n" + refobj
                                + "\n[Demo]\n" + demo
                                + "\n\nAll keybinds listed in\n"
                                + "My Documents/My Games/Infinadeck/Config/keybinds.ini\n"
                                + "\nContact us at <b>support@infinadeck.com</b> for more assistance\n";
                            _keybindsErrorInfo = null;
                        }
                        if (interpreter.errorInfo != _keybindsErrorInfo)
                        {
                            _keybindsErrorInfo = interpreter.errorInfo;
                            guiOutput = "<b>INFINADECK</b>   <color=red>" + _keybindsErrorInfo + "</color>\n" + _keybindsSectionStr;
                        }
                        GUI.DrawTexture(new Rect(Screen.width - 700, Screen.height - 700, 400, 1200), textBG, ScaleMode.StretchToFill, false, 0);
                        GUI.Label(new Rect(Screen.width - 685, Screen.height - 685, 400, 1200), guiOutput);
                    }
                    else
                    {
                        guiOutput = "<b>INFINADECK</b>" + "   <color=red>" + interpreter.errorInfo + "</color>\n"
                            + "No keybinds active\n"
                            + "\n"
                            + "Enable them by setting 'keyboardInputEnabled = true in'\n"
                            + "My Documents/My Games/Infinadeck/Config/settings.ini\n"
                            + "\n"
                            + "\n"
                            + "\n"
                            + "Contact us at <b>support@infinadeck.com</b> for more assistance\n"
                            + "\n"
                            + "Press = to Hide Infinadeck GUI Notifications\n";
                        GUI.DrawTexture(new Rect(Screen.width - 700, Screen.height - 500, 400, 200), textBG, ScaleMode.StretchToFill, false, 0);
                        GUI.Label(new Rect(Screen.width - 685, Screen.height - 485, 400, 200), guiOutput);
                    }
                }
                else// IDA present, Plugin is Disabled
                {
                    guiOutput = "<b>INFINADECK</b>" + "   <color=red>" + interpreter.errorInfo + "</color>\n"
                        + "Infinadeck Plugin disabled, but IDA is open\n"
                        + "\n"
                        + "Re-enable by pressing Ctrl+I\n"
                        + "or by setting 'pluginEnabled = true' in\n"
                        + "My Documents/My Games/Infinadeck/Config/settings.ini\n"
                        + "\n"
                        + "\n"
                        + "Contact us at <b>support@infinadeck.com</b> for more assistance\n"
                        + "\n"
                        + "Press = to Hide Infinadeck GUI Notifications\n";
                    GUI.DrawTexture(new Rect(Screen.width - 700, Screen.height - 500, 400, 200), textBG, ScaleMode.StretchToFill, false, 0);
                    GUI.Label(new Rect(Screen.width - 685, Screen.height - 485, 400, 200), guiOutput);
                }
            }
            else
            {
                if (pluginActive) // IDA not present, Plugin is Enabled
                {
                    guiOutput = "<b>INFINADECK</b>" + "   <color=red>" + interpreter.errorInfo + "</color>\n"
                        + "Infinadeck Plugin enabled, but IDA is not open\n"
                        + "\n"
                        + "Download IDA (Infinadeck Desktop Application)\n"
                        + "<b>https://tinyurl.com/Infinadeck</b>\n"
                        + "\n"
                        + "<color=grey>(OR Disable the plugin by pressing Ctrl+I)</color>\n"
                        + "\n"
                        + "Contact us at <b>support@infinadeck.com</b> for more assistance\n"
                        + "\n"
                        + "Press = to Hide Infinadeck GUI Notifications\n";
                    GUI.DrawTexture(new Rect(Screen.width - 700, Screen.height - 500, 400, 200), textBG, ScaleMode.StretchToFill, false, 0);
                    GUI.Label(new Rect(Screen.width - 685, Screen.height - 485, 400, 200), guiOutput);
                }
                else // IDA not present, Plugin is Disabled
                {
                    if (interpreter.enabled) { guiOutput = ""; }
                    else //Safe Mode Message
                    {
                        guiOutput = "<b>INFINADECK</b>" + "   <color=red>" + interpreter.errorInfo + "</color>\n"
                        + "SAFE MODE\n"
                        + "\n"
                        + "Prior project crashed or closed without\n"
                        + "calling OnApplicationQuit on InfinadeckCore\n"
                        + "\n"
                        + "\n"
                        + "Press Ctrl + I to enable the Infinadeck Plugin\n"
                        + "\n"
                        + "\n"
                        + "Press = to Hide Infinadeck GUI Notifications\n";
                        GUI.DrawTexture(new Rect(Screen.width - 700, Screen.height - 500, 400, 200), textBG, ScaleMode.StretchToFill, false, 0);
                        GUI.Label(new Rect(Screen.width - 685, Screen.height - 485, 400, 200), guiOutput);
                    }
                }
            }
        }
        else
        {
            if (interpreter.enabled) { guiOutput = ""; }
            else //Safe Mode Message even though messages hidden
            {
                guiOutput = "<b>INFINADECK</b>" + "   <color=red>" + interpreter.errorInfo + "</color>\n"
                + "SAFE MODE\n"
                + "\n"
                + "Prior project crashed or closed without\n"
                + "calling OnApplicationQuit on Infinadeck.Core\n"
                + "\n"
                + "\n"
                + "Press Ctrl + I to enable the Infinadeck Plugin\n"
                + "\n"
                + "\n"
                + "Press = to Hide Infinadeck GUI Notifications\n";
                GUI.DrawTexture(new Rect(Screen.width - 700, Screen.height - 500, 400, 200), textBG, ScaleMode.StretchToFill, false, 0);
                GUI.Label(new Rect(Screen.width - 685, Screen.height - 485, 400, 200), guiOutput);
            }
        }
    }
}
}