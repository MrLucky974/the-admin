public static class JRandom
{
    public static int RollDice(int n, int l, int s, System.Random random)
    {
        // Sum of N dice each of which goes from L to S
        var value = 0;
        for (int i = 0; i < n; i++)
        {
            value += random.Next(l, s + 1);
        }
        return value;
    }

    public static int RollDice(int n, int s, System.Random random)
    {
        return RollDice(n, 0, s, random);
    }

    public static int RollDice(int n, int s, int seed)
    {
        return RollDice(n, s, new System.Random(seed));
    }

    public static int RollDice(int n, int s)
    {
        return RollDice(n, s, 0);
    }

    public static T PickRandom<T>(this T[] array, System.Random random)
    {
        int index = random.Next(array.Length);
        return array[index];
    }

    public static T PickRandom<T>(this T[] array, int seed)
    {
        return PickRandom<T>(array, new System.Random(seed));
    }

    public static T PickRandom<T>(this T[] array)
    {
        return PickRandom<T>(array, 0);
    }

    public static double NextDouble(this System.Random random, double max)
    {
        double value = random.NextDouble();
        return value * max;
    }

    public static double NextFloat(this System.Random random, double min, double max)
    {
        double value = random.NextDouble();
        return min + (max - min) * value;
    }

    public static float NextFloat(this System.Random random)
    {
        return (float)random.NextDouble();
    }

    public static float NextFloat(this System.Random random, float max)
    {
        float value = (float)random.NextDouble();
        return value * max;
    }

    public static float NextFloat(this System.Random random, float min, float max)
    {
        float value = (float)random.NextDouble();
        return min + (max - min) * value;
    }
}
