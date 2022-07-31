using System.IO.Compression;
using System.Text;

namespace OpusMajor.FileSystem;

public static class DirectoryInfoExtensions
{
    public static FileInfo GetFile(this DirectoryInfo d, string name) =>
        new FileInfo(Path.Combine(d.FullName, name));

    public static FileInfo GetExistingFile(this DirectoryInfo d, string name)
    {
        var file = GetFile(d, name);
        if (!file.Exists) throw new FileNotFoundException(file.FullName);
        return file;
    }

    public static DirectoryInfo GetDirectory(this DirectoryInfo d, string name) =>
        new DirectoryInfo(Path.Combine(d.FullName, name));

    public static DirectoryInfo EnsureExistence(this DirectoryInfo d)
    {
        try
        {
            if (d.Exists) return d;
            d.Create();
            return d;
        }
        catch (Exception)
        {
            if (d.Exists) return d;
            throw;
        }
    }

    public static async Task<DirectoryInfo> EnsureExistenceAsync(this DirectoryInfo d)
    {
        return await Task.Run(() => EnsureExistence(d));
    }

    public static void EnsureDeleted(this DirectoryInfo d)
    {
        if (!d.Exists) return;
        try
        {
            d.Delete(true);
        }
        catch (Exception)
        {
            if (!d.Exists) return;
            throw;
        }
    }

    public static async Task EnsureDeleteAsync(this DirectoryInfo d)
    {
        await Task.Run(() => EnsureDeleted(d));
    }

    public static DirectoryInfo EnsureEmpty(this DirectoryInfo d)
    {
        if (d.Exists) d.EnsureDeleted();
        d.Create();
        return d;
    }

    public static async Task<DirectoryInfo> EnsureEmptyAsync(this DirectoryInfo d)
    {
        return await Task.Run(() => EnsureEmpty(d));
    }

    public static DirectoryInfo MakeEmpty(this DirectoryInfo d)
    {
        foreach (var di in d.EnumerateDirectories())
        {
            di.Delete(true);
        }
        foreach (var di in d.EnumerateFiles())
        {
            di.Delete();
        }

        return d;
    }

    public static DirectoryInfo DeleteFiles(this DirectoryInfo d, Func<FileInfo, bool> pred)
    {
        var filesToDelete = d.EnumerateFiles("*.*", SearchOption.AllDirectories)
            .Where(fi => pred(fi));
        foreach (var info in filesToDelete)
        {
            info.Delete();
        }

        return d;
    }

    public static async Task<DirectoryInfo> DeleteFilesAsync(this DirectoryInfo d, Func<FileInfo, bool> pred)
    {
        return await Task.Run(() => DeleteFiles(d, pred));
    }
    public static DirectoryInfo CopyTo(this DirectoryInfo source, DirectoryInfo target, CopyOperation operation)
    {
        if (operation == CopyOperation.Force)
        {
            target.EnsureEmpty();
        }
        else
        {
            target.EnsureExistence();
        }

        foreach (var file in source.EnumerateFiles())
        {
            var targetFile = new FileInfo(Path.Combine(target.FullName, file.Name));
            if (targetFile.Exists)
            {
                switch (operation)
                {
                    case CopyOperation.FailIfExists:
                        throw new Exception($"File already exists: {targetFile.FullName}");
                    case CopyOperation.SkipIfExists:
                        continue;
                }
            }
            file.CopyTo(targetFile.FullName, true);
        }

        foreach (var directory in source.EnumerateDirectories())
        {
            CopyTo(directory, target.GetDirectory(directory.Name), operation);
        }

        return target;
    }

    public static long GetSize(this DirectoryInfo d)
    {
        return d.EnumerateFiles("*.*", SearchOption.AllDirectories)
            .Sum(x => x.Length);
    }

    public static async Task<long> GetSizeAsync(this DirectoryInfo d) =>
        await Task.Run(() => GetSize(d));

    public static FileInfo CreateZipFile(
        this DirectoryInfo d,
        FileInfo destination,
        CompressionLevel compressionLevel = CompressionLevel.Optimal,
        bool includeBaseDirectory = false,
        Encoding? entryNameEncoding = null)
    {
        ZipFile.CreateFromDirectory(
            d.FullName,
            destination.FullName,
            compressionLevel,
            includeBaseDirectory,
            entryNameEncoding);
        return destination;
    }

    public static async Task<FileInfo> CreateZipFileAsync(
        this DirectoryInfo d,
        FileInfo destination,
        CompressionLevel compressionLevel = CompressionLevel.Optimal,
        bool includeBaseDirectory = false,
        Encoding? entryNameEncoding = null) =>
        await Task.Run(() =>
            CreateZipFile(d, destination, compressionLevel, includeBaseDirectory, entryNameEncoding));
}