using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace NeplayGame.BagChal.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Menu Panels")]
        [SerializeField] private GameObject menuRoot;
        [FormerlySerializedAs("instructionsPanel")]
        [SerializeField] private GameObject instructionsModal;

        [Header("Side Selection")]
        [SerializeField] private Button goatSideButton;
        [SerializeField] private Button tigerSideButton;
        [SerializeField] private Color selectedSideColor = new Color(0.94f, 0.67f, 0.22f, 1f);
        [SerializeField] private Color unselectedSideColor = new Color(0.12f, 0.17f, 0.18f, 1f);
        [SerializeField] private Color selectedSideTextColor = new Color(0.035f, 0.055f, 0.07f, 1f);
        [SerializeField] private Color unselectedSideTextColor = Color.white;

        [Header("Difficulty")]
        [SerializeField] private Slider difficultySlider;
        [SerializeField] private TextMeshProUGUI difficultyValueTMP;

        [Header("Actions")]
        [FormerlySerializedAs("aiButton")]
        [SerializeField] private Button playAIButton;
        [FormerlySerializedAs("startButton")]
        [SerializeField] private Button localTwoPlayerButton;
        [FormerlySerializedAs("instructionsButton")]
        [SerializeField] private Button howToPlayButton;
        [SerializeField] private Button closeInstructionsButton;

        [Header("Audio")]
        [FormerlySerializedAs("audioOn")]
        [SerializeField] private Button audioButton;
        [FormerlySerializedAs("audioInfoTMP")]
        [SerializeField] private TextMeshProUGUI audioLabelTMP;

        private bool isAudioOn = true;

        void Start()
        {
            goatSideButton.onClick.AddListener(SelectGoats);
            tigerSideButton.onClick.AddListener(SelectTigers);
            difficultySlider.onValueChanged.AddListener(SetDifficulty);
            playAIButton.onClick.AddListener(StartAIGame);
            localTwoPlayerButton.onClick.AddListener(StartLocalGame);
            howToPlayButton.onClick.AddListener(ShowInstructions);
            closeInstructionsButton.onClick.AddListener(HideInstructions);
            audioButton.onClick.AddListener(ToggleAudio);

            menuRoot.SetActive(true);
            instructionsModal.SetActive(false);
            difficultySlider.minValue = 0f;
            difficultySlider.maxValue = 2f;
            difficultySlider.wholeNumbers = true;
            difficultySlider.SetValueWithoutNotify((int)GameModeSettings.Difficulty);
            UpdateSideSelection();
            UpdateDifficultyText();
            UpdateAudioText();
        }

        private void SelectGoats()
        {
            GameModeSettings.PlayerSide = PlayerSide.Goat;
            UpdateSideSelection();
        }

        private void SelectTigers()
        {
            GameModeSettings.PlayerSide = PlayerSide.Tiger;
            UpdateSideSelection();
        }

        private void UpdateSideSelection()
        {
            bool goatsSelected = GameModeSettings.PlayerSide == PlayerSide.Goat;
            SetButtonState(goatSideButton, goatsSelected);
            SetButtonState(tigerSideButton, !goatsSelected);
        }

        private void SetButtonState(Button button, bool selected)
        {
            Image image = button.GetComponent<Image>();
            if (image != null)
                image.color = selected ? selectedSideColor : unselectedSideColor;

            TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
                label.color = selected ? selectedSideTextColor : unselectedSideTextColor;
        }

        private void SetDifficulty(float value)
        {
            GameModeSettings.Difficulty = (AIDifficulty)Mathf.RoundToInt(value);
            UpdateDifficultyText();
        }

        private void UpdateDifficultyText()
        {
            difficultyValueTMP.text = GameModeSettings.Difficulty.ToString().ToUpperInvariant();
        }

        private void ShowInstructions()
        {
            instructionsModal.SetActive(true);
        }

        private void HideInstructions()
        {
            instructionsModal.SetActive(false);
        }

        private void ToggleAudio()
        {
            isAudioOn = !isAudioOn;
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySound = isAudioOn;
            UpdateAudioText();
        }

        private void UpdateAudioText()
        {
            audioLabelTMP.text = isAudioOn ? "AUDIO ON" : "AUDIO OFF";
        }

        private void StartLocalGame()
        {
            GameModeSettings.CurrentMode = GameMode.LocalTwoPlayer;
            SceneManager.LoadScene(1);
        }

        private void StartAIGame()
        {
            GameModeSettings.CurrentMode = GameMode.PlayerVsAI;
            SceneManager.LoadScene(1);
        }

        void OnDestroy()
        {
            goatSideButton.onClick.RemoveListener(SelectGoats);
            tigerSideButton.onClick.RemoveListener(SelectTigers);
            difficultySlider.onValueChanged.RemoveListener(SetDifficulty);
            playAIButton.onClick.RemoveListener(StartAIGame);
            localTwoPlayerButton.onClick.RemoveListener(StartLocalGame);
            howToPlayButton.onClick.RemoveListener(ShowInstructions);
            closeInstructionsButton.onClick.RemoveListener(HideInstructions);
            audioButton.onClick.RemoveListener(ToggleAudio);
        }
    }
}