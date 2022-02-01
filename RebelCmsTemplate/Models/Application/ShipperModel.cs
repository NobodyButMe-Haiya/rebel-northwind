namespace RebelCmsTemplate.Models.Application;

public class ShipperModel
{
    public uint ShipperKey { get; init; }
    public uint TenantKey { get; init; }
    public string? ShipperName { get; init; }
    public string? ShipperPhone { get; init; }
    public int IsDelete { get; init; }
}