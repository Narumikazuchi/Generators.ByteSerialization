using Narumikazuchi.InputOutput;

namespace Narumikazuchi.Generators.ByteSerialization;

internal readonly struct __StreamWrapper : ISeekableStream, IWriteableStream
{
    public void Close()
    {
        m_Stream.Close();
    }

    public void Dispose()
    {
        m_Stream.Dispose();
    }

    public void Flush()
    {
        m_Stream.Flush();
    }

    public Int64 Seek(Int64 offset,
                      SeekOrigin origin)
    {
        return m_Stream.Seek(offset: offset,
                             origin: origin);
    }

    public void WriteByte(Byte value)
    {
        m_Stream.WriteByte(value);
    }

    public ValueTask DisposeAsync()
    {
        return m_Stream.DisposeAsync();
    }

    public async ValueTask FlushAsync()
    {
        await m_Stream.FlushAsync();
    }

    public void Write(ReadOnlySpan<Byte> buffer)
    {
        m_Stream.Write(buffer);
    }

    public async ValueTask WriteAsync(ReadOnlyMemory<Byte> buffer,
                                      CancellationToken cancellationToken)
    {
        await m_Stream.WriteAsync(buffer: buffer,
                                  cancellationToken: cancellationToken);
    }

    public Int64 Length
    {
        get
        {
            return m_Stream.Length;
        }
    }

    public Int64 Position
    {
        get
        {
            return m_Stream.Position;
        }

        set
        {
            m_Stream.Position = value;
        }
    }

    internal __StreamWrapper(Stream stream)
    {
        m_Stream = stream;
    }

    private readonly Stream m_Stream;
}