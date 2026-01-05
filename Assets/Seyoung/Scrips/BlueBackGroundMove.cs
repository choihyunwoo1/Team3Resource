using UnityEngine;

namespace Team3
{
    public class BlueBackGroundMove : MonoBehaviour
    {
        public Vector2 amplitude = new Vector2(0.3f, 0.15f);
        public Vector2 speed = new Vector2(0.5f, 0.8f);

        private Vector3[] childStartPositions;
        private Transform[] children;
        private float seed;

        private void Start()
        {
            seed = Random.Range(0f, 100f);

            children = new Transform[transform.childCount];
            childStartPositions = new Vector3[transform.childCount];

            for (int i = 0; i < transform.childCount; i++)
            {
                children[i] = transform.GetChild(i);
                childStartPositions[i] = children[i].position; // 월드 기준
            }
        }

        private void Update()
        {
            float x = Mathf.Sin(Time.time * speed.x + seed) * amplitude.x;
            float y = Mathf.Sin(Time.time * speed.y + seed) * amplitude.y;

            Vector3 offset = new Vector3(x, y, 0f);

            for (int i = 0; i < children.Length; i++)
            {
                children[i].position = childStartPositions[i] + offset;
            }
        }

    }
}