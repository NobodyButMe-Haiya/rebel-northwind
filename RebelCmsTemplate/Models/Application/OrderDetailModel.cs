namespace RebelCmsTemplate.Models.Application;

public class OrderDetailModel
{
    public uint OrderDetailKey { get; init; }
    public uint OrderKey { get; init; }
    public uint ProductKey { get; init; }
    public decimal OrderDetailUnitPrice { get; init; }
    public int OrderDetailQuantity { get; init; }
    public double OrderDetailDiscount { get; init; }
    public int IsDelete { get; init; }
}