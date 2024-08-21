using Models;

namespace Services.FileService;

public interface IFileService
{
    void CreateFile(Models.File file);

    void CreateDirectory(Models.Directory directory);

    SystemNode[] GetNodes(string path);

    void DeleteFile(string path);

    void DeleteDirectory(string path);

    void UpdateFile(string path, Models.File file);

    //void MoveFile(string path, string newPath);
}