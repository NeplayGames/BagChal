using UnityEngine;

namespace NeplayGame.BagChal.Entity
{
    public abstract class EntityController : MonoBehaviour
    {
        public abstract EEntity eEntity { get; }
        public void Move(Vector3 position)
        {
            
        }
    }

    public enum EEntity
    {
        None = 0,
        Goat = 1,
        Tiger = 2,
    }
}
