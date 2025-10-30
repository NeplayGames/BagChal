using System;
using UnityEngine;
namespace NeplayGame.BagChal.Entity
{
    public class GoatEntity : EntityController
    {
        [SerializeField] private float totalDiedTime = 1.5f;
        public override EEntity eEntity { get; } = EEntity.Goat;
        public event Action IsDead;
        public  void PlayDeathAnimation()
        {
             PlayAnimation("Died", true);
            Invoke(nameof(Died), totalDiedTime);
        }

        private void Died()
        {
            IsDead?.Invoke();
        }
    }
}