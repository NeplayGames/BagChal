using System;
using System.Collections;
using NeplayGame.BagChal.UI;
using UnityEngine;

namespace NeplayGame.BagChal
{
    public class GameManager : MonoBehaviour
    {

        [SerializeField] private GameObject spawnPointGO;
        [SerializeField] private GameObject tiger;
        [SerializeField] private GameObject goat;
        [SerializeField, Range(6, 100)] private float speed = 6;
        [SerializeField] private Material lineMaterial;
        [SerializeField] private UIManager uIManager;
        [SerializeField, Range(1, 100)] private float consecutiveDistance;
        [SerializeField, Range(0.1f, 3f)] private float aiThinkingTime = 0.65f;
        private InputManager inputManager;
        private const int TOTAL_GOAT_TO_EAT = 5;
        private int totalGoatKill = 0;
        private EntityManager entityManager;
        private bool isGameOver;



        void Start()
        {
            inputManager = new();
            bool useAI = GameModeSettings.CurrentMode == GameMode.PlayerVsAI;
            entityManager = new EntityManager(new GenerateBoard(spawnPointGO, consecutiveDistance, lineMaterial), tiger, goat, inputManager, speed, useAI ? GameModeSettings.AIEntity : EEntity.None, GameModeSettings.Difficulty);
            entityManager.GoatKill += GoatKilled;
            entityManager.OnChangeTurn += ChangeTurn;
            uIManager.SetTurnInfoText(EEntity.Goat, 20);
            uIManager.StartFirstTimeTutorial();
            if (useAI && GameModeSettings.AIEntity == EEntity.Goat)
                StartCoroutine(PlayAITurn());
        }

        private void ChangeTurn(EEntity eEntity, int goatLeft)
        {
            uIManager.SetTurnInfoText(eEntity, goatLeft);
            uIManager.NotifyTutorialTurn(eEntity, goatLeft);
            CheckGameOver();
            if (!isGameOver && GameModeSettings.CurrentMode == GameMode.PlayerVsAI && eEntity == GameModeSettings.AIEntity)
                StartCoroutine(PlayAITurn());
        }

        private IEnumerator PlayAITurn()
        {
            yield return new WaitForSeconds(aiThinkingTime);
            if (!isGameOver && entityManager != null)
                entityManager.PerformAITurn();
        }

        private void GoatKilled()
        {
            totalGoatKill++;
            uIManager.SetGoatKillInfo(totalGoatKill);
        }

        void Update()
        {
            inputManager?.Update();
        }

        public void CheckGameOver()
        {
            if (totalGoatKill == TOTAL_GOAT_TO_EAT)
            {
                isGameOver = true;
                DeregisterEvents();
                uIManager.SetGameOverText(EEntity.Tiger);
                return;
            }
            if (entityManager.CheckTigerLock())
            {
                isGameOver = true;
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
            if (entityManager == null)
                return;
            entityManager.GoatKill -= GoatKilled;
            entityManager.OnChangeTurn -= ChangeTurn;
            entityManager.Dispose();
            entityManager = null;
            inputManager = null;
        }
    }
}