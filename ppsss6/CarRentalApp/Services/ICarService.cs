using CarRental.Shared.Requests;
using CarRental.Shared.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRentalApp.Services
{
    public interface ICarService
    {
        Task<List<CarResponse>> GetAvailableCarsAsync();
        Task<CarResponse> GetCarByIdAsync(int id);
        Task<CarAddedResponse> AddCarAsync(AddCarRequest request);
        Task<CarUpdatedResponse> UpdateCarAsync(UpdateCarRequest request);
        Task<bool> DeleteCarAsync(int carId);
        Task<CalculateCostResponse> CalculateCostAsync(CalculateCostRequest request);
    }
}