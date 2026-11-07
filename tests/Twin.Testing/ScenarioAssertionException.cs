namespace Twin.Testing;

/// <summary>A failed scenario expectation. Test runners report it like any failed assertion.</summary>
public sealed class ScenarioAssertionException : Exception
{
    /// <summary>Creates the exception.</summary>
    public ScenarioAssertionException()
    {
    }

    /// <summary>Creates the exception with a message.</summary>
    public ScenarioAssertionException(string message)
        : base(message)
    {
    }

    /// <summary>Creates the exception with a message and cause.</summary>
    public ScenarioAssertionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
