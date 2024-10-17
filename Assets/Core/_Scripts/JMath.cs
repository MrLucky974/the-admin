using System.Collections.Generic;

public struct JMath
{
    public static int Min(List<int> values)
    {
        int minValue = int.MaxValue;

        for (int i = 0; i < values.Count; i++)
        {
            var value = values[i];
            if (value < minValue) minValue = value;
        }

        return minValue;
    }

    public static int Min(params int[] values)
    {
        int minValue = int.MaxValue;

        for (int i = 0; i < values.Length; i++)
        {
            var value = values[i];
            if (value < minValue) minValue = value;
        }

        return minValue;
    }

    public static int Max(params int[] values)
    {
        int maxValue = int.MinValue;

        for (int i = 0; i < values.Length; i++)
        {
            var value = values[i];
            if (value > maxValue) maxValue = value;
        }

        return maxValue;
    }
}
