using LuckiusDev.Utils;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class GameData : PersistentSingleton<GameData>
{
    private string m_identifier = string.Empty;
    public static string Identifier
    {
        get => Instance.m_identifier;
    }

    private int m_seed = 0;
    public static int Seed
    {
        get => Instance.m_seed;
    }

    public static void SetSeed(int seed)
    {
        Instance.m_seed = seed;
    }

    public static void SetIdentifier(string identifier)
    {
        Instance.m_identifier = identifier;
    }
}
