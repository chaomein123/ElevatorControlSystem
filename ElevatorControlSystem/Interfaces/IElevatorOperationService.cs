using ElevatorControlSystem.Models;

namespace ElevatorControlSystem.Interfaces;

public interface IElevatorOperationService
{
    Task ServeElevatorAsync(Elevator elevator);
    Task MonitorElevatorsAsync();
}