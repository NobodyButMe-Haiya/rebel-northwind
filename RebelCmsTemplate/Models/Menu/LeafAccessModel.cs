namespace RebelCmsTemplate.Models.Menu;

public partial class LeafAccessModel
{
    public uint LeafAccessKey { get; init; }
    public uint LeafKey { get; init; }
    public uint RoleKey { get; init; }
    public int LeafAccessCreateValue { get; init; }
    public int LeafAccessReadValue { get; init; }
    public int LeafAccessUpdateValue { get; init; }
    public int LeafAccessDeleteValue { get; init; }
    public int LeafAccessExtraOneValue { get; init; }
    public int LeafAccessExtraTwoValue { get; init; }
}

public partial class LeafAccessModel
{
    public string? RoleName { get; init; }
    public string? FolderName { get; init; }

    public string? LeafName { get; init; }
}