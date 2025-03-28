using UnityEditor;
using UnityEngine;

namespace RNGNeeds.Editor
{
    internal static class OptionSliders
    {
        private const float HandleSize = 8f;
        
        public static float ValueSlider(Rect position, GUIContent label, float value, float minValue, float maxValue, bool disabled = false)
        {
            EditorGUI.LabelField(new Rect(position.x, position.y, position.width * 0.4f, position.height), label, PLDrawerTheme.AdvancedOptionsLabelStyle);
            
            var sliderBackgroundRect = new Rect(position.x + position.width * 0.4f, position.y, position.width * 0.6f, position.height);
            EditorGUI.DrawRect(sliderBackgroundRect, disabled ? PLDrawerTheme.SliderDisabledBackgroundColor : PLDrawerTheme.SliderBackgroundColor);
            
            if(Event.current.type == EventType.Repaint) if (disabled) PLDrawerTheme.RectBorderDisabled.Draw(sliderBackgroundRect, false, false, false, false); 
                else PLDrawerTheme.RectBorderAccent.Draw(sliderBackgroundRect, false, false, false, false);

            var normalizedValue = Mathf.InverseLerp(minValue, maxValue, value);
            var handlePosition = Mathf.Lerp(sliderBackgroundRect.xMin + 2f + HandleSize / 2, sliderBackgroundRect.xMax - 2f - HandleSize / 2, normalizedValue);

            var pixelFix = EditorGUIUtility.pixelsPerPoint > 1f ? 5f : 4f;
            var handleRect = new Rect(handlePosition - HandleSize * 0.5f, position.y + 2f, HandleSize, position.height - pixelFix);
            EditorGUI.DrawRect(handleRect, disabled ? PLDrawerTheme.SliderHandleDisabledColor : PLDrawerTheme.SliderHandleColor);

            var controlId = GUIUtility.GetControlID(FocusType.Passive);

            if (Event.current.type == EventType.MouseDown && sliderBackgroundRect.Contains(Event.current.mousePosition))
            {
                GUIUtility.hotControl = controlId;

                var mouseX = Event.current.mousePosition.x;
                mouseX = Mathf.Clamp(mouseX, sliderBackgroundRect.xMin, sliderBackgroundRect.xMax);

                normalizedValue = Mathf.InverseLerp(sliderBackgroundRect.xMin, sliderBackgroundRect.xMax, mouseX);
                value = Mathf.Lerp(minValue, maxValue, normalizedValue);

                GUI.changed = true;
                Event.current.Use();
            }
            else if (Event.current.type == EventType.MouseDrag && GUIUtility.hotControl == controlId)
            {
                var mouseX = Event.current.mousePosition.x;
                mouseX = Mathf.Clamp(mouseX, sliderBackgroundRect.xMin, sliderBackgroundRect.xMax);

                normalizedValue = Mathf.InverseLerp(sliderBackgroundRect.xMin, sliderBackgroundRect.xMax, mouseX);
                value = Mathf.Lerp(minValue, maxValue, normalizedValue);

                GUI.changed = true;
                Event.current.Use();
            }
            else if (Event.current.type == EventType.MouseUp && GUIUtility.hotControl == controlId)
            {
                GUIUtility.hotControl = 0;
                Event.current.Use();
            }

            return value;
        }

        public static int ValueSlider(Rect position, GUIContent label, int value, int minValue, int maxValue, bool disabled = false)
        {
            return Mathf.RoundToInt(ValueSlider(position, label, (float)value, minValue, maxValue, disabled));
        }
    }
}