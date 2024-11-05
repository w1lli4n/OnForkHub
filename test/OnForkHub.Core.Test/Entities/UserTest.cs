using Newtonsoft.Json.Linq;

using OnForkHub.Core.Validations;

namespace OnForkHub.Core.Test.Entities;

public class UserTests
{
    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Should create user successfully")]
    public void ShouldCreateUserSuccessfully()
    {
        var name = Name.Create("John Silva");    
        var user = User.Create(name, "john@email.com");

        user.Should().NotBeNull();
        user.Name.Should().Be(name);     
        user.Email.Value.Should().Be("john@email.com");
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.UpdatedAt.Should().BeNull();
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Should keep CreatedAt unchanged after update")]
    public void ShouldKeepCreatedAtUnchangedAfterUpdate()
    {
        var name = Name.Create("John Silva");
        var user = User.Create(name, "john@email.com");
        var creationDate = user.CreatedAt;

        var updatedName = Name.Create("John Pereira");
        user.UpdateName(updatedName);

        user.CreatedAt.Should().Be(creationDate);
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Should throw exception when trying to create user with empty email")]
    public void ShouldThrowExceptionWhenEmailIsEmpty()
    {
        var name = Name.Create("John Silva");

        Action act = () => User.Create(name, "");

        act.Should().Throw<DomainException>().WithMessage("Email cannot be empty");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("email@")]
    [InlineData("@domain.com")]
    [Trait("Category", "Unit")]
    [DisplayName("Should throw exception when creating user with invalid email")]
    public void ShouldThrowExceptionWhenEmailIsInvalid(string invalidEmail)
    {
        var name = Name.Create("John Silva");

        Action act = () => User.Create(name, invalidEmail);

        act.Should().Throw<DomainException>().WithMessage("Invalid email");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Should throw DomainException when name is less than 3 characters")]
    public void ShouldThrowDomainExceptionWhenNameIsTooShort()
    {
        var name = "Al";
        var result = new ValidationResult().AddErrorIf(name.Length < 3, "Name must be at least 3 characters long", "Name");

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle();
        result.Errors.First().Message.Should().Be("Name must be at least 3 characters long");
        result.Errors.First().Field.Should().Be("Name");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Should throw exception when updating user with invalid email")]
    public void ShouldThrowExceptionWhenUpdatingWithInvalidEmail()
    {
        var name = Name.Create("John Silva");
        var user = User.Create(name, "john@email.com");

        Action act = () => user.UpdateEmail("email@");

        act.Should().Throw<DomainException>().WithMessage("Invalid email");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Should throw exception when updating user with invalid name")]
    public void ShouldThrowExceptionWhenUpdatingWithInvalidName()
    {
        var name = Name.Create("John Silva");
        var user = User.Create(name, "john@email.com");

        Action act = () => user.UpdateName(Name.Create("Jo"));

        act.Should().Throw<DomainException>().WithMessage("User name is invalid");
    }

    [Fact]
    [Trait("Category", "Unit")]
    [DisplayName("Should successfully update user's name and email")]
    public void ShouldUpdateUserNameAndEmailSuccessfully()
    {
        var name = Name.Create("John Silva");
        var user = User.Create(name, "john@email.com");

        var updatedName = Name.Create("John Pereira");
        user.UpdateName(updatedName);
        user.UpdateEmail("john.pereira@email.com");

        user.Name.Should().Be(updatedName);
        user.Email.Value.Should().Be("john.pereira@email.com");
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
