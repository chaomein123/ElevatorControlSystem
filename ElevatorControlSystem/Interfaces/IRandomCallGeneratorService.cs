using ElevatorControlSystem.Models;

namespace ElevatorControlSystem.Interfaces;
public interface IRandomCallGeneratorService
{
    Task GenerateRandomCallAsync();
}
