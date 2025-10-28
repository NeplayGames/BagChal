using System;
using UnityEngine;
namespace NeplayGame.BagChal.Entity
{
    public class GoatEntity : EntityController
    {
        public override EEntity eEntity { get; } = EEntity.Goat;
        public event Action IsDead;
        public  void PlayDeathAnimation()
        {
            animator.SetBool("Died", true);
            Invoke(nameof(Died), 1.5f);
        }

        private void Died()
        {
            IsDead?.Invoke();
        }
    }
}