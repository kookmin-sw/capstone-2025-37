using UnityEngine;

namespace RNGNeeds.Samples.MonsterSpawner
{
    public class MonsterController : MonoBehaviour
    {
        public float speed = 5f;
        public int level;
        public int health;
        public int attack;
        public Transform target;
        
        public void SetStatsByLevel(int monsterLevel, Transform playerLocation)
        {
            level = monsterLevel;
            speed = (float)monsterLevel / 2;
            health = monsterLevel * 20;
            attack = monsterLevel * 2;
            target = playerLocation;
        }
        
        private void Update()
        {
            if (target.position != transform.position)
            {
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
            }

            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            var unit = other.gameObject.GetComponent<Unit>();
            if(unit) Destroy(gameObject);
        }
    }
}