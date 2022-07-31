# LibFileSystem

A library of extensions to DirectoryInfo and FileInfo

## DirectoryInfo Extensions

### GetFile
```c#
var di = new DirectoryInfo("C:/tmp")
var file = di.GetFile("a.txt")
```
Returns a file info object for the name passed into
the call. It will return even if the file does not exist.

### GetExistingFile
```c#
var di = new DirectoryInfo("C:/tmp")
var file = di.GetExistingFile("a.txt")
```
Like GetFile, but will throw a `FileNotFoundException` if the file does not exist.

### GetDirectory
```c#
var di = new DirectoryInfo("C:/tmp")
var directory = di.GetDirectory("a")
```
Returns a `DirectoryInfo` object for the name passed into
the call. It will return even if the directory does not exist.


### EnsureExistence
```c#
var di = new DirectoryInfo("C:/tmp")
di.EnsureExistence()
```

Will create the directory if it does not already exist.

### EnsureDeleted
```c#
var di = new DirectoryInfo("C:/tmp")
di.EnsureDeleted()
```

Deletes the directory, if it does exist.

### EnsureEmpty
```c#
var di = new DirectoryInfo("C:/tmp")
di.EnsureEmpty()
```

Makes sure the directory exists and is empty. It does so by deleting the directory
first if it exists.

### MakeEmpty
```c#
var di = new DirectoryInfo("C:/tmp")
di.MakeEmpty()
```
Deletes the content of the directory. This differs from `EnsureEmpty` as it fails if
the directory does not exist. The advantage is that it does not need to be able to 
delete the directory itself, only the files and subdirectories within the directory.

### CopyTo

```c#
var source = new DirectoryInfo("C:/tmp")
var target = new DirectoryInfo("C:/tmpcopy")
source.CopyTo(target, CopyOperation.Force)
```
Copies the content of the source directory into the target directory.

CopyOperation has the following values

| Name           | Meaning                                            |
|----------------|----------------------------------------------------|
| `FailIfExists` | Fails a file already exists in target              |         |
| `SkipIfExists` | Ignores the file if it already exists              |
| `Overwite`     | Overwrites the file if it exists                   |
| `Force`        | Overwrites and deletes files not present in source |

If `Force` is selected, the source and target directories will be identical after the copy.
(with the content of the source directory)

### GetSize
```c#
var di = new DirectoryInfo("C:/tmp")
long size = di.GetSize()
```

Returns the size of all files in the directory (and all of it's subdirectories)

### DeleteFiles
```c#
var di = new DirectoryInfo("C:/tmp")
di.DeleteFiles(fileInfo => fileInfo.Name.StartsWith("dummy."))
```
Deletes all files for which the predicate returns true.

### CreateZipFile
```c#
var di = new DirectoryInfo("C:/tmp/a")
var fi = new FileInfo("C:/tmp/a.zip")
di.CreateZipFile(di, fi, CompressionLevel.Fastest, true, Encoding.UTF8)
```
Creates a zip archive for the given file. You can find the arguments here:
https://docs.microsoft.com/de-de/dotnet/api/system.io.compression.zipfile.createfromdirectory?view=net-6.0#system-io-compression-zipfile-createfromdirectory(system-string-system-string-system-io-compression-compressionlevel-system-boolean-system-text-encoding)


## FileInfoExtensions

`FileInfo WriteAllText(this FileInfo fi, string? text)`

`FileInfo WriteAllText(this FileInfo fi, string? text, Encoding e)`

`Task<FileInfo> WriteAllTextAsync(this FileInfo fi, string? text, CancellationToken ct = default)`

`Task<FileInfo> WriteAllTextAsync(this FileInfo fi, string? text, Encoding e, CancellationToken ct = default)`

`FileInfo WriteAllBytes(this FileInfo fi, byte[] bytes)`

`Task<FileInfo> WriteAllBytesAsync(this FileInfo fi, byte[] bytes, CancellationToken ct = default)`

`FileInfo WriteAllLines(this FileInfo fi, IEnumerable<string> lines)`

`FileInfo WriteAllLines(this FileInfo fi, IEnumerable<string> lines, Encoding e)`

`Task<FileInfo> WriteAllLinesAsync(this FileInfo fi, IEnumerable<string> lines, CancellationToken ct = default)`

`Task<FileInfo> WriteAllLinesAsync(this FileInfo fi, IEnumerable<string> lines, Encoding e, CancellationToken ct = default)`

work exactly as the functions with the same name in the `FILE` class

### GetJson

`T? GetJson<T>(this FileInfo fi, JsonSerializerOptions? options = null)`

Tries to read the content of the file and converts it to the type T.
The content must be UTF8 encoded.

## Async Operations

For most operations exist Async overloads. They usually simply use Task.Run for the 
synchronous variant. So don't expect significate improvements.