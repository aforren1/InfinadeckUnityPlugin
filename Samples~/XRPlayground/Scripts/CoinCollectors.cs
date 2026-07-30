using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
#if XR_HANDS_PRESENT
using UnityEngine.XR.Hands;
#endif

namespace Infinadeck.Samples.XRPlayground
{
    /**
     * Creates invisible trigger spheres that follow the controllers and the
     * palms, so coins can be collected by touch whether the player is holding
     * controllers or using hand tracking. Kept independent of the rig's
     * interactor setup so it works with any XR Origin.
     */
    public class CoinCollectors : MonoBehaviour
    {
        public float radius = 0.07f;

        private Transform trackingSpace;
        private readonly List<(InputAction action, Transform follower)> controllers = new();

        private void Start()
        {
            var origin = FindFirstObjectByType<XROrigin>();
            if (!origin)
            {
                Debug.LogWarning("XR PLAYGROUND: No XROrigin found, coins cannot be collected.");
                enabled = false;
                return;
            }
            // Device and hand poses are reported relative to this transform.
            trackingSpace = origin.CameraFloorOffsetObject.transform;

            CreateControllerCollector("<XRController>{LeftHand}/devicePosition", "Left Controller Collector");
            CreateControllerCollector("<XRController>{RightHand}/devicePosition", "Right Controller Collector");
        }

        private void OnDestroy()
        {
            foreach (var (action, _) in controllers) { action.Dispose(); }
        }

        private void Update()
        {
            foreach (var (action, follower) in controllers)
            {
                // No resolved controls means the controller is absent, e.g.
                // set down while hand tracking is active.
                bool tracked = action.controls.Count > 0;
                follower.gameObject.SetActive(tracked);
                if (tracked) { follower.localPosition = action.ReadValue<Vector3>(); }
            }
            UpdateHandCollectors();
        }

        private void CreateControllerCollector(string binding, string name)
        {
            var action = new InputAction(binding: binding, expectedControlType: "Vector3");
            action.Enable();
            controllers.Add((action, CreateFollower(name)));
        }

        private Transform CreateFollower(string name)
        {
            var follower = new GameObject(name);
            follower.transform.SetParent(trackingSpace, false);
            var trigger = follower.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = radius;
            var rb = follower.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            follower.AddComponent<CoinCollector>();
            return follower.transform;
        }

#if XR_HANDS_PRESENT
        private XRHandSubsystem hands;
        private Transform leftPalm;
        private Transform rightPalm;

        private void UpdateHandCollectors()
        {
            if (hands == null)
            {
                var found = new List<XRHandSubsystem>();
                SubsystemManager.GetSubsystems(found);
                if (found.Count == 0) { return; }
                hands = found[0];
                leftPalm = CreateFollower("Left Palm Collector");
                rightPalm = CreateFollower("Right Palm Collector");
            }

            UpdatePalm(hands.leftHand, leftPalm);
            UpdatePalm(hands.rightHand, rightPalm);
        }

        private static void UpdatePalm(XRHand hand, Transform follower)
        {
            if (!hand.isTracked || !hand.GetJoint(XRHandJointID.Palm).TryGetPose(out Pose pose))
            {
                follower.gameObject.SetActive(false);
                return;
            }
            follower.gameObject.SetActive(true);
            follower.localPosition = pose.position;
        }
#else
        private void UpdateHandCollectors() { }
#endif
    }
}
