namespace Yact.Domain.ValueObjects.Activity;

public record FilePath
{
    public string Value { get; }

    private FilePath(string value) => Value = value;

    public static FilePath From(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be empty");

        return new FilePath(path);
    }
}
