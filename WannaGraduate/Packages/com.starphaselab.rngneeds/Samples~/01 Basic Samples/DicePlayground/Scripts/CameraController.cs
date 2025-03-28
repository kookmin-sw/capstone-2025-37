using UnityEngine;

namespace RNGNeeds.Samples.DicePlayground
{
    public class CameraController : MonoBehaviour
    {
        public Transform target;
        public float rotationSpeed = 20f;
        private Vector3 offset;
        private Vector3 geometryCenter;
        private float zoomFactor;
        private float transition;

        private void Start()
        {
            geometryCenter = target.position;
            zoomFactor = GetZoomFactor(12);
            transition = 1;
        }

        private void Update()
        {
            transition += Time.deltaTime * .004f;
            transition = Mathf.Clamp01(transition);

            var smooth = Mathf.SmoothStep(0.0f, 1.0f, transition);
            offset = Vector3.Lerp(offset, new Vector3(0f, zoomFactor, -zoomFactor), smooth);
            target.position = Vector3.Lerp(target.position, geometryCenter, smooth);
            transform.RotateAround(target.position, Vector3.up, rotationSpeed * Time.deltaTime);

            Vector3 adjustedOffset = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * offset;
            
            transform.position = target.position + adjustedOffset;

            transform.LookAt(target);
        }
        
        public void SetNewTarget(Vector3 center, int numOfDice)
        {
            zoomFactor = GetZoomFactor(numOfDice);
            geometryCenter = center;
            transition = 0f;
        }

        private static float GetZoomFactor(int numOfDice)
        {
            return Mathf.Lerp(3f, 5, (float)numOfDice / 18);
        }
    }
}