using ElevatorControlSystem.Models;

namespace ElevatorControlSystem.Interfaces;

public interface IElevatorRequestService
{
    Task RequestElevatorAsync(ElevatorRequest request);
}