using UnityEngine;

/**
 * ------------------------------------------------------------
 * Script to modify the position of the Infinadeck Deck Heading.
 * https://github.com/Infinadeck/InfinadeckUnityPlugin
 * Created by Griffin Brunner @ Infinadeck, 2019-2022
 * Attribution required.
 * ------------------------------------------------------------
 */

namespace Infinadeck
{
    public class DeckHeading : MonoBehaviour
    {
        public float xin;
        public float yin;
        public float angle;
        public float mag;
        public float threshold;
        public float boxsize;
        public Locomotion motion;
        public float elevation;

        Locomotion CheckForLocomotionInScene()
        {
            motion = FindFirstObjectByType<Locomotion>();
            return motion;
        }

        // Update is called once per frame
        void Update()
        {
            if (motion || CheckForLocomotionInScene())
            {
                xin = motion.xDistance;
                yin = motion.yDistance;
            }
            else
            {
                xin = 0;
                yin = 0;
            }
            mag = Mathf.Sqrt(xin * xin + yin * yin);
            if (mag > threshold)
            {
                this.GetComponent<Renderer>().enabled = true;
                if (Mathf.Abs(xin) > Mathf.Abs(yin))
                {
                    yin *= boxsize / Mathf.Abs(xin);
                    xin *= boxsize / Mathf.Abs(xin);
                }
                else
                {
                    xin *= boxsize / Mathf.Abs(yin);
                    yin *= boxsize / Mathf.Abs(yin);
                }
                transform.localPosition = new Vector3(xin, elevation, yin);
            }
            else
            {
                this.GetComponent<Renderer>().enabled = false;
            }
        }
    }
}
