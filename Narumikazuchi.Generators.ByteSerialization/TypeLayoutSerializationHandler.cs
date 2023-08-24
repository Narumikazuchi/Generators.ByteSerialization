namespace Narumikazuchi.Generators.ByteSerialization;

/// <summary>
/// Implements the serialization behavior for <see cref="TypeLayout"/> objects.
/// </summary>
public sealed class TypeLayoutSerializationHandler : ISerializationHandler<TypeLayout>
{
    /// <summary>
    /// The default handler used by the code generator.
    /// </summary>
    static public TypeLayoutSerializationHandler Default { get; } = new();

    /// <inheritdoc/>
    public Unsigned31BitInteger Deserialize(ReadOnlySpan<Byte> buffer,
                                            out TypeLayout result)
    {
        Int32 pointer = 0;
        LayoutMemberType type = Unsafe.As<Byte, LayoutMemberType>(ref MemoryMarshal.GetReference(buffer[pointer..]));
        pointer += Unsafe.SizeOf<LayoutMemberType>();
        if (type is not LayoutMemberType.Object)
        {
            result = new(type);
            return pointer;
        }

        Byte memberCount = buffer[pointer++];
        TypeLayout[] members;
        if (memberCount is 0)
        {
            members = Array.Empty<TypeLayout>();
        }
        else
        {
            members = new TypeLayout[memberCount];
            for (Byte counter = 0;
                 counter < memberCount;
                 counter++)
            {
                pointer += this.Deserialize(buffer: buffer[pointer..],
                                            result: out TypeLayout member);
                members[counter] = member;
            }
        }

        result = new(memberType: type,
                     members: members);
        return pointer;
    }

    /// <inheritdoc/>
    public Unsigned31BitInteger GetExpectedArraySize(TypeLayout graph)
    {
        Int32 size = Unsafe.SizeOf<LayoutMemberType>();
        if (graph.m_Type is LayoutMemberType.Object)
        {
            size += sizeof(Byte);
            if (graph.m_Members is not null)
            {
                foreach (TypeLayout member in graph.m_Members)
                {
                    size += this.GetExpectedArraySize(member);
                }
            }
        }

        return size;
    }

    /// <inheritdoc/>
    public Unsigned31BitInteger Serialize(Span<Byte> buffer,
                                          TypeLayout graph)
    {
        Int32 pointer = 0;
        Unsafe.As<Byte, LayoutMemberType>(ref buffer[pointer]) = graph.m_Type;
        pointer += Unsafe.SizeOf<LayoutMemberType>();
        if (graph.m_Type is not LayoutMemberType.Object)
        {
            return pointer;
        }

        if (graph.m_Members is null)
        {
            buffer[pointer] = 0x0;
            pointer++;
        }
        else
        {
            buffer[pointer] = (Byte)graph.m_Members.Length;
            pointer++;
            foreach (TypeLayout member in graph.m_Members)
            {
                pointer += this.Serialize(buffer: buffer[pointer..],
                                          graph: member);
            }
        }

        return pointer;
    }
}