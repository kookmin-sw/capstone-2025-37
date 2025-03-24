using System;
using System.Collections.Generic;
using UnityEngine;

namespace RNGNeeds.Samples.DicePlayground
{
    [CreateAssetMenu(fileName = "Dice Roller", menuName = "RNGNeeds/Dice Playground/Dice Roller")]
    public class DiceRoller : ScriptableObject
    {
        [Serializable]
        public struct RollResult
        {
            public string dieName;
            public string pickCountMin;
            public string pickCountMax;
            public List<int> rolls;
        }
        
        public List<Die> diceToRoll;
        public List<RollResult> m_RollResults;
        
        public int RollDice()
        {
            m_RollResults.Clear();
            
            foreach (var die in diceToRoll)
            {
                var newRollResult = new RollResult()
                {
                    dieName = die.name,
                    pickCountMin = die.sides.PickCountMin.ToString(),
                    pickCountMax = die.sides.PickCountMax.ToString(),
                    rolls = new List<int>()
                };
                
                die.sides.PickValues(newRollResult.rolls);
                
                m_RollResults.Add(newRollResult);
            }

            return SumResults();
        }

        private int SumResults()
        {
            var totalResult = 0;
            foreach (var rollResult in m_RollResults)
            {
                foreach (var roll in rollResult.rolls)
                {
                    totalResult += roll;
                }
            }

            return totalResult;
        }
    }
}