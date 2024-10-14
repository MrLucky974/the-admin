using System;
using System.Reflection;
using System.Text;
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
        if (CommandSystem.TryGetTypeParser(paramInfo.ParameterType, out var parser))
        {
            try
            {
                result = parser(input);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
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

        for (int i = 0; i < m_parameterInfo.Length; i++)
        {
            var param = m_parameterInfo[i];
            string typeString = CommandSystem.GetTypeString(param.ParameterType);
            sb.Append($"{typeString}");

            if (i < m_parameterInfo.Length - 1)
            {
                sb.Append(", ");
            }
        }
        sb.Append(")");

        return sb.ToString();
    }
}
