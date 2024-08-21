using Context;
using Models;
using Services.FileService;

namespace Services.RestoreService;

public class RestoreService : IRestoreService
{
    private readonly DatabaseContext _context;
    private readonly IFileService _fileService;
    private readonly ILogger<RestoreService> _logger;

    public RestoreService(DatabaseContext context, IFileService fileService, ILogger<RestoreService> logger)
    {
        _context = context;
        _fileService = fileService;
        _logger = logger;   
    }

    public void GetRestoration()
    {
        const string ROOTDIR = "/";
        var nodes = _context.SystemNodes.Where(n => n.Path == ROOTDIR).ToArray();

        if (nodes.Length == 0)
        {
            _logger.LogInformation("Restoration is already done.");
            return;
        }

        foreach (var node in nodes)
        {
            if (node is Models.File file)
            {
                _fileService.CreateFile(file);
                continue;
            }

            if (node is Models.Directory directory)
            {
                _fileService.CreateDirectory(directory);
            }
        }
    }

    public async Task SyncData(SystemNode node, RestoreType type)
    {
        switch (type)
        {
            case RestoreType.Creation:
                CreateNode(node);
                break;
            case RestoreType.Deletion:
                DeleteNode(node);
                break;
            case RestoreType.Update:
                UpdateNode(node);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        await _context.SaveChangesAsync();
    }

    private void CreateNode(SystemNode node)
    {
        if (node is Models.File file)
        {
            _fileService.CreateFile(file);
            return;
        }
        else if (node is Models.Directory directory)
        {
            _fileService.CreateDirectory(directory);
        }
    }

    private void DeleteNode(SystemNode node)
    {
        if (node is Models.File file)
        {
            _fileService.DeleteFile(file.Path);
            return;
        }
        else if (node is Models.Directory directory)
        {
            _fileService.DeleteDirectory(directory.Path);
        }
    }

    private void UpdateNode(SystemNode node)
    {
        if (node is Models.File file)
        {
            _fileService.UpdateFile(file.Path, file);
            return;
        }
    }
}