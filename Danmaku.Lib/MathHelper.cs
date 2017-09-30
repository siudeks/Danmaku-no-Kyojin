namespace Danmaku
{
    // class copied from 
    // https://github.com/MonoGame/MonoGame/blob/6c62eb45ada285147eb4ecc414b86f3537655771/MonoGame.Framework/MathHelper.cs
    public static class MathHelper
    {
        //
        // Summary:
        //     Represents the value of pi divided by two(1.57079637).
        public const float PiOver2 = 1.57079637F;

        /// <summary>
        /// Restricts a value to be within a specified range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">The minimum value. If <c>value</c> is less than <c>min</c>, <c>min</c> will be returned.</param>
        /// <param name="max">The maximum value. If <c>value</c> is greater than <c>max</c>, <c>max</c> will be returned.</param>
        /// <returns>The clamped value.</returns>
        public static float Clamp(float value, float min, float max)
        {
            // First we check to see if we're greater than the max
            value = (value > max) ? max : value;

            // Then we check to see if we're less than the min.
            value = (value < min) ? min : value;

            // There's no check to see if min > max.
            return value;
        }
    }
}
