using UnityEngine;

namespace RNGNeeds.Samples
{
    public class Interactable : MonoBehaviour
    {
        public Vector3 interactPoint;

        public float growTime = .5f;

        private Vector3 fullSize;

        private void Awake()
        {
            fullSize = transform.localScale;
            transform.localScale = Vector3.zero;
        }

        public void Appear()
        {
            StartCoroutine(Grow());
        }

        private System.Collections.IEnumerator Grow()
        {
            float time = 0;

            while (time < growTime)
            {
                transform.localScale = Vector3.Lerp(Vector3.zero, fullSize, time / growTime);
                time += Time.deltaTime;
                yield return null;
            }

            transform.localScale = fullSize;
        }
        
        public Vector3 GetInteractPoint()
        {
            return transform.TransformPoint(interactPoint);
        }
    }

}