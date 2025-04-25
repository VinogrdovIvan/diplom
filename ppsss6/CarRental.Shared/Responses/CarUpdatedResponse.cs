namespace CarRental.Shared.Responses
{
    public class CarUpdatedResponse
    {
        public int CarId { get; set; }
        public string Message { get; set; } = "Информация об автомобиле успешно обновлена.";
    }
}