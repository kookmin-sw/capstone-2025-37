using UnityEngine;

namespace RNGNeeds.Samples.AttackManager
{
    public enum DamageType
    {
        Normal,
        CriticalStrike,
        CrushingBlow,
        Overpower,
        Stun
    }

    public class PlayerAttack : MonoBehaviour
    {
        public ProbabilityList<DamageType> damageType;
        
        [Header("Odds of Idle Dance Pose")]
        public ProbabilityList<bool> oddsOfIdleDancing;
    }

}