namespace RebelCmsTemplate.Models.Menu;

public partial class FolderAccessModel
{
    public uint FolderAccessKey { get; init; }
    public uint FolderKey { get; init; }
    public uint RoleKey { get; init; }
    public int FolderAccessValue { get; init; }
}

// this is foreign value so don't want to spoil 
public partial class FolderAccessModel
{
    public string? RoleName { get; init; }
    public string? FolderName { get; init; }
}