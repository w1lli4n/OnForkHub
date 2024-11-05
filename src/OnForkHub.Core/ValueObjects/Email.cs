namespace OnForkHub.Core.ValueObjects;

public partial class Email : ValueObject
{
    private readonly ValidationResult _validationResult;

    private Email(string value, ValidationResult validationResult)
    {
        Value = value;
        Validate();
        _validationResult = validationResult;
    }

    public string Value { get; private set; }

    public static Email Create(string value)
    {
        DomainException.ThrowErrorWhen(() => string.IsNullOrWhiteSpace(value), $"{nameof(Email)} cannot be empty");
        var email = new Email(value, new ValidationResult());
        email.Validate();
        return email;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value.ToLower(CultureInfo.CurrentCulture);
    }

    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
    private static partial Regex MyRegex();

    public override ValidationResult Validate()
    {
        var regex = MyRegex();
        DomainException.ThrowErrorWhen(() => !regex.IsMatch(Value), "Invalid email");


        return _validationResult;
    }
}
