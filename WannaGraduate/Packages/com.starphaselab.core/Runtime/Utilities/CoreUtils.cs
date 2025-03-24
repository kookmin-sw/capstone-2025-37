using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace StarphaseTools.Core
{
    public static class CoreUtils
    {
        public static string FormatElapsedTime(Stopwatch stopwatch)
        {
            return FormatElapsedTime(stopwatch.Elapsed.TotalMilliseconds);
        }
        
        public static string FormatElapsedTime(double elapsedMilliseconds)
        {
            var elapsedSeconds = elapsedMilliseconds / 1000;
            var elapsedMinutes = elapsedSeconds / 60;
            var elapsedHours = elapsedMinutes / 60;

            if (elapsedMilliseconds < 1000) return $"{elapsedMilliseconds:0.##} ms";
            if (elapsedSeconds < 60) return $"{elapsedSeconds:0.####} s";
            if (elapsedMinutes < 60) return $"{elapsedMinutes:0.####} min";
            return $"{elapsedHours:0.####} h";
        }
        
        public static float Remap (this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
        
        public static int PickRandomInt(this AnimationCurve curve, int minValue, int maxValue)
        {
            return Mathf.Clamp(Mathf.RoundToInt(Mathf.Lerp(minValue, maxValue, curve.Evaluate(Random.value))), minValue, maxValue);
        }
        
        public static int PickRandomInt(this AnimationCurve curve, int minValue, int maxValue, uint seed)
        {
            var random = new Unity.Mathematics.Random(seed);
            var randomValue = random.NextFloat();
            return Mathf.Clamp(Mathf.RoundToInt(Mathf.Lerp(minValue, maxValue, curve.Evaluate(randomValue))), minValue, maxValue);
        }
        
        public static bool HasAny<T>(this T flags, T check) where T : Enum
        {
            var flagsValue = (int)(object)flags;
            var checkValue = (int)(object)check;
            return (flagsValue & checkValue) != 0;
        }
        
        public static bool HasAll<T>(this T flags, T check) where T : Enum
        {
            var flagsValue = (int)(object)flags;
            var checkValue = (int)(object)check;
            return (flagsValue & checkValue) == checkValue;
        }

        public static bool CompareValues(CompareMethod compareMethod, float a, float b)
        {
            switch (compareMethod)
            {
                case CompareMethod.GreaterThan:
                    return a > b;
                case CompareMethod.GreaterOrEqual:
                    return a >= b;
                case CompareMethod.LessThan:
                    return a < b;
                case CompareMethod.LessOrEqual:
                    return a <= b;
                case CompareMethod.EqualTo:
                    return Mathf.Approximately(a, b);
                default:
                    return false;
            }
        }
    }
}