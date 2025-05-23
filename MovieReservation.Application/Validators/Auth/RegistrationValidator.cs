using FluentValidation;
using MovieReservation.Application.DTOs.Auth;
using MovieReservation.Domain.Repository;

namespace MovieReservation.Application.Validators.Auth;

public class RegistrationValidator : AbstractValidator<RegistrationDto>
{
    public RegistrationValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256)
            .MustAsync(async (email, token) =>
                await userRepository.IsEmailUniqueAsync(email, token))
                .WithMessage("Email is already in use");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(30);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(10);

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
                .WithMessage("Passwords don't match");
    }
}