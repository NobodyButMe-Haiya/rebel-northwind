namespace RebelCmsTemplate.Models.Administrator;

public class LogModel
{
    public uint LogKey { get; init; }
    public string? LogUserName { get; init; }
    public string? LogQuery { get; init; }
    public string? LogError { get; init; }
    public DateTime LogDateTime { get; init; }
}