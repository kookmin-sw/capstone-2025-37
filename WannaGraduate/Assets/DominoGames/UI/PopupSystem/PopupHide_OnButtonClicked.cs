using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupHide_OnButtonClicked : PopupCaller
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            HidePopup();
        });
    }
}
