using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utiliities
{
    //TODO: better names
    //TODO: xml comments
    static class Methods
    {
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }
        public static bool IsBetween(this double val, double bound1, double bound2, bool includeEndpoints)
        {
            if (includeEndpoints)
            {
                return bound1 > bound2 ? (val <= bound1 && val >= bound2) : (val <= bound2 && val >= bound1);
            }
            else
            {
                return bound1 > bound2 ? (val < bound1 && val > bound2) : (val < bound2 && val > bound1);
            }
        }

        /// <summary>
        /// Use this in place of "==" to account for floating point rounding error
        /// </summary>
        public static bool IsPrettyCloseTo(this double num1, double num2, float minDiff)
        {
            return Math.Abs(num1 - num2) <= minDiff;
        }

        public static int PMod(this int dividend, int divisor)
        {
            return ((dividend % divisor) + divisor) % divisor;
        }

        public static double PMod(this double dividend, double divisor)
        {
            return ((dividend % divisor) + divisor) % divisor;
        }






        //SOURCE: http://stackoverflow.com/a/1014015
        public static void Fill<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }

        public static void Fill<T>(this T[,] array, T value)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] = value;
                }
            }
        }
    }
}
