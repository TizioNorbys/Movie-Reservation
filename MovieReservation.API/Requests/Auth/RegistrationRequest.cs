namespace MovieReservation.API.Requests.Auth;

public record RegistrationRequest(
    string Email,
    string FirstName,
    string LastName,
    string Password,
    string ConfirmPassword);