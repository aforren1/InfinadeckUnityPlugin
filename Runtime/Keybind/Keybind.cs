using System;
using UnityEngine;
using UnityEngine.InputSystem;

#if !ENABLE_INPUT_SYSTEM
#error Infinadeck requires the Input System package. In Player Settings, set "Active Input Handling" to "Input System Package" (or "Both").
#endif

namespace Infinadeck
{
    /// <summary>
    /// Optional helper for sampling keyboard "profiles" of twelve keys via the Input System.
    /// Migrated from the legacy UnityEngine.Input/KeyCode implementation; not used by the core
    /// plugin, but kept for projects that referenced it.
    /// </summary>
    public class Keybind : MonoBehaviour
    {
        public bool isInputEnabled;
        public bool checkYourKeys;
        public Key currentKey;
        public int bufferLength = 8;
        public Key[] keyBuffer;

        readonly string funcGroup = "FUNC";
        readonly Key[] funcKeys = new Key[] {
            Key.F1,
            Key.F2,
            Key.F3,
            Key.F4,
            Key.F5,
            Key.F6,
            Key.F7,
            Key.F8,
            Key.F9,
            Key.F10,
            Key.F11,
            Key.F12
        };

        readonly string alphGroup = "1234";
        readonly Key[] alphKeys = new Key[] {
            Key.Digit1,
            Key.Digit2,
            Key.Digit3,
            Key.Digit4,
            Key.Digit5,
            Key.Digit6,
            Key.Digit7,
            Key.Digit8,
            Key.Digit9,
            Key.Digit0,
            Key.Minus,
            Key.Equals
        };

        readonly string npadGroup = "#PAD";
        readonly Key[] npadKeys = new Key[] {
            Key.Numpad1,
            Key.Numpad2,
            Key.Numpad3,
            Key.Numpad4,
            Key.Numpad5,
            Key.Numpad6,
            Key.Numpad7,
            Key.Numpad8,
            Key.Numpad9,
            Key.NumpadDivide,
            Key.NumpadMultiply,
            Key.NumpadMinus
        };

        readonly string stndGroup = "STND";
        readonly Key[] stndKeys = new Key[] {
            Key.LeftShift,
            Key.LeftCtrl,
            Key.LeftAlt,
            Key.Space,
            Key.RightShift,
            Key.RightCtrl,
            Key.RightAlt,
            Key.Enter,
            Key.Backquote,
            Key.Tab,
            Key.Backslash,
            Key.Backspace
        };

        readonly string cpadGroup = "CPAD";
        readonly Key[] cpadKeys = new Key[] {
            Key.LeftArrow,
            Key.DownArrow,
            Key.RightArrow,
            Key.UpArrow,
            Key.Delete,
            Key.End,
            Key.PageDown,
            Key.Insert,
            Key.Home,
            Key.PageUp,
            Key.ScrollLock,
            Key.Pause
        };

        readonly string qwerGroup = "QWER";
        readonly Key[] qwerKeys = new Key[] {
            Key.Q,
            Key.W,
            Key.E,
            Key.R,
            Key.T,
            Key.Y,
            Key.U,
            Key.I,
            Key.O,
            Key.P,
            Key.LeftBracket,
            Key.RightBracket
        };

        readonly string asdfGroup = "ASDF";
        readonly Key[] asdfKeys = new Key[] {
            Key.A,
            Key.S,
            Key.D,
            Key.F,
            Key.G,
            Key.H,
            Key.J,
            Key.K,
            Key.L,
            Key.Semicolon,
            Key.Quote,
            Key.Slash
        };
        public Key[] customKeys = new Key[12];
        readonly string defaultCustomKeystring = "Digit1-Digit2-Digit3-Digit4-Digit5-Digit6-Digit7-Digit8-Digit9-Digit0-Minus-Equals";

        public Key[] GetMyKeys(string keybindProfile)
        {
            if (keybindProfile == funcGroup) { return funcKeys; }
            else if (keybindProfile == alphGroup) { return alphKeys; }
            else if (keybindProfile == npadGroup) { return npadKeys; }
            else if (keybindProfile == stndGroup) { return stndKeys; }
            else if (keybindProfile == cpadGroup) { return cpadKeys; }
            else if (keybindProfile == qwerGroup) { return qwerKeys; }
            else if (keybindProfile == asdfGroup) { return asdfKeys; }
            else // assume keybindProfile == "Custom"
            {
                string[] keyArray = keybindProfile.Split('-');
                if (keyArray.Length != 12)
                {
                    Debug.LogError("INFINAKEYBIND NOTIFICATION: customBinding not valid, using default (1234)");
                    keyArray = defaultCustomKeystring.Split('-');
                }
                for (int b = 0; b < 12; b++)
                {
                    if (!Enum.TryParse(keyArray[b], true, out customKeys[b]))
                    {
                        Debug.LogError("INFINAKEYBIND: invalid key name '" + keyArray[b] + "' in custom profile; using Key.None");
                        customKeys[b] = Key.None;
                    }
                }
                return customKeys;
            }
        }

        public int KeybindRequest(Key[] theKeys)
        {
            int output = 0;
            if (checkYourKeys)
            {
                for (int b = 0; b < 12; b++)
                {
                    if (CheckKeyBuffer(theKeys[b])) { output = b + 1; RemoveKeyFromBuffer(theKeys[b]); }
                }
            }
            if (CheckKeyBufferEmpty()) { checkYourKeys = false; }
            return output;
        }

        public bool KeyRequest(Key theKey)
        {
            bool output = false;
            if (checkYourKeys)
            {
                for (int b = 0; b < 12; b++)
                {
                    if (CheckKeyBuffer(theKey)) { output = true; RemoveKeyFromBuffer(theKey); }
                }
            }
            if (CheckKeyBufferEmpty()) { checkYourKeys = false; }
            return output;
        }

        private void Awake()
        {
            if ((bufferLength < 2) || (bufferLength > 64)) { bufferLength = 8; }
            keyBuffer = new Key[bufferLength];
            FlushKeyBuffer();
        }

        private void Update()
        {
            if (isInputEnabled) { InputCheck(); }
        }

        /**
         * Keybind Checking Loop.
         */
        public void InputCheck()
        {
            Keyboard kb = Keyboard.current;
            if (kb == null) { return; }
            if (kb.anyKey.wasPressedThisFrame)
            {
                checkYourKeys = true;
                foreach (var keyControl in kb.allKeys)
                {
                    if (keyControl.wasPressedThisFrame)
                    {
                        AddKeyToBuffer(keyControl.keyCode);
                    }
                }

                if (kb.escapeKey.wasPressedThisFrame) // Exits the game.
                {
                    Application.Quit();
                }
            }
        }

        public bool CheckKeyBuffer(Key key)
        {
            for (int b = 0; b < bufferLength; b++)
            {
                if (keyBuffer[b] == key) { return true; }
            }
            return false;
        }

        public bool CheckKeyBufferEmpty()
        {
            for (int b = 0; b < bufferLength; b++)
            {
                if (keyBuffer[b] != Key.None) { return false; }
            }
            return true;
        }

        public void FlushKeyBuffer()
        {
            for (int b = 0; b < bufferLength; b++)
            {
                keyBuffer[b] = Key.None;
            }
        }

        public void AddKeyToBuffer(Key key)
        {
            for (int b = bufferLength - 1; b >= 1; b--)
            {
                keyBuffer[b] = keyBuffer[b - 1];
            }
            keyBuffer[0] = key;
        }

        public void RemoveKeyFromBuffer(Key key)
        {
            for (int b = 0; b < bufferLength; b++)
            {
                if (keyBuffer[b] == key) { keyBuffer[b] = Key.None; }
            }
        }
    }
}
