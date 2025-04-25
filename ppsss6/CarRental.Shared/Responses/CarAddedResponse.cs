namespace CarRental.Shared.Responses
{
    public class CarAddedResponse
    {
        public int CarId { get; set; }
        public string Message { get; set; } = "Автомобиль успешно добавлен.";
    }
}