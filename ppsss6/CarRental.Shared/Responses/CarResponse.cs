namespace CarRental.Shared.Responses
{
    public record CarResponse(
        int CarId,
        string Brand,
        string Model,
        int Year,
        string Color,
        string LicensePlate,
        decimal HourlyRate,
        bool IsAvailable);
}