using StarphaseTools.Core;
using TMPro;
using UnityEngine;

namespace RNGNeeds.Samples
{
    public class SpawnLocation : MonoBehaviour, IProbabilityItemColorProvider, IProbabilityInfluenceProvider
    {
        public GameObject spawnRadius;
        public SpriteRenderer radiusSprite;
        public Transform playerLocation;
        public TMP_Text influence;
        public TMP_Text probability;
        
        public GameObject Spawn(GameObject prefab)
        {
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius.transform.localScale.x * 5;
            spawnPosition.y = transform.position.y;
            return Instantiate(prefab, spawnPosition, Quaternion.identity);
        }

        public Color ItemColor => radiusSprite.color;
        public string InfluenceInfo => $"Distance to player: {DistanceToPlayer} = {ProbabilityInfluence}";
        public float DistanceToPlayer => Vector3.Distance(transform.position, playerLocation.position);
        public float ProbabilityInfluence => DistanceToPlayer.Remap(0f, 30f, 1f, -1f);
    }
}