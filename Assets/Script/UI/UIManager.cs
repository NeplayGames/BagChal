using TMPro;
using UnityEngine;
namespace NeplayGame.BagChal.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI turnInfoTMP;

        public void SetTurnInfoText(EEntity eEntity)
        {
            turnInfoTMP.text = eEntity == EEntity.Goat ? "Goat Turn" : "Tiger Turn";
        }
    }
}