namespace Twin.Sim.Orders;

/// <summary>Thrown when an order cannot be accepted, for example an unknown unit or an off-map destination.</summary>
public sealed class OrderRejectedException : Exception
{
    /// <summary>Creates the exception.</summary>
    public OrderRejectedException()
    {
    }

    /// <summary>Creates the exception with a reason.</summary>
    public OrderRejectedException(string message)
        : base(message)
    {
    }

    /// <summary>Creates the exception with a reason and cause.</summary>
    public OrderRejectedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
