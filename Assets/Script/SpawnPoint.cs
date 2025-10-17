using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace NeplayGame.BagChal
{
    public class SpawnPoint : MonoBehaviour
    {
        public List<SpawnPoint> movablePoint { set; private get; }

        public bool IsOccupied { get; set; } = false;
    }
}