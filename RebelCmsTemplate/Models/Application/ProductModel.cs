namespace RebelCmsTemplate.Models.Application;

public partial class ProductModel
{
    public uint ProductKey { get; init; }
    public uint TenantKey { get; init; }
    public uint SupplierKey { get; init; }
    public uint ProductCategoryKey { get; init; }
    public uint ProductTypeKey { get; init; }
    public string? ProductName { get; init; }
    public string? ProductDescription { get; init; }
    public string? ProductQuantityPerUnit { get; init; }
    public double ProductCostPrice { get; init; }
    public double ProductSellingPrice { get; init; }
    public double ProductUnitsInStock { get; init; }
    public double ProductUnitsOnOrder { get; init; }
    public double ProductReOrderLevel { get; init; }
    public int IsDelete { get; init; }
    public string? ExecuteBy { get; init; }
}

public partial class ProductModel
{
    public string? TenantName { get; init; }
    public string? SupplierName { get; init; }
    public string? ProductCategoryName { get; init; }
    public string? ProductTypeName { get; init; }
}