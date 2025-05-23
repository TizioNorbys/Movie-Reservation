namespace MovieReservation.Application.DTOs.Auth;

public record RegistrationDto(
    string Email,
    string FirstName,
    string LastName,
    string Password,
    string ConfirmPassword);