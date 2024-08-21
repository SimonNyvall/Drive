using Models;

namespace DTO;

public class FileDto
{
    public string Path { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Size { get; set; }
    public Models.Type NodeType { get; set; }
}