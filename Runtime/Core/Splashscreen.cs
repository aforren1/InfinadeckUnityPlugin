using UnityEngine;
using UnityEngine.UI;

/**
 * ------------------------------------------------------------
 * Script to control the existence of the Infinadeck Splashscreen.
 * https://github.com/Infinadeck/InfinadeckUnityPlugin
 * Created by Griffin Brunner @ Infinadeck, 2019-2022
 * Attribution required.
 * ------------------------------------------------------------
 */

namespace Infinadeck
{
    public class Splashscreen : MonoBehaviour
    {
        public Interpreter interpreter;
        public Text deckSN;
        public Text modelNumber;
        public Text APIVersion;
        public Text pluginVersion;
        public GameObject headset;

        public GameObject referenceRig;

        /**
         * Runs once on the object's first frame.
         */
        private void Start()
        {
            if (!referenceRig)
            {
                Debug.LogWarning("INFINADECK WARNING: No Splashscreen ReferenceRig Assigned, Assuming Parent");
                if (this.transform.parent == null)
                {
                    Debug.LogWarning("INFINADECK WARNING: No Splashscreen Parent, Assuming Self");
                    referenceRig = this.gameObject;
                }
                else { referenceRig = this.transform.parent.gameObject; }
            }
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;
            this.transform.localScale = Vector3.one;

            // position the splashscreen such that it's visible.
            this.transform.eulerAngles = new Vector3(0, headset.transform.eulerAngles.y, 0);
            this.transform.position = new Vector3(referenceRig.transform.position.x, headset.transform.position.y, referenceRig.transform.position.z);
            Destroy(this.gameObject, 3.0f);
        }

        private void Update()
        {
            // update the splashscreen with relevant info, pull from deck itself
            if (interpreter.TreadmillInfoID != null) { deckSN.text = "Deck SN: " + interpreter.TreadmillInfoID; }
            if (interpreter.TreadmillInfoModelNumber != null) { modelNumber.text = "Model Number: " + interpreter.TreadmillInfoModelNumber; }
            if (interpreter.TreadmillInfoDllVersion != null) { APIVersion.text = "API Version: " + interpreter.TreadmillInfoDllVersion; }
        }
    }
}
