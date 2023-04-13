namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Provides methods to serialize runtime objects into and from <see cref="Byte"/>[] arrays or streams.
/// </summary>
static public partial class ByteSerializer
{
    static ByteSerializer()
    {
        Type[] types = AppDomain.CurrentDomain.GetAssemblies()
                                              .SelectMany(a => a.GetTypes())
                                              .Where(t => !t.IsInterface && t.IsAssignableTo(typeof(IByteSerializer)))
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
            throw new Exception();
        }

        Handlers = instance!;
        s_PolymorphicDeserializerMethod = typeof(ByteSerializer).GetMethods()
                                                                .Where(method => method.Name is nameof(Deserialize))
                                                                .Where(method => method.IsStatic)
                                                                .Where(method => method.IsPublic)
                                                                .First(method => method.GetParameters()[0].ParameterType == typeof(Byte).MakePointerType());
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
        if (graph is null)
        {
            return 20;
        }
        else
        {
            if (graph.GetType()
                     .IsUnmanagedStruct())
            {
                return Unsafe.SizeOf<TSerializable>();
            }
            else if (Handlers is ISerializationHandler<TSerializable> handler)
            {
                return handler.GetExpectedSize(graph) + 20;
            }
            else
            {
                throw new TypeNotSerializable(graph.GetType());
            }
        }
    }

    static private IByteSerializer Handlers { get; }
}