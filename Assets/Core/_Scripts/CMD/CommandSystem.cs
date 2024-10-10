using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: Add support for aliases (e.g. help -> hl, checkup -> chk)
public class CommandSystem : MonoBehaviour
{
    private readonly List<ICommandDefinition> m_commands = new List<ICommandDefinition>();

    public IEnumerable<(string identifier, string description)> GetCommandHelp()
    {
        foreach (var command in m_commands)
        {
            yield return (command.ToString(), "Lorem ipsum dolor sit amet!");
        }
    }

    public void AddCommand(ICommandDefinition command)
    {
        bool alreadyHasCommandName = m_commands.Any((cmd) => cmd.CheckIdentifier(command.Identifier));
        if (alreadyHasCommandName)
        {
            Debug.LogError($"Command already registered under identifier \'{command.Identifier}\'");
            return;
        }
        m_commands.Add(command);
    }

    private static readonly char[] separators = new char[]
    {
        ' '
    };

    private CommandLogManager m_commandLog;

    private void Awake()
    {
        m_commands.Clear();
    }

    private void Start()
    {
        m_commandLog = GameManager.Instance.GetCommandLog();
    }

    public void ParseCommand(string input)
    {
        // EXAMPLE: "checkup A1" -> ["checkup", "A1"]
        Debug.Log($"Parsing command: {input}");
        m_commandLog.AddLog($"command: {input}");
        
        string[] parts = input.Split(separators);

        // TODO: Search & call relevent command
        var identifier = parts[0];

        bool commandFound = false;
        object[] parameters;
        foreach (var command in m_commands)
        {
            if (command.CheckIdentifier(identifier))
            {
                commandFound = true;

                int paramCount = command.GetCommandParameterCount();
                parameters = new object[paramCount];

                // Check if parameter count matches
                if ((parts.Length - 1) != paramCount)
                {
                    Debug.LogError("Parameter count doesn't match.");
                    m_commandLog.AddLog($"error: incorrect parameter count (required: {paramCount} | given: {parts.Length - 1})", GameManager.RED);
                    SoundManager.PlaySound(SoundType.ERROR);
                    break;
                }

                bool canExecute = true;
                int paramIndex;
                for (paramIndex = 1; paramIndex < parts.Length; paramIndex++)
                {
                    var inputParameter = parts[paramIndex];
                    if (!command.TryConvertParameter(paramIndex - 1, inputParameter, out var result))
                    {
                        canExecute = false;
                        break;
                    }

                    parameters[paramIndex - 1] = result;
                }

                if (!canExecute)
                {
                    Debug.LogError("Invalid parameter type.");
                    m_commandLog.AddLog($"error: invalid parameter type at index {paramIndex}", GameManager.RED);
                    SoundManager.PlaySound(SoundType.ERROR);
                    break;
                }

                command.Execute(parameters);
                break;
            }
        }

        if (!commandFound)
        {
            Debug.LogError("No command found.");
            m_commandLog.AddLog($"error: invalid command identifier", GameManager.RED);
            SoundManager.PlaySound(SoundType.ERROR);
        }
    }
}
