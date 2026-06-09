using UnityEngine;

/**
 * ------------------------------------------------------------
 * Script to translate Infinadeck motion into game motion.
 * https://github.com/Infinadeck/InfinadeckUnityPlugin
 * Created by George Burger & Griffin Brunner @ Infinadeck, 2019-2022
 * Attribution required.
 * ------------------------------------------------------------
 */

namespace Infinadeck
{
    public class Locomotion : MonoBehaviour
    {
        public GameObject referenceRig;
        public GameObject cameraRig;

        [ReadOnlyInEditor] public float xDistance;
        [ReadOnlyInEditor] public float yDistance;

        private float fixAngle;
        private float calcX;
        private float calcY;
        public float speedGain = 1;
        public ReferenceObjects refObjects;
        public bool showCollisions = false;
        private Vector3 targetPosition = Vector3.zero;
        private Vector3 previousFramePosition = Vector3.zero;
        public bool showTreadmillVelocity = false;
        public Interpreter interpreter;

        public bool testShake = false;
        public float testShakeStrength = .005f;

        private void Start()
        {
            if (!referenceRig)
            {
                Debug.LogWarning("INFINADECK WARNING: No Locomotion ReferenceRig Assigned, Assuming Parent");
                if (this.transform.parent == null)
                {
                    Debug.LogWarning("INFINADECK WARNING: No Locomotion Parent, Assuming CameraRig");
                    if (cameraRig == null)
                    {
                        Debug.LogWarning("INFINADECK WARNING: No Locomotion CameraRig, Assuming Self");
                        referenceRig = this.gameObject;
                    }
                    else { referenceRig = cameraRig; }
                }
                else { referenceRig = this.transform.parent.gameObject; }
            }
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;
            this.transform.localScale = Vector3.one;
        }

        /**
         * Runs once per frame update.
         */
        void Update()
        {
            if (interpreter.Connected) // only run if there is a successful connection
            {
                if (!referenceRig) { return; }
                if (!cameraRig) { return; }

                if (Vector3.Distance(cameraRig.transform.position, previousFramePosition) > 0.001f)
                {
                    if (showCollisions) { Debug.Log("Infinadeck Pre - Locomotion Check: User moved since last frame.Collision has occurred."); }
                }

                // Import speeds from Infinadeck

                if (showTreadmillVelocity) { Debug.Log(interpreter.FloorSpeeds.v0 + " " + interpreter.FloorSpeeds.v1); }

                // Distance = speed * time between samples
                calcX = (float)interpreter.FloorSpeeds.v0 * (Time.deltaTime);
                calcY = (float)interpreter.FloorSpeeds.v1 * (Time.deltaTime);
                // Convert for any weird world rotation or scale
                fixAngle = this.transform.eulerAngles.y * Mathf.Deg2Rad;
                xDistance = (calcX * Mathf.Cos(fixAngle) + calcY * Mathf.Sin(fixAngle)) * referenceRig.transform.lossyScale.x * speedGain;
                yDistance = (-calcX * Mathf.Sin(fixAngle) + calcY * Mathf.Cos(fixAngle)) * referenceRig.transform.lossyScale.z * speedGain;

                targetPosition = cameraRig.transform.position + new Vector3(xDistance, 0, yDistance);

                if (testShake) { targetPosition += new Vector3(Random.Range(-testShakeStrength, testShakeStrength), 0, Random.Range(-testShakeStrength, testShakeStrength)); }

                // Move user based on treadmill motion as long as the deck is not calibrating
                cameraRig.transform.position = targetPosition;

                if (Vector3.Distance(cameraRig.transform.position, targetPosition) > 0.001f)
                {
                    if (showCollisions) { Debug.Log("Infinadeck Post-Locomotion Check: User not moved to target location. Collision has occurred."); }
                }

                previousFramePosition = cameraRig.transform.position;

                if (refObjects) { refObjects.currentTreadmillSpeed = Vector3.Magnitude(new Vector3((float)interpreter.FloorSpeeds.v0, (float)interpreter.FloorSpeeds.v1, 0)); }
            }
        }
    }
}
