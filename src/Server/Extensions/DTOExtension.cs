using DTO;

namespace Server.Extensions;

public static class DTOExtension
{
    public static DirectoryDto ToDto(this Models.Directory directory)
    {
        return new DirectoryDto
        {
            Path = directory.Path,
            NodeType = directory.NodeType
        };
    }

    public static FileDto ToDto(this Models.File file)
    {
        return new FileDto
        {
            Path = file.Path,
            Name = file.Name,
            Size = file.Size,
            NodeType = file.NodeType
        };
    }
}