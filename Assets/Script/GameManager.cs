using UnityEngine;

namespace NeplayGame.BagChal
{
    public class GameManager : MonoBehaviour
    {

        [SerializeField] private GameObject spawnPointGO;
        [SerializeField] private GameObject tiger;
        [SerializeField] private GameObject goat;
        [SerializeField] private Material lineMaterial;
        [SerializeField, Range(1, 100)] private float consecutiveDistance;
        void Start()
        {
           new NPCManager(new GenerateBoard(spawnPointGO, consecutiveDistance, lineMaterial), tiger);
        }
    }
}