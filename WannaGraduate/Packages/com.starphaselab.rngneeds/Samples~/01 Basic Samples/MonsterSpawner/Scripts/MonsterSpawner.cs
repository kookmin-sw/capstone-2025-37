using UnityEngine;

namespace RNGNeeds.Samples.MonsterSpawner
{
    public class MonsterSpawner : MonoBehaviour
    {
        public Transform playerLocation;
        public ProbabilityList<Monster> monsterTypes;
        public ProbabilityList<int> monsterLevels;
        public ProbabilityList<SpawnLocation> spawnLocations;

        private void Start()
        {
            InvokeRepeating(nameof(SpawnMonster), .5f, .5f);
        }

        private void Update()
        {
            for (var i = 0; i < spawnLocations.ItemCount; i++)
            {
                var spawnLocationItem = spawnLocations.GetProbabilityItem(i);
                var spawnLocation = spawnLocationItem.Value;
                spawnLocation.influence.SetText(spawnLocationItem.InfluenceProvider.ProbabilityInfluence.ToString("F1"));
                spawnLocation.probability.SetText(spawnLocationItem.Probability.ToString("P2"));
            }
        }

        public void SpawnMonster()
        {
            Monster monster = monsterTypes.PickValue();
            if (spawnLocations.TryPickValue(out var location) == false) return;
            var monsterObject = location.Spawn(monster.monsterPrefab);
            monsterObject.GetComponent<MonsterController>().SetStatsByLevel(monsterLevels.PickValue(), playerLocation);
        }
    }
}