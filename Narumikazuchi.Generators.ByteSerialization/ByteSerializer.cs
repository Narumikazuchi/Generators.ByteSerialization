namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Provides methods to serialize runtime objects into and from <see cref="Byte"/>[] arrays or streams.
/// </summary>
/// <remarks>
/// The <see cref="ByteSerializer"/> currently only supports the serialization of public mutable members,
/// which means public non-readonly fields and public properties with a getter and setter or initilizer.
/// </remarks>
static public partial class ByteSerializer
{
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
        if (typeof(TSerializable).IsUnmanagedStruct())
        {
            return Unsafe.SizeOf<TSerializable>();
        }
        else if (Handlers is ISerializationHandler<TSerializable> handler)
        {
            return handler.GetExpectedArraySize(graph);
        }
        else
        {
            throw new TypeNotSerializable(graph?.GetType() ?? typeof(TSerializable));
        }
    }

    static private IByteSerializer FindHandler()
    {
        List<Type> types = new();
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                Type[] assemblyTypes = assembly.GetTypes();
#if DEBUG
                Type[] notInterface = assemblyTypes.Where(t => !t.IsInterface).ToArray();
                Type[] compilerGenerated = notInterface.Where(AttributeResolver.HasAttribute<CompilerGeneratedAttribute>).ToArray();
                Type[] reduced = compilerGenerated.Where(t => t.IsAssignableTo(typeof(IByteSerializer))).ToArray();
#else
                IEnumerable<Type> reduced = assemblyTypes.Where(t => !t.IsInterface)
                                                         .Where(AttributeResolver.HasAttribute<CompilerGeneratedAttribute>)
                                                         .Where(t => t.IsAssignableTo(typeof(IByteSerializer)));
#endif
                types.AddRange(reduced);
            }
            catch { }
        }

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
}