using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DominoGames.DominoRx.RxField;

namespace DominoGames.DominoRx.UI
{
    public class RxTabButton_AchheroStyle : RxTabButton
    {
        [SerializeField, UIAutoAttachField] Image iconImage;
        [SerializeField, UIAutoAttachField] GameObject textObject;
        [SerializeField, UIAutoAttachField] GameObject lightObject;

        public override void OnTabActiveChanged(bool status)
        {
            StopAllCoroutines();

            GetComponent<LayoutElement>().DOKill();
            GetComponent<LayoutElement>().DOFlexibleSize(new Vector2(status ? 1.4f : 1f, 1f), 0.3f).SetEase(Ease.OutQuart);

            iconImage.transform.DOKill();
            GetComponent<Image>().DOKill();
            var size = GetComponent<RectTransform>().sizeDelta;

            if (status)
            {
                lightObject.SetActive(true);
                size.y = 190f;
                GetComponent<RectTransform>().DOSizeDelta(size, 0.3f).SetEase(Ease.OutBack);
                iconImage.transform.DOLocalMoveY(35f, 0.3f).SetEase(Ease.OutQuart);
                textObject.SetActive(true);
                GetComponent<Image>().DOColor(new Color(0.2666667f, 0.3215f, 0.8352f), 0.3f).SetEase(Ease.OutQuart);
                iconImage.transform.DOScale(1.1f, 0.3f).SetEase(Ease.OutBack);
                targetTab.SetActive(true);
            }
            else
            {
                lightObject.SetActive(false);
                size.y = 180f;
                textObject.SetActive(false);
                GetComponent<RectTransform>().DOSizeDelta(size, 0.3f).SetEase(Ease.OutQuart);
                GetComponent<Image>().DOColor(new Color(0.1176f, 0.1411f, 0.2509f), 0.3f).SetEase(Ease.OutQuart);
                iconImage.transform.DOScale(1f, 0.3f).SetEase(Ease.OutQuart);
                iconImage.transform.DOLocalMoveY(11f, 0.3f).SetEase(Ease.OutQuart);
                targetTab.SetActive(false);
            }
        }
    }
}