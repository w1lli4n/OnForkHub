using OnForkHub.Core.Entities.Base;
using OnForkHub.Core.Validations;

namespace OnForkHub.Core.Test.Entities.Base;

public class EntidadeInvalidaTestFixture : BaseEntity
{
    public EntidadeInvalidaTestFixture()
        : base() { }

    public EntidadeInvalidaTestFixture(long id, DateTime createdAt, DateTime? updatedAt = null)
        : base(id, createdAt, updatedAt) { }

    public void ExecutarUpdate()
    {
        this.Validate();
        this.Update();
    }

    public override ValidationResult Validate()
    {
        var validationResult = new ValidationResult();
        ValidationResult.ThrowErrorIf(() => this.Id <= 0, "Id deve ser maior que zero");
        return validationResult;
    }
}
