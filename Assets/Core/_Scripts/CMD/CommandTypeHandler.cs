using System;

public struct CommandTypeHandler
{
    public Func<string, object> Parser { get; }
    public string TypeString { get; }

    public CommandTypeHandler(Func<string, object> parser, string typeString)
    {
        Parser = parser;
        TypeString = typeString;
    }
}