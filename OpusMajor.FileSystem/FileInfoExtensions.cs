using System.Text;
using System.Text.Json;

namespace OpusMajor.FileSystem;

public static class FileInfoExtensions
{
    public static FileInfo WriteAllText(this FileInfo fi, string? text)
    {
        File.WriteAllText(fi.FullName, text);
        return fi;
    }

    public static FileInfo WriteAllText(this FileInfo fi, string? text, Encoding e)
    {
        File.WriteAllText(fi.FullName, text, e);
        return fi;
    }

    public static async Task<FileInfo> WriteAllTextAsync(this FileInfo fi, string? text, CancellationToken ct = default)
    {
        await File.WriteAllTextAsync(fi.FullName, text, ct);
        return fi;
    }

    public static async Task<FileInfo> WriteAllTextAsync(this FileInfo fi, string? text, Encoding e, CancellationToken ct = default)
    {
        await File.WriteAllTextAsync(fi.FullName, text, e, ct);
        return fi;
    }

    public static FileInfo WriteAllBytes(this FileInfo fi, byte[] bytes)
    {
        File.WriteAllBytes(fi.FullName, bytes);
        return fi;
    }

    public static async Task<FileInfo> WriteAllBytesAsync(this FileInfo fi, byte[] bytes, CancellationToken ct = default)
    {
        await File.WriteAllBytesAsync(fi.FullName, bytes, ct);
        return fi;
    }

    public static FileInfo WriteAllLines(this FileInfo fi, IEnumerable<string> lines)
    {
        File.WriteAllLines(fi.FullName, lines);
        return fi;
    }

    public static FileInfo WriteAllLines(this FileInfo fi, IEnumerable<string> lines, Encoding e)
    {
        File.WriteAllLines(fi.FullName, lines, e);
        return fi;
    }

    public static async Task<FileInfo> WriteAllLinesAsync(this FileInfo fi, IEnumerable<string> lines, CancellationToken ct = default)
    {
        await File.WriteAllLinesAsync(fi.FullName, lines, ct);
        return fi;
    }

    public static async Task<FileInfo> WriteAllLinesAsync(this FileInfo fi, IEnumerable<string> lines, Encoding e, CancellationToken ct = default)
    {
        await File.WriteAllLinesAsync(fi.FullName, lines, e, ct);
        return fi;
    }

    public static T? GetJson<T>(this FileInfo fi, JsonSerializerOptions? options = null)
    {
        using var stream = File.OpenRead(fi.FullName);
        return JsonSerializer.Deserialize<T>(stream, options);
    }

    public static async Task<T?> GetJsonAsync<T>(this FileInfo fi, JsonSerializerOptions? options = null,
        CancellationToken ct = default)
    {
        await using var stream = File.OpenRead(fi.FullName);
        return await JsonSerializer.DeserializeAsync<T>(stream, options, ct);
    }

}