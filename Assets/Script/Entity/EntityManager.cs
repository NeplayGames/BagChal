using System.Collections.Generic;
using NeplayGame.BagChal.Entity;
using NeplayGame.BagChal.UI;
using UnityEngine;
namespace NeplayGame.BagChal
{
    public class EntityManager
    {
        private GameObject goat;
        private UIManager uIManager;
        private int totalGoat = 0;
        private EEntity currentEntity;
        private InputManager inputManager;
        Dictionary<SpawnPoint, EntityController> entitySpawnPoints = new();
        public EntityManager(GenerateBoard generateBoard, GameObject tiger, GameObject goat, UIManager uIManager, InputManager inputManager)
        {
            this.uIManager = uIManager;
            this.goat = goat;
            foreach (var tigerSpawnPoint in generateBoard.TigerSpawnPoint)
            {
                GameObject.Instantiate(tiger, tigerSpawnPoint.transform.position + Vector3.up, tiger.transform.rotation);
            }
            this.inputManager = inputManager;
            inputManager.TouchEntity += CurrentTouchEntity;
        }

        private void CurrentTouchEntity(SpawnPoint spawnPoint)
        {
            if (currentEntity == EEntity.Goat)
            {
                if (totalGoat <= 20)
                {
                    if (!entitySpawnPoints.ContainsKey(spawnPoint))
                    {
                        GameObject.Instantiate(goat, spawnPoint.transform.position + Vector3.up, goat.transform.rotation);
                        currentEntity = EEntity.Tiger;
                        totalGoat++;
                    }
                    return;
                }

            }
        }
    }
}