// TODO: Add support for aliases (e.g. help -> hl, checkup -> chk)
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

// TODO: Add support for aliases (e.g. help -> hl, checkup -> chk)
public class CommandSystem : MonoBehaviour
{
    private readonly List<ICommandDefinition> m_commands = new List<ICommandDefinition>();

    private static readonly Dictionary<Type, CommandTypeHandler> TypeHandlers = new Dictionary<Type, CommandTypeHandler>()
    {
        { typeof(int), new CommandTypeHandler(
            input =>
            {
                var regex = new Regex("^-?\\d+$");
                if (regex.IsMatch(input))
                {
                    return int.Parse(input, CultureInfo.InvariantCulture);
                }
                throw new FormatException("Invalid integer format.");
            },
            "int")
        },
        { typeof(float), new CommandTypeHandler(
            input =>
            {
                var regex = new Regex("^-?\\d*(\\.\\d+)?$");
                if (regex.IsMatch(input))
                {
                    return float.Parse(input, CultureInfo.InvariantCulture);
                }
                throw new FormatException("Invalid float format.");
            },
            "float")
        },
        { typeof(bool), new CommandTypeHandler(
            input =>
            {
                var regex = new Regex("^(?i)(true|false)$");
                if (regex.IsMatch(input))
                {
                    return input.Equals("true", StringComparison.OrdinalIgnoreCase);
                }
                throw new FormatException("Invalid boolean format.");
            },
            "bool")
        },
        { typeof(string), new CommandTypeHandler(
            input => input,
            "string")
        }
    };

    public IEnumerable<(string identifier, string description)> GetCommandsList()
    {
        foreach (var command in m_commands)
        {
            yield return (command.ToString(), command.Description);
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

    // Type handling methods
    public static bool TryGetTypeParser(Type type, out Func<string, object> parser)
    {
        parser = null;
        if (TypeHandlers.TryGetValue(type, out var handler))
        {
            parser = handler.Parser;
            return true;
        }
        return false;
    }

    public static string GetTypeString(Type type)
    {
        return TypeHandlers.TryGetValue(type, out var handler) ? handler.TypeString : "error";
    }

    public static void AddCustomType(Type type, Func<string, object> parser, string typeString)
    {
        if (TypeHandlers.ContainsKey(type))
        {
            Debug.LogError($"Type '{type}' is already registered.");
            return;
        }
        TypeHandlers[type] = new CommandTypeHandler(parser, typeString);
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
