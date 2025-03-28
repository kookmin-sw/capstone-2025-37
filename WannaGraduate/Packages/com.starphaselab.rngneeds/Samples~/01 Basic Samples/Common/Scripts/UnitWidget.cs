using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RNGNeeds.Samples
{
    public class UnitWidget : MonoBehaviour
    {
        public Image unitPortrait;
        public TMP_Text unitName;
        public TMP_Text unitType;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void SetUnit(Unit unit)
        {
            unitPortrait.sprite = unit.unitPortrait;
            unitName.SetText(unit.unitName);
            unitType.SetText(unit.unitType);
            gameObject.SetActive(true);
        }

        public void ClearWidget()
        {
            gameObject.SetActive(false);
        }
    }
}