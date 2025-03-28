using UnityEngine;

namespace RNGNeeds.Samples
{
    public class MoveTargetIndicator : MonoBehaviour
    {
        public float maxScale = 1.0f;
        public float growTime = 1.0f;

        public SpriteRenderer spriteRenderer;
        
        private void Start()
        {
            StartCoroutine(Animate());
        }

        private System.Collections.IEnumerator Animate()
        {
            float time = 0;

            while (time < growTime)
            {
                var scale = Mathf.Lerp(0, maxScale, time / growTime);
                transform.localScale = new Vector3(scale, scale, scale);

                Color c = spriteRenderer.color;
                c.a = Mathf.Lerp(1, 0, time / growTime);
                spriteRenderer.color = c;
                
                time += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}