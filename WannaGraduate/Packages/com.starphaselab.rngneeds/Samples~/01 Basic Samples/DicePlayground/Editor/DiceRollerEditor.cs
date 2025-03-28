using UnityEditor;
using UnityEngine;

namespace RNGNeeds.Samples.DicePlayground.Editor
{
    [CustomEditor(typeof(DiceRoller))]
    public class DiceRollerEditor : UnityEditor.Editor
    {
        private SerializedProperty p_DiceToRoll;
        private SerializedProperty p_RollResults;
        private DiceRoller m_DiceRoller;

        private readonly Color separatorColor = new Color(.5f, .6f, .7f);
        private const float ResultRowHeight = 20f;

        private void OnEnable()
        {
            p_DiceToRoll = serializedObject.FindProperty("diceToRoll");
            p_RollResults = serializedObject.FindProperty("m_RollResults");
            m_DiceRoller = (DiceRoller)serializedObject.targetObject;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(p_DiceToRoll);
            serializedObject.ApplyModifiedProperties();
            
            EditorGUI.BeginDisabledGroup(CanWeRoll() == false);
            if (GUILayout.Button($"Roll {p_DiceToRoll.arraySize} Dice", GUILayout.Height(30f)))
            {
                m_DiceRoller.RollDice();
            }
            EditorGUI.EndDisabledGroup();

            DrawSeparator();
            DisplayResults();
        }

        private void DisplayResults()
        {
            // Results Table Header
            var controlRect = EditorGUILayout.GetControlRect(false, 20f);
            GUI.BeginGroup(controlRect);
            DrawResultLine(0, 20f, controlRect.width, "Die;Total;Min;Max;Picks;Rolled Values", EditorStyles.boldLabel);
            GUI.EndGroup();
            
            DrawSeparator();
            
            if (p_RollResults.arraySize < 1) return;

            // Results Table Rows
            controlRect = EditorGUILayout.GetControlRect(false, p_RollResults.arraySize * ResultRowHeight);
            GUI.BeginGroup(controlRect);
            var total = 0;
            for (var i = 0; i < p_RollResults.arraySize; i++)
            {
                var rollResults = p_RollResults.GetArrayElementAtIndex(i);
                var dieResult = GetRowData(rollResults);
                total += dieResult.dieTotal;
                DrawResultLine(i, ResultRowHeight, controlRect.width, GetRowData(rollResults).Info, EditorStyles.label);
            }
            GUI.EndGroup();
            
            DrawSeparator();
            
            // Results Total
            controlRect = EditorGUILayout.GetControlRect(false, 20f);
            GUI.BeginGroup(controlRect);
            DrawResultLine(0, 20f, controlRect.width, $"Total;{total};;;;", EditorStyles.boldLabel);
            GUI.EndGroup();
        }

        private static (string Info, int dieTotal) GetRowData(SerializedProperty rollResults)
        {
            var dieName = rollResults.FindPropertyRelative("dieName").stringValue;
            var pickCountMin = rollResults.FindPropertyRelative("pickCountMin").stringValue;
            var pickCountMax = rollResults.FindPropertyRelative("pickCountMax").stringValue;
            var rolls = rollResults.FindPropertyRelative("rolls");
            var rollsInfo = "";
            var dieTotal = 0;
            for (var i = 0; i < rolls.arraySize; i++)
            {
                var roll = rolls.GetArrayElementAtIndex(i).intValue;
                rollsInfo = $"{rollsInfo}{roll.ToString()}";
                if (i > 100) break;
                if (i < rolls.arraySize - 1) rollsInfo = $"{rollsInfo} + ";
                dieTotal += roll;
            }
            
            return ($"{dieName};{dieTotal.ToString()};{pickCountMin};{pickCountMax};{rolls.arraySize};{rollsInfo}", dieTotal);
        }

        private static void DrawResultLine(int row, float rowHeight, float width, string data, GUIStyle style)
        {
            var splitData = data.Split(';');
            var yPos = row * rowHeight;
            var colOne = new Rect(0f, yPos, 100f, rowHeight);
            var colTwo = new Rect(colOne.xMax + 4f, yPos, 50f, rowHeight);
            var colThree = new Rect(colTwo.xMax + 4f, yPos, 30f, rowHeight);
            var colFour = new Rect(colThree.xMax + 4f, yPos, 30f, rowHeight);
            var colFive = new Rect(colFour.xMax + 4f, yPos, 40f, rowHeight);
            var ColSix = new Rect(colFive.xMax + 4f, yPos, width - colFour.xMax, rowHeight);
            EditorGUI.LabelField(colOne, splitData[0], style);
            EditorGUI.LabelField(colTwo, splitData[1], style);
            EditorGUI.LabelField(colThree, splitData[2], style);
            EditorGUI.LabelField(colFour, splitData[3], style);
            EditorGUI.LabelField(colFive, splitData[4], style);
            EditorGUI.LabelField(ColSix, splitData[5], style);
        }

        private void DrawSeparator()
        {
            var controlRect = EditorGUILayout.GetControlRect(true, 4f);
            GUI.BeginGroup(controlRect);
            EditorGUI.DrawRect(new Rect(0f, 2f, controlRect.xMax - 12f, 1f), separatorColor);
            GUI.EndGroup();
        }
        
        private bool CanWeRoll()
        {
            if (p_DiceToRoll.arraySize < 1) return false;
            for (var i = 0; i < p_DiceToRoll.arraySize; i++)
            {
                if (p_DiceToRoll.GetArrayElementAtIndex(i).objectReferenceValue == null) return false;
            }

            return true;
        }
    }
}