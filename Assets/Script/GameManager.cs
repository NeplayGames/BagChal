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
        void Start()
        {
            inputManager = new();
            new EntityManager(new GenerateBoard(spawnPointGO, consecutiveDistance, lineMaterial), tiger,goat, uIManager, inputManager);
        }

        void Update()
        {
            inputManager.Update();
        }
    }
}