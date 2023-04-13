namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Contains the result of an asynchronous deserialization operation.
/// </summary>
public readonly struct AsynchronousDeserializationResult<TSerializable>
{
    /// <summary>
    /// The amount of bytes read during the operation.
    /// </summary>
    public Int32 BytesRead
    {
        get;
        init;
    }

    /// <summary>
    /// The result of the deserialization operation.
    /// </summary>
    public TSerializable? Result
    {
        get;
        init;
    }
}