namespace CarRental.Shared.Responses
{
    public record UserResponse(
        int UserId,
        string FirstName,
        string LastName,
        string Email,
        string Phone,
        string Role,
        string Password);
}