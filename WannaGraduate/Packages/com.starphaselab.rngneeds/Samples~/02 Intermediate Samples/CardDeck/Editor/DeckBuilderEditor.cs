using UnityEditor;
using UnityEngine;

namespace RNGNeeds.Samples.CardDeck.Editor
{
    [CustomEditor(typeof(DeckBuilder))]
    public class DeckBuilderEditor : UnityEditor.Editor
    {
        private SerializedProperty m_DeckBuilderProperty;
        
        private DeckBuilder m_DeckBuilder;
        private readonly Color separatorColor = new Color(.5f, .6f, .7f);
        private GUIStyle m_TempStyle;
        private Rect lightSkinBackgroundRect;
        private Texture2D lightSkinBackgroundTexture;
        
        private void OnEnable()
        {
            m_DeckBuilder = (DeckBuilder)serializedObject.targetObject;
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
            
            m_DeckBuilderProperty = serializedObject.GetIterator();
            if (m_DeckBuilderProperty.NextVisible(true))
            {
                do
                {
                    if(m_DeckBuilderProperty.name == "m_Script") continue;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(m_DeckBuilderProperty.name), true);
                }
                while (m_DeckBuilderProperty.NextVisible(false));
            }
            serializedObject.ApplyModifiedProperties();


            var deckToFillSet = m_DeckBuilder.deckToFill != null;
            EditorGUI.BeginDisabledGroup(!deckToFillSet);
            if (GUILayout.Button("Fill Deck", GUILayout.Height(30f)))
            {
                m_DeckBuilder.FillDeck();
                EditorUtility.SetDirty(m_DeckBuilder.deckToFill);
                AssetDatabase.SaveAssets();
            }
            EditorGUI.EndDisabledGroup();
            
            if (deckToFillSet == false) return;
            
            DrawSeparator();
            DrawDeckContents();
            DrawSeparator();
        }
        
        private void DrawSeparator()
        {
            var controlRect = EditorGUILayout.GetControlRect(true, 4f);
            GUI.BeginGroup(controlRect);
            EditorGUI.DrawRect(new Rect(0f, 2f, controlRect.xMax - 12f, 1f), separatorColor);
            GUI.EndGroup();
        }

        private void DrawDeckContents()
        {
            for (var i = 0; i < m_DeckBuilder.deckToFill.cards.ItemCount; i++)
            {
                var card = m_DeckBuilder.deckToFill.cards.GetProbabilityItem(i).Value;
                var color = card.ItemColor;
                m_TempStyle.normal.textColor = color;
                EditorGUILayout.LabelField(card.name, m_TempStyle);
            }
        }
    }
}