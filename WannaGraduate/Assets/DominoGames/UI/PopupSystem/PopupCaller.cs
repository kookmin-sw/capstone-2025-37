using DG.Tweening;
using DominoGames.UI.PopupSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupCaller : MonoBehaviour
{
    [ValueDropdown("GetPopupNames")] public string targetPopup;

    [SerializeField] float showDuration, hideDuration;
    [SerializeField] Ease showEase, hideEase;



    private List<string> GetPopupNames()
    {
        var targetEnum = System.Type.GetType("EPopupNames_" + SceneManager.GetActiveScene().name);
        return System.Enum.GetNames(targetEnum).ToList();
    }



    
    


    public void ShowPopup(object parameter = null)
    {
        Action<PopupBase> direction = PopupSystem.MakeScaleShowDirection(showDuration, showEase);

        if(showDuration == 0)
        {
            direction = (x) =>
            {
                x.transform.localScale = Vector3.one;
                x.gameObject.SetActive(true);
            };
        }

        PopupSystem.Show(targetPopup, parameter, direction);
    }

    public void HidePopup()
    {
        Action<PopupBase> direction = PopupSystem.MakeScaleShowDirection(hideDuration, hideEase);

        if (hideDuration == 0)
        {
            direction = (x) =>
            {
                x.gameObject.SetActive(false);
            };
        }

        PopupSystem.Hide(targetPopup, direction);
    }
}
