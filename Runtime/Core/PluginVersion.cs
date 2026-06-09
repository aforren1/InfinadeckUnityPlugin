using UnityEngine;
using UnityEngine.UI;

/**
 * ------------------------------------------------------------
 * Visually updates the Infinadeck plugin to display the correct version in-editor.
 * https://github.com/Infinadeck/InfinadeckUnityPlugin
 * Created by Griffin Brunner @ Infinadeck, 2019-2022
 * Attribution required.
 * ------------------------------------------------------------
 */

namespace Infinadeck
{
    [ExecuteAlways]
    public class PluginVersion : MonoBehaviour
    {
        public Text version;
        private Core core;
        private double nextSearch;

        void Update()
        {
            if (!core)
            {
                if (Time.realtimeSinceStartup < nextSearch) return;
                nextSearch = Time.realtimeSinceStartup + 1.0;
                core = FindFirstObjectByType<Core>();
            }
            else
            {
                version.text = core.pluginVersionForEditorReference;
            }
        }
    }
}
