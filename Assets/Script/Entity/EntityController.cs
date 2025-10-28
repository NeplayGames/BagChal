using System;
using UnityEngine;

namespace NeplayGame.BagChal.Entity
{
    public abstract class EntityController : MonoBehaviour
    {
        [SerializeField] protected Animator animator;
        public event Action MovementCompleted;
        public abstract EEntity eEntity { get; }
        private Vector3 startPosition;
        private Vector3 targetPosition;
        private float duration;
        private float elapsedTime;
        protected bool isMoving = false;

        protected virtual void Update()
        {
            if (isMoving)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                t = Mathf.Clamp01(t);  // Keep t between 0 and 1

                transform.position = Vector3.Lerp(startPosition, targetPosition, t);

                // Stop when we reach the endpoint
                if (t >= 1f)
                {
                    isMoving = false;
                    animator.SetBool("Movement", false);
                    MovementCompleted?.Invoke();
                }
            }
        }

        public void MoveTo(Vector3 destination, float speed = 6)
        {

            startPosition = transform.position;
            targetPosition = destination + Vector3.up;
            transform.LookAt(targetPosition);
            // Calculate duration using distance / speed
            float distance = Vector3.Distance(startPosition, targetPosition);
            duration = distance / speed;

            elapsedTime = 0f;
            isMoving = true;
            animator.SetBool("Movement", true);
        }
    }
}

public enum EEntity
{
    None = 0,
    Goat = 1,
    Tiger = 2,
}
