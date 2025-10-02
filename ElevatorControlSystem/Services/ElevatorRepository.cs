using ElevatorControlSystem.Interfaces;
using ElevatorControlSystem.Models;
using ElevatorControlSystem.Config;
using Microsoft.Extensions.Options;

namespace ElevatorControlSystem.Services
{
    public class ElevatorRepository : IElevatorRepository
    {
        public List<Elevator> Elevators { get; } = new();
        public List<Passenger> Passengers { get; } = new();
        private readonly ElevatorSettings _settings;
        private int _passengerCounter = 1;
        public ElevatorRepository(IOptions<ElevatorSettings> settings)
        {
            _settings = settings.Value;
            
            for (int i = 1; i <= _settings.ElevatorCount; i++)
                Elevators.Add(new Elevator(i));
        }
        public int NextPassengerId() => _passengerCounter++;
    }
}