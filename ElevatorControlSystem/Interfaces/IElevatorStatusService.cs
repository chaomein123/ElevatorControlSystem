using ElevatorControlSystem.Models;

namespace ElevatorControlSystem.Interfaces;
public interface IElevatorStatusService
{
    List<Elevator> GetElevators();
}