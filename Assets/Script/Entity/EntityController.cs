using System;
using UnityEngine;

namespace NeplayGame.BagChal.Entity
{
    public abstract class EntityController : MonoBehaviour
    {
        [SerializeField] protected Animator animator;
        private bool isAnimating = false;
        private float timeCounter = 0f;
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
                    PlayAnimation("Movement", false);
                    MovementCompleted?.Invoke();
                }
            }
            if (!isAnimating) return;

            timeCounter += Time.deltaTime * 2;
            // Use sine wave to oscillate scale between smaller and larger
            float scaleFactor = 1f + ((Mathf.Sin(timeCounter) + 1f) / 2f) * 0.5f;
            transform.localScale = Vector3.one * scaleFactor;

        }
        public void StartGrowShrink()
        {
            isAnimating = true;
        }

        // Stop the animation
        public void StopGrowShrink()
        {
            isAnimating = false;
            transform.localScale = Vector3.one;
            timeCounter = 0f;
        }

        protected void PlayAnimation(string animationStr, bool animationBool)
        {
            if (!animator) return;
            animator.SetBool(animationStr, animationBool);
        }
        public void MoveTo(Vector3 destination, float speed = 6)
        {
            SoundManager.Instance.PlayMovement();
            StopGrowShrink();
            startPosition = transform.position;
            targetPosition = destination + Vector3.up;
            if (animator)
                transform.LookAt(targetPosition);
            // Calculate duration using distance / speed
            float distance = Vector3.Distance(startPosition, targetPosition);
            duration = distance / speed;
            elapsedTime = 0f;
            isMoving = true;
            PlayAnimation("Movement", true);
        }
    }
}

public enum EEntity
{
    None = 0,
    Goat = 1,
    Tiger = 2,
}
