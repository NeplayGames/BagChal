using System;
using UnityEngine;
namespace NeplayGame.BagChal.Entity
{
    public class TigerEntity : EntityController
    {
        public override EEntity eEntity { get; } = EEntity.Tiger;

        private float killDistance = 1.2f; // Distance within which tiger can kill

        private GoatEntity targetGoat;

        protected override void Update()
        {
            base.Update();
            if (targetGoat == null) return;

            float distance = Vector3.Distance(transform.position, targetGoat.transform.position);
            if (distance <= killDistance)
            {
                KillGoat(targetGoat);
            }
        }
        public void PlayKillAnimation()
        {
            //PlayAnimation("Kill"); // must match the animation name in Animator
        }

        private void KillGoat(GoatEntity goat)
        {
            PlayKillAnimation();
            isMoving = false;
            goat.PlayDeathAnimation();
        }
        public void SetGoat(GoatEntity goat)
        {
            targetGoat = goat;
            targetGoat.IsDead += MoveAgain;
        }

        private void MoveAgain()
        {
            targetGoat.IsDead -= MoveAgain;
            Destroy(targetGoat.gameObject);
            targetGoat = null;
            isMoving = true;
        }
    }

}