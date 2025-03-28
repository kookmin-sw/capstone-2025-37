using System;
using System.Collections.Generic;
using UnityEngine;

namespace RNGNeeds.Editor
{
    [Serializable]
    public class ProbabilityListColorPalette
    {
        public string palettePath;
        [ColorUsage(false)]
        public List<Color> colors;

        internal void SetDefaultAlpha()
        {
            for (var i = 0; i < colors.Count; i++)
            {
                var fixedAlpha = colors[i];
                fixedAlpha.a = 1f;
                colors[i] = fixedAlpha;
            }
        }
    }
}