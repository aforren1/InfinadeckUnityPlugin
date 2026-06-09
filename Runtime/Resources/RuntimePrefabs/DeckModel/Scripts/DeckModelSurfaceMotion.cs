using UnityEngine;

/**
 * ------------------------------------------------------------
 * Script to translate Infinadeck motion into game motion.
 * http://tinyurl.com/InfinadeckSDK
 * Created by George Burger & Griffin Brunner @ Infinadeck, 2019
 * Attribution required.
 * ------------------------------------------------------------
 */

namespace Infinadeck
{
    public class DeckModelSurfaceMotion : MonoBehaviour
    {
        public Material mat;
        public bool demo = false;
        public Transform anchor;
        public Vector3 anchorPoint;
        private Vector3 deviance;
        public Locomotion motion;
        private float speedToDeckSurface = 1.2192f;

        [ReadOnlyInEditor] public float xDistance;
        [ReadOnlyInEditor] public float yDistance;
        public Interpreter interpreter;

        /**
         * Runs once per frame update.
         */
        void Update()
        {
            if (!motion) { motion = FindFirstObjectByType<Locomotion>(); }
            else if (!interpreter) { interpreter = FindFirstObjectByType<Interpreter>(); }
            else
            {
                if (anchor) // only run if there is a successful connection
                {
                    deviance = anchor.position - anchorPoint;
                    mat.SetTextureOffset("_MainTex", new Vector2(-deviance.x / 1.2192f, -deviance.z / 1.2192f));
                }
                else
                {
                    if (demo)
                    {
                        xDistance += .01f * Mathf.Cos(Time.time);
                        yDistance += .01f * Mathf.Sin(Time.time);
                    }
                    else if (motion)
                    {
                        xDistance += (float)interpreter.FloorSpeeds.v0 * (Time.deltaTime);
                        yDistance += (float)interpreter.FloorSpeeds.v1 * (Time.deltaTime);
                    }

                    mat.SetTextureOffset("_MainTex", new Vector2(-xDistance / speedToDeckSurface, -yDistance / speedToDeckSurface));
                }
            }
        }
    }
}
