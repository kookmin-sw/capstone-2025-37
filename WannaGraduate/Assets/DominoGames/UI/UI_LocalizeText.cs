using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using DominoGames.DominoRx.Util;

public class UI_LocalizeText : TextMeshProUGUI
{
    string originTranslationString = "";
    string valPattern = @"\{\$Val\.([^}]+)\}";
    string locPattern = @"\{\$Loc\.([^}]+)\}";

    public void SetLocalizeText(string localizeKey, Dictionary<string, object> localizeParams = null)
    {
        string originTranslationString = LocalizationManager.GetTermTranslation(localizeKey);
        var targetString = originTranslationString;

        if(localizeParams != null)
        {
            LocalizationManager.ApplyLocalizationParams(ref targetString, localizeParams);
        }

        targetString = ApplyValParams(targetString);
        targetString = ApplyLocParams(targetString);

        this.text = targetString;
    }



    private string ApplyValParams(string targetString)
    {
        return Regex.Replace(targetString, valPattern, match =>
        {
            string key = match.Groups[1].Value;

            string className = key.Substring(0, key.IndexOf("."));
            string fieldPath = key.Substring(key.IndexOf(".") + 1);
            return RxDataAccessor.GetValue(className, fieldPath).ToString();
        });
    }

    private string ApplyLocParams(string targetString)
    {
        return Regex.Replace(targetString, locPattern, match =>
        {
            return LocalizationManager.GetTermTranslation(match.Groups[1].Value);
        });
    }
}
