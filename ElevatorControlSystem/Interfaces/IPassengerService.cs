using ElevatorControlSystem.Models;

namespace ElevatorControlSystem.Interfaces;
public interface IPassengerService
{
    List<Passenger> GetPassengers();
    Passenger AddPassenger(ElevatorRequest request);
}