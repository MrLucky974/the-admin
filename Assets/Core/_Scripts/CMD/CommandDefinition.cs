using System;
using System.Reflection;
using System.Text;
using UnityEngine;

public interface ICommandDefinition
{
    string Identifier { get; }
    string Description { get; }
    public int GetCommandParameterCount();
    public bool CheckIdentifier(string identifier);
    public void Execute(params object[] parameters);
    public bool TryConvertParameter(int index, string input, out object result);
}

public class CommandDefinition<IDelegate> : ICommandDefinition where IDelegate : Delegate
{
    private string m_identifier;
    private string m_description;
    private IDelegate m_commandAction;
    private ParameterInfo[] m_parameterInfo;

    public string Identifier => m_identifier;
    public string Description => m_description;

    public CommandDefinition(string identifier, IDelegate command)
    {
        m_identifier = identifier;
        m_commandAction = command;
        m_parameterInfo = command.GetMethodInfo().GetParameters();
    }

    public CommandDefinition(string identifier, string description, IDelegate command) : this(identifier, command)
    {
        m_description = description;
    }

    public bool CheckIdentifier(string identifier)
    {
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
        if (string.IsNullOrEmpty(input) && paramInfo.HasDefaultValue)
        {
            result = paramInfo.DefaultValue;
            return true;
        }

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

        if (!string.IsNullOrEmpty(Description))
        {
            sb.Append($" - {Description}");
        }

        sb.Append("(");

        for (int i = 0; i < m_parameterInfo.Length; i++)
        {
            var param = m_parameterInfo[i];
            string typeString = CommandSystem.GetTypeString(param.ParameterType);
            sb.Append($"{typeString}");

            if (param.HasDefaultValue)
            {
                sb.Append($" = {param.DefaultValue}");
            }

            if (i < m_parameterInfo.Length - 1)
            {
                sb.Append(", ");
            }
        }
        sb.Append(")");

        return sb.ToString();
    }

    public class Builder
    {
        private string m_builderIdentifier;
        private string m_builderDescription = string.Empty; // Default to empty if not provided
        private IDelegate m_builderCommandAction;

        public Builder SetIdentifier(string identifier)
        {
            m_builderIdentifier = identifier;
            return this;
        }

        public Builder SetDescription(string description)
        {
            m_builderDescription = description;
            return this;
        }

        public Builder SetCommandAction(IDelegate commandAction)
        {
            m_builderCommandAction = commandAction;
            return this;
        }

        public CommandDefinition<IDelegate> Build()
        {
            if (string.IsNullOrEmpty(m_builderIdentifier))
            {
                throw new InvalidOperationException("Command identifier must be set.");
            }

            if (m_builderCommandAction == null)
            {
                throw new InvalidOperationException("Command action must be set.");
            }

            // Use the overloaded constructor based on the description
            return string.IsNullOrEmpty(m_builderDescription)
                ? new CommandDefinition<IDelegate>(m_builderIdentifier, m_builderCommandAction)
                : new CommandDefinition<IDelegate>(m_builderIdentifier, m_builderDescription, m_builderCommandAction);
        }
    }
}
