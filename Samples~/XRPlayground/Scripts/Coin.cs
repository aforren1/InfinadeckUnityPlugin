using UnityEngine;

namespace Infinadeck.Samples.XRPlayground
{
    /**
     * A spinning, bobbing collectable. Touch it with a hand or controller to
     * score a point.
     */
    public class Coin : MonoBehaviour
    {
        public float spinSpeed = 120f;
        public float bobHeight = 0.04f;
        public float bobSpeed = 2f;

        private Vector3 restPosition;

        private void Start()
        {
            restPosition = transform.position;
        }

        private void Update()
        {
            transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);
            transform.position = restPosition + Vector3.up * (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.GetComponentInParent<CoinCollector>()) { return; }
            if (ScoreBoard.Instance) { ScoreBoard.Instance.AddCoin(); }
            Destroy(gameObject);
        }
    }
}
