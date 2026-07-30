using UnityEngine;

namespace Infinadeck.Samples.XRPlayground
{
    /**
     * Shows the coin count on a TextMesh floating in the scene.
     */
    [RequireComponent(typeof(TextMesh))]
    public class ScoreBoard : MonoBehaviour
    {
        public static ScoreBoard Instance { get; private set; }

        private TextMesh text;
        private int collected;
        private int total;

        private void Awake()
        {
            Instance = this;
            text = GetComponent<TextMesh>();
            Refresh();
        }

        private void OnDestroy()
        {
            if (Instance == this) { Instance = null; }
        }

        public void SetTotal(int value)
        {
            total = value;
            Refresh();
        }

        public void AddCoin()
        {
            collected++;
            Refresh();
        }

        private void Refresh()
        {
            text.text = total > 0 && collected >= total
                ? "All " + total + " coins collected!"
                : "Coins: " + collected + " / " + total;
        }
    }
}
