using System;
using System.IO;
using System.Reflection;

namespace Anemone;

public static class ApplicationInfo
{
    private const string DatabaseName = "data.db";

    public static readonly string LocalFilesDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        Assembly.GetEntryAssembly()!.GetName()!.Name!);

    private static readonly string DatabasePath = Path.Combine(LocalFilesDirectory, DatabaseName);

    public static readonly string ConnectionString = @$"Data Source={DatabasePath};";
}