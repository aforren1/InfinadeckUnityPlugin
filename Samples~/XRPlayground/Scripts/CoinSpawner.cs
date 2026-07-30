using UnityEngine;

namespace Infinadeck.Samples.XRPlayground
{
    /**
     * Spawns a ring of collectable coins around the start position, at varied
     * distances and heights so some need reaching and some need walking.
     */
    public class CoinSpawner : MonoBehaviour
    {
        public Material coinMaterial;
        public int count = 12;
        public float innerRadius = 1.5f;
        public float outerRadius = 4f;

        private void Start()
        {
            for (int i = 0; i < count; i++)
            {
                float angle = i * (360f / count) * Mathf.Deg2Rad;
                float radius = Mathf.Lerp(innerRadius, outerRadius, (i % 3) / 2f);
                float height = Mathf.Lerp(0.7f, 1.4f, ((i + 1) % 4) / 3f);
                CreateCoin(new Vector3(Mathf.Cos(angle) * radius, height, Mathf.Sin(angle) * radius));
            }
            if (ScoreBoard.Instance) { ScoreBoard.Instance.SetTotal(count); }
        }

        private void CreateCoin(Vector3 position)
        {
            var coin = new GameObject("Coin");
            coin.transform.SetParent(transform, false);
            coin.transform.localPosition = position;

            var trigger = coin.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = 0.1f;
            coin.AddComponent<Coin>();

            var disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            disc.name = "Disc";
            Destroy(disc.GetComponent<Collider>());
            disc.transform.SetParent(coin.transform, false);
            disc.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            disc.transform.localScale = new Vector3(0.15f, 0.008f, 0.15f);
            if (coinMaterial) { disc.GetComponent<MeshRenderer>().sharedMaterial = coinMaterial; }
        }
    }
}
