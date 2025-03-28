using UnityEngine;

namespace RNGNeeds.Samples.RandomAudioClip
{
    public class Spawner : MonoBehaviour
    {
        public GameObject interactablePrefab;
        public SpawnLocation spawnLocation;
        
        public float spawnInterval = 5f;

        private void Start()
        {
            InvokeRepeating(nameof(Spawn), spawnInterval, spawnInterval);
        }

        private void Spawn()
        {
            GameObject newInteractable = spawnLocation.Spawn(interactablePrefab);
            newInteractable.GetComponent<Interactable>().Appear();
        }
    }
}