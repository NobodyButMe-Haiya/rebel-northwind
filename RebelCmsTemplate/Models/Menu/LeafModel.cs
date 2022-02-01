namespace RebelCmsTemplate.Models.Menu;

public class LeafModel
{
    public uint LeafKey { get; init; }
    public uint FolderKey { get; init; }
    public int LeafSeq { get; init; }
  
    public string? LeafFilename { get; init; }
    public string? LeafName { get; init; }
    public string? LeafIcon { get; init; }
    public int IsDelete { get; init; }
}