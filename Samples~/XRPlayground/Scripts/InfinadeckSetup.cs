using Unity.XR.CoreUtils;
using UnityEngine;

namespace Infinadeck.Samples.XRPlayground
{
    /**
     * Connects the [Infinadeck] prefab to the XR Interaction Toolkit rig.
     *
     * The prefab starts inactive in the scene so its references can be
     * assigned here before Core.Awake runs. Once activated, treadmill motion
     * moves the XR Origin exactly like the thumbstick does, and the reference
     * objects (ring, center mark, deck) render inside the rig's play space.
     *
     * The native Infinadeck API is Windows only (which matches the PCVR
     * target); on any other platform the prefab stays inactive and the rest
     * of the sample works as usual.
     */
    public class InfinadeckSetup : MonoBehaviour
    {
        private void Start()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            var core = FindFirstObjectByType<Core>(FindObjectsInactive.Include);
            var origin = FindFirstObjectByType<XROrigin>();
            if (!core || !origin)
            {
                Debug.LogWarning("XR PLAYGROUND: [Infinadeck] prefab or XROrigin missing, treadmill locomotion disabled.");
                return;
            }
            core.cameraRig = origin.gameObject;
            core.headset = origin.Camera.gameObject;
            core.gameObject.SetActive(true);
#else
            Debug.Log("XR PLAYGROUND: Infinadeck plugin is Windows only; leaving [Infinadeck] inactive on this platform.");
#endif
        }
    }
}
