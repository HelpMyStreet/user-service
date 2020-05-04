using System;

namespace UserService.Core.Extensions
{
    public static class MathExtensions
    {
        public static int RoundUpToNearest(this int i, int nearest)
        {
            if (nearest <= 0 || nearest % 10 != 0)
                throw new ArgumentOutOfRangeException("nearest", "Must round to a positive multiple of 10");

            if (i % nearest == 0)
            {
                return i;
            }

            i = i + (nearest / 2) - 1;

            return (i + 5 * nearest / 10) / nearest * nearest;
        }
    }
}
