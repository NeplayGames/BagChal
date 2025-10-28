using System;
using NeplayGame.BagChal.UI;
using UnityEngine;

namespace NeplayGame.BagChal
{
    public class GameManager : MonoBehaviour
    {

        [SerializeField] private GameObject spawnPointGO;
        [SerializeField] private GameObject tiger;
        [SerializeField] private GameObject goat;
        [SerializeField] private Material lineMaterial;
        [SerializeField] private UIManager uIManager;
        [SerializeField, Range(1, 100)] private float consecutiveDistance;
        private InputManager inputManager;
        private const int TOTAL_GOAT_TO_EAT = 5;
        private int totalGoatKill = 0;
        private EntityManager entityManager;



        void Start()
        {
            inputManager = new();
            entityManager = new EntityManager(new GenerateBoard(spawnPointGO, consecutiveDistance, lineMaterial), tiger, goat, inputManager);
            entityManager.GoatKill += GoatKilled;
            entityManager.OnChangeTurn += ChangeTurn;
        }

        private void ChangeTurn(EEntity eEntity)
        {
            uIManager.SetTurnInfoText(eEntity);
            CheckGameOver();
        }

        private void GoatKilled()
        {
            totalGoatKill++;
        }

        void Update()
        {
            inputManager?.Update();
        }

        public void CheckGameOver()
        {
            if (totalGoatKill == TOTAL_GOAT_TO_EAT)
            {
                DeregisterEvents();
                uIManager.SetGameOverText(EEntity.Tiger);
                return;
            }
            if (entityManager.CheckTigerLock())
            {
                DeregisterEvents();
                uIManager.SetGameOverText(EEntity.Goat);
                return;
            }
        }

        void OnDestroy()
        {
            if (entityManager != null)
            {
                DeregisterEvents();
            }
        }

        private void DeregisterEvents()
        {
            entityManager.GoatKill -= GoatKilled;
            entityManager.OnChangeTurn -= ChangeTurn;
            entityManager = null;
            inputManager = null;
        }
    }
}