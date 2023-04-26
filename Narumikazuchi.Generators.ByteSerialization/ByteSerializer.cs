namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Provides methods to serialize runtime objects into and from <see cref="Byte"/>[] arrays or streams.
/// </summary>
/// <remarks>
/// The <see cref="ByteSerializer"/> currently only supports the serialization of public mutable members,
/// which means public non-readonly fields and public properties with a getter and setter or initilizer.
/// </remarks>
public sealed partial class ByteSerializer
{
    /// <summary>
    /// Adds the specified handler to the custom handlers for all serializers to use for serialization.
    /// </summary>
    /// <param name="handler">The handler to add.</param>
    /// <exception cref="ArgumentNullException"/>
    static public void GlobalSetupWithHandler<TSerializable>(ISerializationHandler<TSerializable> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        s_PriorityHandlers.Add(key: typeof(TSerializable),
                               value: handler);
    }

    /// <summary>
    /// Calculates the expected size of the <see cref="Byte"/>[] array after serialization of the sepcified runtime object.
    /// </summary>
    /// <param name="graph">The runtime object to calculate the expected size of.</param>
    /// <returns>The expected size of the <see cref="Byte"/>[] array after serialization.</returns>
    /// <remarks>
    /// This should NOT throw an exception however if it still does, then it indicates a problem with the code generation.
    /// If you encounter an exception it would help if you could contact me for a timely fix.
    /// </remarks>
    /// <exception cref="TypeNotSerializable"/>
    static public Int32 GetExpectedSerializedSize<TSerializable>(TSerializable? graph)
    {
        Type? type = graph?.GetType();
        if (type is null)
        {
            return 21;
        }
        else
        {
            if (type.IsUnmanagedStruct())
            {
                return Unsafe.SizeOf<TSerializable>();
            }
            else if (Handlers is ISerializationHandler<TSerializable> handler)
            {
                return handler.GetExpectedArraySize(graph!) + 37;
            }
            else
            {
                throw new TypeNotSerializable(type);
            }
        }
    }

    /// <summary>
    /// Adds the specified handler to the custom handlers to use for serialization.
    /// </summary>
    /// <param name="handler">The handler to add.</param>
    /// <returns>The <see cref="ByteSerializer"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException"/>
    public ByteSerializer SetupWithHandler<TSerializable>(ISerializationHandler<TSerializable> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        m_PriorityHandlers.Add(key: typeof(TSerializable),
                               value: handler);
        return this;
    }

    /// <summary>
    /// Gets the default seriliazer that uses globally setup custom handlers but no locally setup custom handlers.
    /// </summary>
    static public ByteSerializer Default { get; } = new();

    static private IByteSerializer FindHandler()
    {
        Type[] types = AppDomain.CurrentDomain.GetAssemblies()
                                              .SelectMany(a => a.GetTypes())
                                              .ToArray();
        types = types.Where(t => !t.IsInterface)
                     .Where(AttributeResolver.HasAttribute<CompilerGeneratedAttribute>)
                     .Where(t => t.IsAssignableTo(typeof(IByteSerializer)))
                     .ToArray();
        IByteSerializer? instance = default;
        foreach (Type type in types)
        {
            IByteSerializer candidate = (IByteSerializer)Activator.CreateInstance(type)!;
            if (instance is null ||
                candidate.Variant > instance.Variant)
            {
                instance = candidate;
            }
        }

        if (instance is null)
        {
            throw new FailedToGenerateCode();
        }
        else
        {
            return instance;
        }
    }

    static private IByteSerializer Handlers
    {
        get
        {
            return s_Handlers.Value;
        }
    }

    static private readonly Lazy<IByteSerializer> s_Handlers = new(valueFactory: FindHandler,
                                                                   mode: LazyThreadSafetyMode.ExecutionAndPublication);
    static private readonly Dictionary<Type, dynamic> s_PriorityHandlers = new();

    private readonly Dictionary<Type, dynamic> m_PriorityHandlers = new();
}