using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct JRandom
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
}
