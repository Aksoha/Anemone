using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Anemone.Repository.HeatingSystem;

public class HeatingSystemLocalRepository : LocalRepository, IHeatingSystemRepository
{
    public HeatingSystemLocalRepository(IFile file, IDirectory directory, ILogger<HeatingSystemLocalRepository> logger,
        LocalRepositoryOptions localRepositoryOptions) : base(localRepositoryOptions)
    {
        File = file;
        Directory = directory;
        Logger = logger;
    }

    private IFile File { get; }
    private IDirectory Directory { get; }
    private ILogger<HeatingSystemLocalRepository> Logger { get; }

    public Task<IEnumerable<string>> GetAllNames()
    {
        var filePaths = Directory.GetFiles(LocalRepositoryOptions.WorkingDir,
            "*" + LocalRepositoryFileExtensions.HeatingSystem);

        var output = filePaths.Select(Path.GetFileNameWithoutExtension).Cast<string>();

        return Task.FromResult(output);
    }

    public async Task Create(PersistenceHeatingSystemModel data, string? fileName = null)
    {
        if (fileName is null)
            throw new ArgumentNullException(nameof(fileName));
        var path = GetFileName(fileName);
        await using var file = File.Create(path);

        Logger.LogInformation("created heating system data file {FileName}", path);

        await JsonSerializer.SerializeAsync(file, data);
        data.Id = fileName;
    }

    public async Task<PersistenceHeatingSystemModel?> Get(string fileName)
    {
        var path = GetFileName(fileName);
        await using var file = File.OpenRead(path);
        var output = await JsonSerializer.DeserializeAsync<PersistenceHeatingSystemModel>(file,
            new JsonSerializerOptions { NumberHandling = JsonNumberHandling.AllowReadingFromString });
        if (output is not null) output.Id = fileName;
        return output;
    }

    public async Task<IEnumerable<PersistenceHeatingSystemModel>> GetAll()
    {
        var fileNames = await GetAllNames();

        var output = new List<PersistenceHeatingSystemModel>();
        foreach (var fileName in fileNames)
        {
            var model = await Get(fileName);
            if (model != null) output.Add(model);
        }

        return output;
    }

    public async Task Update(string fileName, PersistenceHeatingSystemModel data)
    {
        await Create(data, fileName);
        Logger.LogInformation("updated heating system data file {FileName}", data.Id);
    }

    public Task Delete(PersistenceHeatingSystemModel data)
    {
        if (data.Id is null)
            throw new ArgumentNullException(nameof(data), "Id of an object is null");

        File.Delete(data.Id);
        Logger.LogInformation("deleted heating system data file {FileName}", data.Id);
        data.Id = null;
        return Task.CompletedTask;
    }

    private string GetFileName(string fileName)
    {
        return Path.Combine(LocalRepositoryOptions.WorkingDir, fileName) +
               LocalRepositoryFileExtensions.HeatingSystem;
    }
}