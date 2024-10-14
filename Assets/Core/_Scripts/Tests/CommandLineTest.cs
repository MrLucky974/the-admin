using UnityEngine;

public class CommandLineTest : MonoBehaviour
{
    //public delegate void Debug1Delegate(string testA);
    //public delegate void Debug2Delegate(int value);
    //public delegate void Debug3Delegate(string value1, string value2, string value3);

    private CommandSystem m_commandSystem;

    private void Awake()
    {
        m_commandSystem = GameManager.Instance.GetCommands();

        //m_commandSystem.AddCommand(new CommandDefinition<Debug1Delegate>("debug", (string a) =>
        //{
        //    Debug.Log(a);
        //}));

        //m_commandSystem.AddCommand(new CommandDefinition<Debug2Delegate>("value", (int v) =>
        //{
        //    Debug.Log($"Integer value: {v}");
        //}));

        //m_commandSystem.AddCommand(new CommandDefinition<Debug3Delegate>("multiple", (string value1, string value2, string value3) =>
        //{
        //    Debug.Log($"{value1}, {value2}, {value3}");
        //}));
    }

    private void Start()
    {
        //CommandLinePrompt.ParseCommand("debug tremolo");
        //CommandLinePrompt.ParseCommand("value 1000000000");
        //CommandLinePrompt.ParseCommand("value -10");
        //CommandLinePrompt.ParseCommand("value 3.45"); // Invalid parameter type.
        //CommandLinePrompt.ParseCommand("value 5");
        //CommandLinePrompt.ParseCommand("value 0");
        //CommandLinePrompt.ParseCommand("multiple 0 hello world");
        //CommandLinePrompt.ParseCommand("multiple && test -100");
        //CommandLinePrompt.ParseCommand("multiple &&"); // Parameter count doesn't match.
        //CommandLinePrompt.ParseCommand("multiple quoi coubeh"); // Parameter count doesn't match.
    }
}
