namespace RebelCmsTemplate.Models.Application;

public class SupplierModel
{
    public uint SupplierKey { get; init; }
    public uint TenantKey { get; init; }
    public string? SupplierName { get; init; }
    public string? SupplierContactName { get; init; }
    public string? SupplierContactTitle { get; init; }
    public string? SupplierAddress { get; init; }
    public string? SupplierCity { get; init; }
    public string? SupplierRegion { get; init; }
    public string? SupplierPostalCode { get; init; }
    public string? SupplierCountry { get; init; }
    public string? SupplierPhone { get; init; }
    public string? SupplierFax { get; init; }
    public string? SupplierHomePage { get; init; }
    public int IsDelete { get; init; }
}