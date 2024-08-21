using Models;

namespace DTO;

public class DirectoryDto
{
    public string Path { get; set; } = string.Empty;
    public Models.Type NodeType { get; set; }
}

