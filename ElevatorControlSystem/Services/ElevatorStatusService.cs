using ElevatorControlSystem.Models;
using ElevatorControlSystem.Interfaces;

namespace ElevatorControlSystem.Services
{
    public class ElevatorStatusService : IElevatorStatusService
    {
        private readonly IElevatorRepository _repo;

        public ElevatorStatusService(IElevatorRepository repo)
        {
            _repo = repo;
        }

        public List<Elevator> GetElevators() => _repo.Elevators;
    }
}
