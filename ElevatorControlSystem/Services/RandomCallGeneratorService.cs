using ElevatorControlSystem.Models;
using ElevatorControlSystem.Interfaces;
using ElevatorControlSystem.Config;
using Microsoft.Extensions.Options;

namespace ElevatorControlSystem.Services
{
    public class RandomCallGeneratorService : IRandomCallGeneratorService
    {
        private readonly IElevatorRequestService _requestService; // Service that processes elevator requests
        private readonly Random _random;
        private readonly ElevatorSettings _settings;

        public RandomCallGeneratorService(
            IElevatorRequestService requestService,
            IOptions<ElevatorSettings> settings
        )
        {
            _requestService = requestService;
            _settings = settings.Value;
            _random = new Random();
        }

        public async Task GenerateRandomCallAsync()
        {
            int totalFloors = _settings.FloorCount; 
            int startFloor = _random.Next(1, totalFloors + 1);
            int destinationFloor;

            // Ensure destination is not the same as start
            do
            {
                destinationFloor = _random.Next(1, totalFloors + 1);
            } while (destinationFloor == startFloor);

            var request = new ElevatorRequest(startFloor, destinationFloor);

            Console.WriteLine($"[RandomCallGenerator] Requesting elevator from Floor {startFloor} to Floor {destinationFloor}");

            await _requestService.RequestElevatorAsync(request);
        }
    }
}
