using UnityEngine;

namespace NeplayGame.BagChal
{
    public class Entity : MonoBehaviour
    {
        private SpawnPoint currentSpawnPoint;
        public void SetSpawnPoint(SpawnPoint spawnPoint)
        {
            this.currentSpawnPoint = spawnPoint;
        }

        
    }
}
