using UnityEngine;

namespace RNGNeeds.Samples
{
    public class InteractTargetIndicator : MonoBehaviour
    {
        public float animateTime = .5f;

        public GameObject indicator;
        public SpriteRenderer spriteRenderer;
        public Vector3 startPosition = new Vector3(0f, 1f, 0f);
        
        private void Start()
        {
            StartCoroutine(Animate());
            transform.localPosition = new Vector3(transform.localPosition.x, 0f, transform.localPosition.z);
            indicator.transform.localPosition = startPosition;
        }

        private System.Collections.IEnumerator Animate()
        {
            float time = 0;

            while (time < animateTime)
            {
                indicator.transform.localPosition = Vector3.Lerp(startPosition, Vector3.zero, time / animateTime);

                Color c = spriteRenderer.color;
                c.a = Mathf.Lerp(1, 0, time / animateTime);
                spriteRenderer.color = c;
                
                time += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}