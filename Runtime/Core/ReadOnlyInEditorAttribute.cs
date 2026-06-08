using UnityEngine;

/**
 * ------------------------------------------------------------
 * Read-only parameter for Infinadeck plugin components in the Unity Editor.
 * https://github.com/Infinadeck/InfinadeckUnityPlugin
 * Created by Griffin Brunner @ Infinadeck, 2019-2022
 * Attribution required.
 * ------------------------------------------------------------
 */

namespace Infinadeck
{
    /// <summary>
    /// Apply <c>[ReadOnlyInEditor]</c> to a serialized field to display it greyed-out
    /// (non-editable) in the Inspector. Drawn by <c>Infinadeck.ReadOnlyInEditorDrawer</c>.
    /// </summary>
    public class ReadOnlyInEditorAttribute : PropertyAttribute
    {
    }
}
