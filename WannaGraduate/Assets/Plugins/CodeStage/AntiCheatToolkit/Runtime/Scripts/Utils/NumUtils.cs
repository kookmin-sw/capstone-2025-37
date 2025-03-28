#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;

namespace CodeStage.AntiCheat.Utils
{
    internal static class NumUtils
    {
        public static bool CompareWithEpsilon(float f1, float f2, float epsilon = float.Epsilon)
        {
            return f1.Equals(f2) || Math.Abs(f1 - f2) < epsilon;
        }
		
		public static bool CompareWithEpsilon(double a, double b, double epsilon = double.Epsilon)
		{
			const double lowestMeaningful = 2.2250738585072014E-308d;
			var absA = Math.Abs(a);
			var absB = Math.Abs(b);
			var diff = Math.Abs(a - b);

			if (a.Equals(b)) // for infinities and NaNs
				return true;
        
			if (a == 0 || b == 0 || absA + absB < lowestMeaningful) // for very small numbers
				return diff < epsilon * lowestMeaningful;
        
			return diff / (absA + absB) < epsilon;
		}
    }
}