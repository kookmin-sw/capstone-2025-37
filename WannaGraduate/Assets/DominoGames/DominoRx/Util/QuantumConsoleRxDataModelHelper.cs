using QFSW.QC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DominoGames.DominoRx.DataModel;
using DominoGames.DominoRx.Util;
using System.Reflection;
using DominoGames.DominoRx.RxField;
using Mono.CSharp;

public class QuantumConsoleRxDataModelHelper : MonoBehaviour
{
    [Command("Game_SetRxValue")]
    public static void SetRxValue([QCSuggestion_RxDataModel]string className, string fieldPath, string data)
    {
        var target = RxDataAccessor.GetRxField(className, fieldPath);
        
        target.SetValue(Convert.ChangeType(data, target.GetDataType()));
    }


    [Command("Game_SaveModel")]
    public static void SaveModel([QCSuggestion_RxDataModel] string className)
    {
        var target = System.Type.GetType(className + ", Assembly-CSharp");
        var dataField = target.GetField("data", BindingFlags.Static | BindingFlags.Public);
        var dataObject = dataField.GetValue(null);
        dataObject.GetType().GetMethod("Save").Invoke(dataObject, null);
    }







    public struct RxDataModelTag : IQcSuggestorTag   
    {

    }

    /// <summary>
    /// Specifies that scene name values should be suggested for the parameter.
    /// </summary>
    public sealed class QCSuggestion_RxDataModel : SuggestorTagAttribute
    {
        private RxDataModelTag _tag;

        public override IQcSuggestorTag[] GetSuggestorTags()
        {
            return new IQcSuggestorTag[] { _tag };
        }
    }

    public class RxDataModelSuggestor : BasicCachedQcSuggestor<string>
    {
        protected override bool CanProvideSuggestions(SuggestionContext context, SuggestorOptions options)
        {
            return context.HasTag<RxDataModelTag>();
        }

        protected override IEnumerable<string> GetItems(SuggestionContext context, SuggestorOptions options)
        {
            return RxDataModelSearch.GetDerivedClassNames(typeof(RxDataModel<>));
        }

        protected override IQcSuggestion ItemToSuggestion(string item)
        {
            return new RawSuggestion(item);
        }
    }
}