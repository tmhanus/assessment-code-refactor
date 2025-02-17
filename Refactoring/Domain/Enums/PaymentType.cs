namespace Refactoring.Domain.Enums;

public sealed record PaymentType
{
    public string Name { get; }

    public static readonly PaymentType PayPal = new("PayPal");
    public static readonly PaymentType CreditCard = new("CreditCard");

    private PaymentType(string name)
    {
        Name = name;
    }

    private static readonly IReadOnlyDictionary<string, PaymentType> Types =
        new Dictionary<string, PaymentType>(StringComparer.OrdinalIgnoreCase)
        {
            { PayPal.Name, PayPal },
            { CreditCard.Name, CreditCard }
        };

    public static IEnumerable<PaymentType> GetAll() => Types.Values;

    public static PaymentType? FromString(string name) =>
        Types.TryGetValue(name, out var type) ? type : null;

    public static bool IsValid(string name) => Types.ContainsKey(name);
    
    public static implicit operator string(PaymentType type) => type.Name;
    
    public static explicit operator PaymentType(string name) =>
        FromString(name) ?? throw new ArgumentException($"Invalid payment type: {name}");

    public override string ToString() => Name;
}