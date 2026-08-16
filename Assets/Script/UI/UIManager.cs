using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NeplayGame.BagChal.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Game HUD")]
        [SerializeField] private TextMeshProUGUI turnInfoTMP;
        [SerializeField] private TextMeshProUGUI GoatKillTMP;
        [SerializeField] private TextMeshProUGUI GoatLeftTMP;

        [Header("Game Over")]
        [SerializeField] private GameObject GameOverPanel;
        [SerializeField] private TextMeshProUGUI gameOverText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Sprite goatSprite;
        [SerializeField] private Sprite tigerSprite;
        [SerializeField] private Image gameWinObjImg;

        [Header("First-Time Tutorial")]
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private TextMeshProUGUI tutorialText;
        [SerializeField] private Button tutorialButton;
        [SerializeField] private TextMeshProUGUI tutorialButtonLabelTMP;

        private const string TutorialCompletedKey = "BagChal.TutorialCompleted";
        private int tutorialStage = -1;

        void Start()
        {
            restartButton.onClick.AddListener(RestartGame);
            backButton.onClick.AddListener(RestartGame);
            if (tutorialButton != null)
                tutorialButton.onClick.AddListener(AdvanceTutorial);
            if (tutorialPanel != null)
                tutorialPanel.SetActive(false);
        }

        private void RestartGame()
        {
            SceneManager.LoadScene(0);
        }

        public void StartFirstTimeTutorial()
        {
            if (tutorialPanel == null || tutorialText == null || tutorialButton == null || tutorialButtonLabelTMP == null)
            {
                tutorialStage = -1;
                return;
            }

            if (PlayerPrefs.GetInt(TutorialCompletedKey, 0) == 1)
            {
                tutorialPanel.SetActive(false);
                return;
            }

            tutorialStage = 0;
            bool playerIsTiger = GameModeSettings.CurrentMode == GameMode.PlayerVsAI &&
                                 GameModeSettings.PlayerSide == PlayerSide.Tiger;
            string introduction = playerIsTiger
                ? "WELCOME TO BAGH CHAL\n\nYou control the tigers. Hunt the AI goats and capture five before they surround you."
                : "WELCOME TO BAGH CHAL\n\nYou control the goats. Place goats and surround all four tigers before they capture five goats.";
            SetTutorialMessage(introduction, "START");
        }

        public void NotifyTutorialTurn(EEntity entity, int goatLeft)
        {
            if (tutorialStage < 0)
                return;

            if (entity == EEntity.Tiger && tutorialStage <= 1)
            {
                tutorialStage = 2;
                SetTutorialMessage(
                    "TIGER TURN\n\nTurns alternate. A tiger can move along a line or jump over an adjacent goat to capture it.",
                    "CONTINUE");
            }
            else if (entity == EEntity.Goat && tutorialStage >= 2)
            {
                tutorialStage = 3;
                SetTutorialMessage(
                    "GOAT TURN\n\nKeep placing goats on empty points. After all 20 are placed, goats move one connected point at a time. Trap every tiger to win.",
                    "GOT IT");
            }
        }

        private void AdvanceTutorial()
        {
            if (tutorialStage == 0)
            {
                tutorialStage = 1;
                bool playerIsTiger = GameModeSettings.CurrentMode == GameMode.PlayerVsAI &&
                                     GameModeSettings.PlayerSide == PlayerSide.Tiger;
                SetTutorialMessage(
                    playerIsTiger
                        ? "GOATS MOVE FIRST\n\nThe AI will place a goat, then your tiger turn begins."
                        : "PLACE A GOAT\n\nTap any empty point on the board to place your first goat.",
                    string.Empty);
                return;
            }

            if (tutorialStage == 2)
            {
                tutorialButton.gameObject.SetActive(false);
                return;
            }

            CompleteTutorial();
        }

        private void SetTutorialMessage(string message, string buttonLabel)
        {
            tutorialPanel.SetActive(true);
            tutorialText.text = message;
            bool showButton = !string.IsNullOrEmpty(buttonLabel);
            tutorialButton.gameObject.SetActive(showButton);
            if (showButton)
                tutorialButtonLabelTMP.text = buttonLabel;
        }

        private void CompleteTutorial()
        {
            PlayerPrefs.SetInt(TutorialCompletedKey, 1);
            PlayerPrefs.Save();
            tutorialStage = -1;
            tutorialPanel.SetActive(false);
        }

        public void SetTurnInfoText(EEntity eEntity, int goatLeft)
        {
            turnInfoTMP.text = eEntity == EEntity.Goat ? "Goat Turn" : "Tiger Turn";
            GoatLeftTMP.text = $"{goatLeft}";
        }

        public void SetGoatKillInfo(int goatKilled)
        {
            GoatKillTMP.text = $"{goatKilled}";
        }

        public void SetGameOverText(EEntity winningTeam)
        {
            GameOverPanel.SetActive(true);
            gameOverText.text = winningTeam == EEntity.Goat ? "Goat Won" : "Tiger Won";
            gameWinObjImg.sprite = winningTeam == EEntity.Goat ? goatSprite : tigerSprite;
        }

        void OnDestroy()
        {
            backButton.onClick.RemoveListener(RestartGame);
            if (restartButton != null)
                restartButton.onClick.RemoveListener(RestartGame);
            if (tutorialButton != null)
                tutorialButton.onClick.RemoveListener(AdvanceTutorial);
        }
    }
}