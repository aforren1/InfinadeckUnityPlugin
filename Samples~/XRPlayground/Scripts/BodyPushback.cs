using Unity.XR.CoreUtils;
using UnityEngine;

namespace Infinadeck.Samples.XRPlayground
{
    /**
     * Adds body collision on top of the XR Interaction Toolkit rig.
     *
     * XRI blocks stick locomotion with the rig's CharacterController, but
     * physically walking or leaning still lets the camera pass through
     * geometry. This keeps the capsule under the headset every frame and
     * nudges the controller so its overlap recovery pushes the whole rig
     * back out of walls.
     */
    public class BodyPushback : MonoBehaviour
    {
        public float minHeight = 0.5f;
        public float maxHeight = 2.5f;

        private CharacterController body;
        private Transform head;
        private Transform rig;

        private void Start()
        {
            var origin = FindFirstObjectByType<XROrigin>();
            body = origin ? origin.GetComponent<CharacterController>() : null;
            if (!body || !origin.Camera)
            {
                Debug.LogWarning("XR PLAYGROUND: No XROrigin with a CharacterController found, body collision disabled.");
                enabled = false;
                return;
            }
            head = origin.Camera.transform;
            rig = origin.transform;
        }

        private void LateUpdate()
        {
            // Head position in rig space, the same space the capsule center uses.
            Vector3 headLocal = rig.InverseTransformPoint(head.position);
            float height = Mathf.Clamp(headLocal.y + 0.1f, minHeight, maxHeight);
            body.height = height;
            Vector3 center = headLocal;
            center.y = height / 2f + body.skinWidth;
            body.center = center;

            // A tiny move is enough to trigger overlap recovery. The rig's own
            // locomotion still handles gravity and stick movement.
            body.Move(Vector3.down * 0.001f);
        }
    }
}
