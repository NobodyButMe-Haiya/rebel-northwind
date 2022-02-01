namespace RebelCmsTemplate.Models.Shared;

public class ParameterModel
{
    public ParameterModel()
    {
    }

    public ParameterModel(string? key, dynamic? value)
    {
        Key = key;
        Value = value;
    }

    public string? Key { get; init; }
    public dynamic? Value { get; init; }
}