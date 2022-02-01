namespace RebelCmsTemplate.Models.Application;

public partial class OrderModel
{
    public uint OrderKey { get; init; }
    public uint TenantKey { get; init; }
    public uint CustomerKey { get; init; }
    public uint ShipperKey { get; init; }
    public uint EmployeeKey { get; init; }
    public DateOnly? OrderDate { get; init; }
    public DateOnly? OrderRequiredDate { get; init; }
    public DateOnly? OrderShippedDate { get; init; }
    public decimal OrderFreight { get; init; }
    public string? OrderShipName { get; init; }
    public string? OrderShipAddress { get; init; }
    public string? OrderShipCity { get; init; }
    public string? OrderShipRegion { get; init; }
    public string? OrderShipPostalCode { get; init; }
    public string? OrderShipCountry { get; init; }
    public int IsDelete { get; init; }
}

public partial class OrderModel
{
    public string? TenantName { get; init; }
    public string? CustomerName { get; init; }
    public string? ShipperName { get; init; }
    public string? EmployeeLastName { get; init; }
}

public partial class OrderModel
{
    // @todo  we cannot auto declare this thing as models  .. data is enough
    public List<OrderDetailModel>? Data { get; set; }
}