using System.ComponentModel;

namespace Infrastructure.Contracts.ResultContracts;

public static class ResultHelper
{
    private const int RESULT_CODE_TO_HTTP_STATUS_DIV = 1000;

    public static int ConvertHttpStatusCode(ResultCode code)
    {
        // ReSharper disable once PossibleLossOfFraction
        return (int)Math.Floor((decimal)((int)code / RESULT_CODE_TO_HTTP_STATUS_DIV));
    }

    public static bool IsSuccess(ResultCode code)
    {
        return ConvertHttpStatusCode(code) / 100 == 2;
    }

    internal static (string? Name, int Value, string? Description) GetEnumInfo(Enum source,
        bool allowDescriptionNull = false)
    {
        var type = source.GetType();
        
        if (!type.IsEnum) throw new NotSupportedException("source is not Enum");
        if (!Enum.IsDefined(type, source)) return (null, 0, null);

        var field = type.GetField(Enum.GetName(type, source) ?? throw new InvalidOperationException());

        if (field == null) throw new InvalidOperationException("field can not be null");

        var customAttributeData =
            field.CustomAttributes.FirstOrDefault(t => t.AttributeType == typeof(DescriptionAttribute));

        var item = (int)Enum.Parse(type, field.Name);

        var text = customAttributeData?.ConstructorArguments[0].Value?.ToString();

        return allowDescriptionNull ? (field.Name, item, text) : (field.Name, item, text ?? field.Name);
    }

    internal static (string? Name, int Value, string? Description, string? CustomerDetail) GetEnumInfo<TAttribute>(
        Enum source,
        bool allowDescriptionNull = false) where TAttribute : Attribute
    {
        var (text, item, desc) = GetEnumInfo(source, allowDescriptionNull);
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(desc))
            return (text, item, desc, null);
        var type = source.GetType();
        var customerDetail = type.GetField(Enum.GetName(type, source) ?? throw new InvalidOperationException())?.CustomAttributes
            .FirstOrDefault(t => t.AttributeType == typeof(TAttribute))?.ConstructorArguments[0].Value?.ToString();
        return (text, item, desc, customerDetail ?? desc);
    }
}