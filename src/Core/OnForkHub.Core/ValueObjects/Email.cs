namespace OnForkHub.Core.ValueObjects;

public partial class Email : ValueObject
{
    private readonly CustomValidationResult _validationResult;

    private Email(string value, CustomValidationResult validationResult)
    {
        Value = value;
        Validate();
        _validationResult = validationResult;
    }

    public string Value { get; private set; }

    public static Email Create(string value)
    {
        DomainException.ThrowErrorWhen(() => string.IsNullOrWhiteSpace(value), EmailResources.EmailCannotBeEmpty);
        var email = new Email(value, new CustomValidationResult());
        email.Validate();
        return email;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value.ToLower(CultureInfo.CurrentCulture);
    }

    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
    private static partial Regex MyRegex();

    public override CustomValidationResult Validate()
    {
        var regex = MyRegex();
        DomainException.ThrowErrorWhen(() => !regex.IsMatch(Value), EmailResources.InvalidEmail);

        return _validationResult;
    }
}