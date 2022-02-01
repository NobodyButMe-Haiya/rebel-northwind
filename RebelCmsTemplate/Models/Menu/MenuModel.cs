namespace RebelCmsTemplate.Models.Menu;

public class MenuModel
{
    public string? FolderName { get; init; }
    public List<MenuDetailModel>? Details { get; set; }
}

public class MenuDetailModel
{
    public int LeafKey { get; init; }
    public string? LeafName { get; init; }
}