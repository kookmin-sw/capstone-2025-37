using StarphaseTools.Core;
using UnityEngine;

namespace RNGNeeds.Samples.AttackManager
{
    public class AudioManager : MonoBehaviour, IProbabilityInfluenceProvider
    {
        public float musicLevel;
        public string InfluenceInfo => $"Increased chance if music level over 75%\nLevel {musicLevel:P2} = {ProbabilityInfluence} Inf.";
        public float ProbabilityInfluence => musicLevel.Remap(.75f, 1f, 0f, 1f);
    }
}