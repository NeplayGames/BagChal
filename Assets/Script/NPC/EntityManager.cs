using UnityEngine;
namespace NeplayGame.BagChal
{
    public class NPCManager
    {
        public NPCManager(GenerateBoard generateBoard, GameObject tiger)
        {
            foreach (var tigerSpawnPoint in generateBoard.TigerSpawnPoint)
            {
                GameObject.Instantiate(tiger, tigerSpawnPoint.transform.position + Vector3.up, tiger.transform.rotation);
            }
        }

    }
}