using ElevatorControlSystem.Models;

namespace ElevatorControlSystem.Interfaces;

public interface IElevatorRepository
{
    List<Elevator> Elevators { get; }
    List<Passenger> Passengers { get; }
    int NextPassengerId();
}