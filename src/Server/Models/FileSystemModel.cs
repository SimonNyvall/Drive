using System.ComponentModel.DataAnnotations;

namespace Models;

public abstract class SystemNode
{
    [Key]
    public string Path { get; set; } = string.Empty;

    public Type NodeType { get; set; }

    public bool IsStared { get; set; } = false;
}

public class Directory : SystemNode
{
    public Directory(string path) 
    {
        Path = path;
        NodeType = Type.Directory;
    }
}

public class File : SystemNode
{
    public string Name { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>();

    public int Size => Content.Length;

    public File(string path, string name, byte[] content)
    {
        Path = path;
        Name = name;
        Content = content;
        NodeType = Type.File;
    }
}

public enum Type
{
    Directory,
    File
}

public enum RestoreType
    {
        Creation,
        Deletion,
        Update
    }