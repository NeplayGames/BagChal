using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace NeplayGame.BagChal.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI turnInfoTMP;
        [SerializeField] private TextMeshProUGUI GoatKillTMP;
        [SerializeField] private GameObject GameOverPanel;
        [SerializeField] private TextMeshProUGUI gameOverText;
        [SerializeField] private Button restartButton;

        void Start()
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        private void RestartGame()
        {
            SceneManager.LoadScene(0);
        }

        public void SetTurnInfoText(EEntity eEntity)
        {
            turnInfoTMP.text = eEntity == EEntity.Goat ? "Goat Turn" : "Tiger Turn";
        }
        public void SetGoatKillInfo(int goatKilled)
        {
            GoatKillTMP.text = $"{goatKilled}";
        }

        public void SetGameOverText(EEntity winningTeam)
        {
            GameOverPanel.SetActive(true);
            gameOverText.text = winningTeam == EEntity.Goat ? "Goat Won" : "Tiger Won";
        }
        
         void OnDestroy()
        {
            restartButton.onClick.AddListener(RestartGame);
        }
    }
}