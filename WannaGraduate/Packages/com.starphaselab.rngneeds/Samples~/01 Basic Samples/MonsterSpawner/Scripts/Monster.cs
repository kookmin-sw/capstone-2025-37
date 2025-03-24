using UnityEngine;

namespace RNGNeeds.Samples.MonsterSpawner
{
    [CreateAssetMenu(fileName = "Monster Type", menuName = "RNGNeeds/Monster Spawner/Monster")]
    public class Monster : ScriptableObject
    {
        public GameObject monsterPrefab;
    }
}