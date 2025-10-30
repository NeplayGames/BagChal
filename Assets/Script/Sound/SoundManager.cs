using UnityEngine;
namespace NeplayGame.BagChal
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioSource sfxAudioSource;
        [SerializeField] private AudioClip moveClip;
        [SerializeField] private AudioClip goatKill;
        public static SoundManager Instance;
        public bool PlaySound
        {
            set
            {
                playSound = value;
                this.gameObject.SetActive(playSound);
            }
        }

        private bool playSound = true;
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

        public void PlayMovement()
        {
            sfxAudioSource.PlayOneShot(moveClip);
        }

        public void PlayGoatKill()
        {
            sfxAudioSource.PlayOneShot(goatKill);
        }
    }
}