using ElevatorControlSystem.Models;
using ElevatorControlSystem.Interfaces;

namespace ElevatorControlSystem.Services
{
    public class PassengerService : IPassengerService
    {
        private readonly IElevatorRepository _repo;
        
        public PassengerService(IElevatorRepository repo)
        {
            _repo = repo;
        }

        public List<Passenger> GetPassengers() => _repo.Passengers;

        public Passenger AddPassenger(ElevatorRequest request)
        {
            var passenger = new Passenger(_repo.NextPassengerId(), request);
            _repo.Passengers.Add(passenger);
            return passenger;
        }
    }
}
