using System.Linq;
using UnityEngine;

namespace RNGNeeds.Samples.DicePlayground
{
    [CreateAssetMenu(fileName = "DXX", menuName = "RNGNeeds/Dice Playground/Die")]
    public class Die : ScriptableObject
    {
        public GameObject diePrefab;
        public ProbabilityList<int> sides;

        public int Roll(int numOfRolls)
        {
            return sides.PickValues(numOfRolls).Sum();
        }
    }
}