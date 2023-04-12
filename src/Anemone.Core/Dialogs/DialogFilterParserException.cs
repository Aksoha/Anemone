using System;

namespace Anemone.Core.Dialogs;

public class DialogFilterParserException : Exception
{
    protected DialogFilterParserException(string message) : base(message)
    {
        
    }
}

public class DialogExtensionCountException : DialogFilterParserException
{
    public DialogExtensionCountException() : base(FormatException())
    {
    }

    private static string FormatException()
    {
        return "Cannot convert object to string. Object must contain at least one extension";
    }
}

public class DialogFilterCountException : DialogFilterParserException
{
    public DialogFilterCountException() : base(FormatException())
    {
    }

    private static string FormatException()
    {
        return "Cannot convert object to string. Object must contain at least one filter";
    }
}