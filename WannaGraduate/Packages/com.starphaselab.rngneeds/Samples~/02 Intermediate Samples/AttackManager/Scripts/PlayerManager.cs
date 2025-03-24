using StarphaseTools.Core;
using UnityEngine;

namespace RNGNeeds.Samples.AttackManager
{
    public class PlayerManager : MonoBehaviour, IProbabilityInfluenceProvider
    {
        public int playerLevel;
        public string InfluenceInfo => $"Increased based on player level\nLevel {playerLevel} = {ProbabilityInfluence} Inf.";
        public float ProbabilityInfluence => ((float)playerLevel).Remap(1, 20, -1f, 1f);
    }
}