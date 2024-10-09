using System;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public interface ICommandDefinition
{
    string Identifier { get; }
    public int GetCommandParameterCount();
    public bool CheckIdentifier(string identifier);
    public void Execute(params object[] parameters);
    public bool TryConvertParameter(int index, string input, out object result);
}

// TODO : Check for optional variables by using parameters default
public class CommandDefinition<IDelegate> : ICommandDefinition where IDelegate : Delegate
{
    string m_identifier;
    IDelegate m_commandAction;
    ParameterInfo[] m_parameterInfo;

    public string Identifier => m_identifier;

    public CommandDefinition(string identifier, IDelegate command)
    {
        m_identifier = identifier;
        m_commandAction = command;
        m_parameterInfo = command.GetMethodInfo().GetParameters();
    }


    public bool CheckIdentifier(string identifier)
    {
        // TODO : Hash the strings for better comparaison
        return m_identifier == identifier;
    }

    public int GetCommandParameterCount()
    {
        return m_parameterInfo.Length;
    }

    public bool TryConvertParameter(int index, string input, out object result)
    {
        result = null;

        if (index < 0 || index >= m_parameterInfo.Length)
        {
            Debug.LogError("Index outside of parameter info array!");
            return false;
        }

        var paramInfo = m_parameterInfo[index];
        return Compare(paramInfo.ParameterType, input, out result);
    }

    private static bool Compare(Type type, string input, out object output)
    {
        Regex regex;
        output = null;

        if (type == null)
        {
            return false;
        }
        else if (type == typeof(int))
        {
            regex = new Regex("^-?\\d+$");
            if (regex.IsMatch(input))
            {
                output = int.Parse(input);
                return true;
            }
        }
        else if (type == typeof(bool))
        {
            regex = new Regex("^(?i)(true|false)$");
            if (regex.IsMatch(input))
            {
                output = input.Equals("true", StringComparison.OrdinalIgnoreCase);
                return true;
            }
        }
        else if (type == typeof(string))
        {
            output = input;
            return true;
        }

        return false;
    }

    public void Execute(params object[] parameters)
    {
        m_commandAction?.DynamicInvoke(parameters);
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"{Identifier}");
        sb.Append("(");

        int index = 0;
        foreach (var param in m_parameterInfo)
        {
            string type = "error";
            if (param.ParameterType == typeof(string))
            {
                type = "string";
            }
            else if (param.ParameterType == typeof(int))
            {
                type = "int";
            }
            else if (param.ParameterType == typeof(bool))
            {
                type = "bool";
            }

            sb.Append($"{type}");
            if (index < m_parameterInfo.Length - 1)
            {
                sb.Append(", ");
            }
            index++;
        }
        sb.Append(")");

        return sb.ToString();
    }
}
