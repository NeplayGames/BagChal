using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace NeplayGame.BagChal.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button audioOn;
        [SerializeField] private TextMeshProUGUI audioInfoTMP;
        bool isAudioOn = true;
        void Start()
        {
            startButton.onClick.AddListener(RestartGame);
            audioOn.onClick.AddListener(AudioSwitch);
        }

        private void AudioSwitch()
        {
            isAudioOn = !isAudioOn;
            SoundManager.Instance.PlaySound = isAudioOn;
            audioInfoTMP.text = isAudioOn ? "Audio ON" : "Audio OFF";
        }

        private void RestartGame()
        {
            SceneManager.LoadScene(1);
        }

        void OnDestroy()
        {
            startButton.onClick.AddListener(RestartGame);
            audioOn.onClick.RemoveListener(AudioSwitch);
        }
    }
}