namespace Metalogic.DataUtil
{
    public static class MathTools
    {
        public static double IncheToMeter(this double inche)
        {
            return inche * 0.0254d;
        }

        public static double MeterToInche(this double meter)
        {
            return meter * 39.3700787d;
        }

        public static bool IsOverlap(double xStart, double xEnd, double yStart, double yEnd)
        {
            return yStart <= xStart && xStart <= yEnd
                   || yStart <= xEnd && xEnd <= yEnd
                   || xStart <= yStart && yStart <= xEnd
                   || xStart <= yEnd && yEnd <= xEnd;
        }

        public static void SwapSmallestToFirst(ref double firstValue, ref double secondValue)
        {
            if (firstValue > secondValue)
            {
                var middle = firstValue;
                firstValue = secondValue;
                secondValue = middle;
            }
        }
    }
}
