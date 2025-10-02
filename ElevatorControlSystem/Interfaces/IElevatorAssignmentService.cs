using ElevatorControlSystem.Models;

namespace ElevatorControlSystem.Interfaces;

public interface IElevatorAssignmentService
{
    Elevator AssignNearestElevator(List<Elevator> elevators, int startFloor, int destinationFloor);
}