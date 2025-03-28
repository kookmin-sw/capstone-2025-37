using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using DominoGames.DominoRx.DataModel;
using DominoGames.DominoRx.RxField;
using System.Linq;
using Sirenix.Utilities;
using Mono.CSharp;
using QFSW.QC;
using Unity.VisualScripting;
using BBB.CSVData;

public class ExampleDataModel : RxDataModel<ExampleDataModel>, IRxDataModelInitializable
{
    public RxList<ExampleDataInnerDataContainer> exampleClassList = new();
    public RxProperty<int> exampleInt = new();

    public void InitializeData()
    {

    }

    public class ExampleDataInnerDataContainer : RxBase
    {
        public RxProperty<int> exampleValue = new(0);
    }
}