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
            EntityManager entityManager = new EntityManager(new GenerateBoard(spawnPointGO, consecutiveDistance, lineMaterial), tiger, goat, uIManager, inputManager);
            entityManager.GoatKill += GoatKilled;
        }

        private void GoatKilled()
        {
            totalGoatKill++;
            CheckGameOver();
        }

        void Update()
        {
            inputManager?.Update();
        }

        public void CheckGameOver()
        {
            if(totalGoatKill == TOTAL_GOAT_TO_EAT)
            {
                entityManager.GoatKill -= GoatKilled;
                entityManager = null;
                inputManager = null;
            }
        }

        void OnDestroy()
        {
            if(entityManager != null)
            entityManager.GoatKill -= GoatKilled;
        }
    }
}