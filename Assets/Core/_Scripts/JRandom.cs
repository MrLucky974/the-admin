using System.Collections.Generic;

using UnityEngine;

public static class JRandom
{
    public class WeightSumGenerator<T>
    {
        private readonly Dictionary<T, int> values = new();

        public void Add(T element, int value)
        {
            if (values.ContainsKey(element))
            {
                values[element] = value;
            }
            else
            {
                values.Add(element, value);
            }
        }

        public T Generate(int seed = 0)
        {
            var rng = new System.Random(seed);
            return Generate(rng);
        }

        public T Generate(System.Random rng)
        {
            int weightedSum = 0;
            foreach (var (_, chance) in values)
            {
                weightedSum += chance;
            }

            int r = rng.Next(weightedSum);
            foreach (var (value, chance) in values)
            {
                if (r < chance && r > 0)
                {
                    return value;
                }

                r -= chance;
            }

            return default;
        }
    }

    private const string ALPHANUMERIC_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public static string GenerateRandomString(int size)
    {
        char[] stringChars = new char[size];
        for (int i = 0; i < size; i++)
        {
            stringChars[i] = ALPHANUMERIC_CHARACTERS[Random.Range(0, ALPHANUMERIC_CHARACTERS.Length)];
        }

        return new string(stringChars);
    }

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

    public static T PickRandom<T>(this List<T> list, System.Random random)
    {
        int index = random.Next(list.Count);
        return list[index];
    }

    public static T PickRandom<T>(this List<T> list, int seed)
    {
        return PickRandom<T>(list, new System.Random(seed));
    }

    public static T PickRandom<T>(this List<T> list)
    {
        return PickRandom<T>(list, 0);
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
