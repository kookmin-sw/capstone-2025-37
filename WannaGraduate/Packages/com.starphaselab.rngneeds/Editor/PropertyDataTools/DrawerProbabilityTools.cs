using System;
using UnityEditor;
using UnityEngine;

namespace RNGNeeds.Editor
{
    internal static class DrawerProbabilityTools
    {
        internal static float GetBaseProbability(this PropertyData propertyData, int index)
        {
            return propertyData.ProbabilityListEditorInterface.GetItemBaseProbability(index);
        }

        private static void AdjustProbability(this PropertyData propertyData, int index, float amount)
        {
            propertyData.ProbabilityListEditorInterface.AdjustItemBaseProbability(index, amount);
            propertyData.SetPercentageCacheFor(index);
        }

        private static bool WouldProbabilityFallBelowZero(this PropertyData propertyData, int index, float amount)
        {
            return propertyData.GetBaseProbability(index) - amount <= 0;
        }

        internal static float MaxProbability(this PropertyData propertyData)
        {
            var highestProbability = 0f;
            for (var i = 0; i < propertyData.p_ProbabilityItems.arraySize; i++)
            {
                var probability = propertyData.GetBaseProbability(i);
                if (probability > highestProbability) highestProbability = probability;
            }

            return highestProbability;
        }
        
        internal static float ModifyProbabilities(this PropertyData propertyData, Event m_CurrentEvent, float m_GrabPoint, Rect m_StripeRect)
        {
            var fractionChange = ((m_CurrentEvent.mousePosition.x - m_GrabPoint) * 2) / m_StripeRect.width / 2;
            m_GrabPoint = m_CurrentEvent.mousePosition.x;
    
            int leftProbabilityIndex;
            var rightProbabilityIndex = propertyData.SelectedModifier + 1;

            switch (propertyData.ModifierType)
            {
                case ModifierType.ModifierRect:
                    leftProbabilityIndex = propertyData.SelectedModifier;
                    break;
                case ModifierType.ProbabilityRect:
                    leftProbabilityIndex = propertyData.SelectedModifier - 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var leftProbability = propertyData.GetBaseProbability(leftProbabilityIndex);
            var rightProbability = propertyData.GetBaseProbability(rightProbabilityIndex);

            var clampedFractionChange = Mathf.Clamp(fractionChange, -leftProbability, rightProbability);
            propertyData.AdjustProbability(leftProbabilityIndex, clampedFractionChange);
            propertyData.AdjustProbability(rightProbabilityIndex, -clampedFractionChange);

            if (propertyData.IsInfluencedList) propertyData.SetSpreadCache();
            return m_GrabPoint;
        }
        
        internal static void ModifyProbabilityViaScrollWheel(this PropertyData propertyData, int shiftRectIndex, float amount, bool invertScrollDirection)
        {
            if (propertyData.ItemPropertyCache[shiftRectIndex].p_Locked.boolValue) return;
            amount = invertScrollDirection ? amount : amount * -1;
        
            if (shiftRectIndex == 0)
            {
                if (propertyData.ItemPropertyCache[1].p_Locked.boolValue) return;
                if (propertyData.WouldProbabilityFallBelowZero(1, -amount)) return;
                propertyData.AdjustProbability(shiftRectIndex, -amount);
                propertyData.AdjustProbability(1, amount);
            } else if (shiftRectIndex == propertyData.p_ProbabilityItems.arraySize - 1)
            {
                if (propertyData.ItemPropertyCache[shiftRectIndex - 1].p_Locked.boolValue) return;
                if (propertyData.WouldProbabilityFallBelowZero(shiftRectIndex - 1, -amount)) return;
                propertyData.AdjustProbability(shiftRectIndex, -amount);
                propertyData.AdjustProbability(shiftRectIndex - 1, amount);
            }
            else
            {
                var leftLocked = propertyData.ItemPropertyCache[shiftRectIndex - 1].p_Locked.boolValue;
                var rightLocked = propertyData.ItemPropertyCache[shiftRectIndex + 1].p_Locked.boolValue;
                if (leftLocked && rightLocked) return;
                var divider = leftLocked || rightLocked ? 1 : 2;
                if (propertyData.WouldProbabilityFallBelowZero(shiftRectIndex - 1, -amount / divider)
                    || propertyData.WouldProbabilityFallBelowZero(shiftRectIndex + 1, -amount / divider))
                    return;
        
                propertyData.AdjustProbability(shiftRectIndex, -amount);
            
                if (leftLocked == false) propertyData.AdjustProbability(shiftRectIndex - 1, amount / divider);
                if (rightLocked == false) propertyData.AdjustProbability(shiftRectIndex + 1, amount / divider);
            }
            
            if (propertyData.IsInfluencedList) propertyData.SetSpreadCache();
        }

        internal static void EvenOutProbabilities(this PropertyData propertyData, int modifierRectIndex)
        {
            Undo.RecordObject(propertyData.p_ProbabilityListProperty.serializedObject.targetObject, $"Even out Items {modifierRectIndex} and {modifierRectIndex + 1}");
            var combined = propertyData.GetBaseProbability(modifierRectIndex) + propertyData.GetBaseProbability(modifierRectIndex + 1);
            propertyData.ProbabilityListEditorInterface.SetItemBaseProbability(modifierRectIndex, combined * .5f, false);
            propertyData.ProbabilityListEditorInterface.SetItemBaseProbability(modifierRectIndex + 1, combined * .5f, false);
            propertyData.SetPercentageCacheFor(modifierRectIndex);
            propertyData.SetPercentageCacheFor(modifierRectIndex + 1);
            if (propertyData.IsInfluencedList) propertyData.SetSpreadCache();
            EditorUtility.SetDirty(propertyData.p_ProbabilityListProperty.serializedObject.targetObject);
        }
    }
}