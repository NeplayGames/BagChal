using TMPro;
using UnityEngine;
namespace NeplayGame.BagChal.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI turnInfoTMP;

        public void SetTurnInfoText(bool goat)
        {
            turnInfoTMP.text = goat ? "Goat Turn" : "Tiger Turn";
        }
    }
}