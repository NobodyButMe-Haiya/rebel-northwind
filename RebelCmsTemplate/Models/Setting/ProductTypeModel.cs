namespace RebelCmsTemplate.Models.Setting;

public partial class ProductTypeModel
{
    public uint ProductTypeKey { get; init; }
    public uint TenantKey { get; init; }
    public uint ProductCategoryKey { get; init; }
    public string? ProductTypeName { get; init; }
}

public partial class ProductTypeModel
{
    public string? ProductCategoryName { get; init; }
    public string? TenantName { get; init; }
}