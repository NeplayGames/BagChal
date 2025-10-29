using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace NeplayGame.BagChal.UI
{
    public class MainMenuUI : MonoBehaviour
    {
          [SerializeField] private Button startButton;

        void Start()
        {
            startButton.onClick.AddListener(RestartGame);
        }

        private void RestartGame()
        {
            SceneManager.LoadScene(1);
        }
        
         void OnDestroy()
        {
            startButton.onClick.AddListener(RestartGame);
        }
    }
}