using RNGNeeds.Samples.RandomAudioClip;
using UnityEngine;
using UnityEngine.UI;

namespace RNGNeeds.Samples
{
    public class Unit : MonoBehaviour
    {
        public string unitName;
        public string unitType;
        public Sprite unitPortrait;
        public float speed = 5.0f;
        public float pickUpSpeed = 1.0f;
        public Image selectionCircle;
        public UnitAudio unitAudio;
        public AudioSource audioSource;
        
        private Vector3 targetPosition;

        private void Start()
        {
            targetPosition = transform.position;
            ToggleSelection(false);
        }

        private void Update()
        {
            if (targetPosition != transform.position)
            {
                Vector3 directionToTarget = (targetPosition - transform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
                if (Physics.Raycast(transform.position, (targetPosition - transform.position).normalized, out RaycastHit hit, speed * Time.deltaTime))
                {
                    if (!hit.collider.gameObject.GetComponent<Interactable>()) targetPosition = transform.position;
                }
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            Interactable interactable = other.gameObject.GetComponent<Interactable>();
            if (interactable) StartCoroutine(PickUpInteractable(interactable));
        }

        private System.Collections.IEnumerator PickUpInteractable(Component interactable)
        {
            while (interactable.transform.localScale.magnitude > 0.01f)
            {
                interactable.transform.position = Vector3.MoveTowards(interactable.transform.position, transform.position, pickUpSpeed * Time.deltaTime);
                interactable.transform.localScale = Vector3.Lerp(interactable.transform.localScale, Vector3.zero, pickUpSpeed * Time.deltaTime);
                yield return null;
            }
            
            Destroy(interactable.gameObject);
        }

        private void SetTargetPosition(Vector3 target)
        {
            targetPosition = new Vector3(target.x, transform.position.y, target.z);
        }

        public void ToggleSelection(bool isSelected)
        {
            selectionCircle.enabled = isSelected;
        }

        public void SelectCommand()
        {
            if(audioSource.isPlaying == false) audioSource.PlayOneShot(unitAudio.selectResponses.PickValue());
            ToggleSelection(true);
        }
        
        public void MoveCommand(Vector3 target)
        {
            if(audioSource.isPlaying == false) audioSource.PlayOneShot(unitAudio.moveResponses.PickValue());
            SetTargetPosition(target);
        }

        public void InteractCommand(Interactable interactable)
        {
            if (audioSource.isPlaying == false) audioSource.PlayOneShot(unitAudio.interactResponses.PickValue());
            SetTargetPosition(interactable.GetInteractPoint());
        }
    }
}