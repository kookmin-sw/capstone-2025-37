using UnityEngine;

namespace RNGNeeds.Samples.DicePlayground
{
    public class DieBrain : MonoBehaviour
    {
        public GameObject materialObject;
        public Rigidbody objectBody;
        public Collider dieCollider;
        
        public int rollResult;
        
        public Vector3 spawnPosition;
        public Vector3 force;
        public Vector3 torque;
        public int topFace;

        public DieFaceData dieFaceData;

        public void DetermineTopFace()
        {
            topFace = -1;
            var smallestAngle = 180.0f;
            
            for (var i = 1; i <= dieFaceData.faceDirections.Count; i++)
            {
                Vector3 worldFaceDir = objectBody.transform.TransformDirection(dieFaceData.faceDirections[i - 1].direction);
                var angle = Vector3.Angle(worldFaceDir, Vector3.up);
                if (angle < smallestAngle)
                {
                    smallestAngle = angle;
                    topFace = i;
                }
            }

            // Debug.Log("Face " + topFace + " is on top");
        }

        public void ResetAndOrient()
        {
            transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
            materialObject.transform.localRotation = dieFaceData.GetRotation(topFace, rollResult);
        }

        public void PerformRoll()
        {
            transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
            objectBody.AddForce(force, ForceMode.Impulse);
            objectBody.AddTorque(torque, ForceMode.Impulse);
        }

        public void SetForceAndTorque(Vector3 setForce, Vector3 setTorque)
        {
            force = setForce;
            torque = setTorque;
        }
    }
}