using System;
using System.Collections.Generic;
using UnityEngine;

namespace RNGNeeds.Samples.DicePlayground
{
    [Serializable]
    public struct FaceDirectionsData
    {
        public int face;
        public Vector3 direction;
    }

    [Serializable]
    public struct RotationMapData
    {
        public int topFace;
        public int desiredFace;
        public Vector3 rotation;
    }
    
    [CreateAssetMenu(fileName = "DXX Face Data", menuName = "RNGNeeds/Dice Playground/Die Face Data")]
    public class DieFaceData : ScriptableObject
    {
        public List<FaceDirectionsData> faceDirections;
        public List<RotationMapData> rotationMap;

        public Quaternion GetRotation(int topFace, int desiredFace)
        {
            foreach (var rotationData in rotationMap)
            {
                if (rotationData.topFace == topFace && rotationData.desiredFace == desiredFace)
                    return Quaternion.Euler(rotationData.rotation);
            }
            
            return Quaternion.identity;
        }
    }
}