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

        // Update is called once per frame
        void Update()
        {
            if (!core) { core = FindFirstObjectByType<Core>(); }
            else { version.text = core.pluginVersionForEditorReference; }
        }
    }
}
