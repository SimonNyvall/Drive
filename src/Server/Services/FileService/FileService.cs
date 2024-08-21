using System.Text;
using Context;
using Models;

namespace Services.FileService;

class FileService : IFileService
{
    private readonly ILogger<FileService> _logger;

    private static readonly Dictionary<string, List<SystemNode>> _nodes = [];

    public FileService(ILogger<FileService> logger)
    {
        _logger = logger;
    }
    
    public SystemNode[] GetNodes(string path)
    {
        if (string.IsNullOrEmpty(path)) return [];

        if (!_nodes.ContainsKey(path)) return [];

        return [.. _nodes[path]];
    }

    public void CreateDirectory(Models.Directory directory)
    {
        string dirPath = directory.Path;

        EnsureDirectoryExists(dirPath);
    }

    public void CreateFile(Models.File file)
    {
        string filePath = file.Path;
        string dirPath = GetDirPathFromFile(filePath);
        EnsureDirectoryExists(dirPath);

        _nodes[filePath].Add(file);
    }

    public void DeleteFile(string path)
    {
        path = GetDirPathFromFile(path);

        if (string.IsNullOrEmpty(path) || !_nodes.ContainsKey(path)) return;

        _nodes[path].RemoveAll(n => n is Models.File file && file.Path == path);
    }

    public void DeleteDirectory(string path)
    {
        if (string.IsNullOrEmpty(path) || !_nodes.ContainsKey(path)) return;

        _nodes.Remove(path);
    }

    public void UpdateFile(string path, Models.File file)
    {
        path = GetDirPathFromFile(path);

        if (string.IsNullOrEmpty(path) || !_nodes.ContainsKey(path)) return;

        _nodes[path].RemoveAll(n => n is Models.File f && f.Path == path);

        _nodes[path].Add(file);
    }

    private void EnsureDirectoryExists(string path)
    {
        if (string.IsNullOrEmpty(path) || _nodes.ContainsKey(path)) return;

        string parentDir = GetDirPathFromFile(path);
        if (parentDir != path)
        {
            EnsureDirectoryExists(parentDir);
        }

        _nodes.Add(path, []);

        _nodes[parentDir].Add(new Models.Directory(path));
    }

    private string GetDirPathFromFile(string filePath)
    {
        var filePathDirs = filePath.Split('/').Where(p => p != "").ToArray();

        if (filePathDirs.Length == 0) return "/";

        StringBuilder pathBuilder = new();
        pathBuilder.Append('/');

        for (int i = 0; i < filePathDirs.Length - 1; i++)
        {
            pathBuilder.Append($"{filePathDirs[i]}/");
        }

        return pathBuilder.ToString();
    }
}