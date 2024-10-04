using UnityEngine;

public class CommandLinePrompt : MonoBehaviour
{
    private static readonly char[] separators = new char[]
    {
        ' '
    };

    public void ParseCommand(string input)
    {
        string[] parts = input.Split(separators);
    }
}
