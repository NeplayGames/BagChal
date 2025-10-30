using UnityEngine;
namespace NeplayGame.BagChal
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource sfxAudioSource;

        public static SoundManager Instance;

        void Start()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
                return;
            }
            Destroy(this.gameObject);
        }
    }
}