namespace Narumikazuchi.Generators.ByteSerialization.Generators;

public sealed class Logger
{
    public void LogInformation(String message)
    {
        if (!this.LoggingEnabled ||
            String.IsNullOrWhiteSpace(this.LogPath))
        {
            return;
        }

        FileInfo file = new(this.LogPath);
        if (!file.Exists)
        {
            Directory.CreateDirectory(file.Directory.FullName);
            file.Create()
                .Dispose();
        }

        StringBuilder builder = new();
        builder.Append(DateTimeOffset.UtcNow.ToString("yyyy.MM.dd HH:mm:ss.fff"));
        builder.Append(" [INF] === ");
        builder.Append(message);

        using StreamWriter writer = file.AppendText();
        writer.WriteLine(builder.ToString());
    }

    public void LogWarning(String message)
    {
        if (!this.LoggingEnabled ||
            String.IsNullOrWhiteSpace(this.LogPath))
        {
            return;
        }

        FileInfo file = new(this.LogPath);
        if (!file.Exists)
        {
            Directory.CreateDirectory(file.Directory.FullName);
            file.Create()
                .Dispose();
        }

        StringBuilder builder = new();
        builder.Append(DateTimeOffset.UtcNow.ToString("yyyy.MM.dd HH:mm:ss.fff"));
        builder.Append(" [WRN] === ");
        builder.Append(message);

        using StreamWriter writer = file.AppendText();
        writer.WriteLine(builder.ToString());
    }

    public void LogError(String message)
    {
        if (!this.LoggingEnabled ||
            String.IsNullOrWhiteSpace(this.LogPath))
        {
            return;
        }

        FileInfo file = new(this.LogPath);
        if (!file.Exists)
        {
            Directory.CreateDirectory(file.Directory.FullName);
            file.Create()
                .Dispose();
        }

        StringBuilder builder = new();
        builder.Append(DateTimeOffset.UtcNow.ToString("yyyy.MM.dd HH:mm:ss.fff"));
        builder.Append(" [ERR] === ");
        builder.Append(message);

        using StreamWriter writer = file.AppendText();
        writer.WriteLine(builder.ToString());
    }

    public Boolean LoggingEnabled
    {
        get;
        set;
    }

    public String LogPath
    {
        get;
        set;
    }
}
