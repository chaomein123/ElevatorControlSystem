using ElevatorControlSystem.Models;
using ElevatorControlSystem.Interfaces;

namespace ElevatorControlSystem.Services
{
    public class ElevatorAssignmentService : IElevatorAssignmentService
    {
        public Elevator AssignNearestElevator(List<Elevator> elevators, int startFloor, int destinationFloor)
        {
            bool passengerGoingUp = destinationFloor > startFloor;

            var suitableElevators = elevators.Where(e =>
                e.State == ElevatorState.Idle ||
                (passengerGoingUp && (e.State == ElevatorState.MovingUp || e.State == ElevatorState.LoadingUp)) ||
                (!passengerGoingUp && (e.State == ElevatorState.MovingDown || e.State == ElevatorState.LoadingDown))
            ).ToList();

            if (!suitableElevators.Any())
            {
                suitableElevators = elevators.ToList();
            }

            return suitableElevators
                .OrderBy(e => Math.Abs(e.CurrentFloor - startFloor))
                .First();
        }
    }
}
