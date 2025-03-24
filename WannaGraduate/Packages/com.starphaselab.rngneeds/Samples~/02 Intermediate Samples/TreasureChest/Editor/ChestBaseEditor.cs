using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RNGNeeds.Samples.TreasureChest.Editor
{
    [CustomEditor(typeof(ChestBase), true)]
    public class ChestBaseEditor : UnityEditor.Editor
    {
        private SerializedProperty p_ChestProperty;
        private List<Item> m_ChestContents;
        
        private ChestBase m_Chest;
        private readonly Color separatorColor = new Color(.5f, .6f, .7f);
        private GUIStyle m_TempStyle;
        private Rect lightSkinBackgroundRect;
        private Texture2D lightSkinBackgroundTexture;
        
        private void OnEnable()
        {
            m_Chest = (ChestBase)serializedObject.targetObject;
            m_ChestContents = new List<Item>();
            m_TempStyle = new GUIStyle();
            m_TempStyle.padding = new RectOffset(10, 0, 2, 0);
            if (EditorGUIUtility.isProSkin == false)
            {
                lightSkinBackgroundTexture = new Texture2D(1, 1);
                lightSkinBackgroundTexture.SetPixel(0, 0, new Color(.32f, .32f, .32f, 1f));
                lightSkinBackgroundTexture.Apply();
                m_TempStyle.normal.background = lightSkinBackgroundTexture;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (GUILayout.Button("Open Chest", GUILayout.Height(30f))) m_ChestContents = m_Chest.OpenChest();
            DrawSeparator();
            if (m_ChestContents.Count > 0)
            {
                DrawChestContents();
                DrawSeparator();
            }

            p_ChestProperty = serializedObject.GetIterator();
            if (p_ChestProperty.NextVisible(true))
            {
                do
                {
                    if(p_ChestProperty.name == "m_Script") continue;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(p_ChestProperty.name), true);
                }
                while (p_ChestProperty.NextVisible(false));
            }
            serializedObject.ApplyModifiedProperties();
        }
        
        private void DrawSeparator()
        {
            var controlRect = EditorGUILayout.GetControlRect(true, 4f);
            GUI.BeginGroup(controlRect);
            EditorGUI.DrawRect(new Rect(0f, 2f, controlRect.xMax - 12f, 1f), separatorColor);
            GUI.EndGroup();
        }

        private void DrawChestContents()
        {
            foreach (var item in m_ChestContents)
            {
                var color = item.ItemColor;
                m_TempStyle.normal.textColor = color;
                EditorGUILayout.LabelField(item.ItemDescription, m_TempStyle);
            }
        }
    }
}