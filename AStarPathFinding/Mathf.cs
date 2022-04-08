using System;

namespace UnityEngine
{
    public struct Mathf
    {
        public static float Clamp01(float value)
        {
            return (value - float.MinValue) / (float.MaxValue - float.MinValue);
        }

        public static int RoundToInt(float value) => (int)Math.Round(value);
    }

}
