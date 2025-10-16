using UnityEngine;

namespace NeplayGame.BagChal
{
    public class GameManager : MonoBehaviour
    {

        [SerializeField] private GameObject spawnPointGO;
        [SerializeField] private Material lineMaterial;
        [SerializeField, Range(1, 100)] private float consecutiveDistance;
        void Start()
        {
            new GenerateBoard(spawnPointGO, consecutiveDistance, lineMaterial);
        }
    }
}