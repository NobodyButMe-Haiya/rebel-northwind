namespace RebelCmsTemplate.Models.Application;

public class CustomerModel
{
    public uint CustomerKey { get; init; }
    public uint TenantKey { get; init; }
    public string? CustomerCode { get; init; }
    public string? CustomerName { get; init; }
    public string? CustomerContactName { get; init; }
    public string? CustomerContactTitle { get; init; }
    public string? CustomerAddress { get; init; }
    public string? CustomerCity { get; init; }
    public string? CustomerRegion { get; init; }
    public string? CustomerPostalCode { get; init; }
    public string? CustomerCountry { get; init; }
    public string? CustomerPhone { get; init; }
    public string? CustomerFax { get; init; }
    public int IsDelete { get; init; }
}