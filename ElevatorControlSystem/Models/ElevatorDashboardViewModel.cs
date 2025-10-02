using ElevatorControlSystem.Models;

namespace ElevatorControlSystem.Models;

public class ElevatorDashboardViewModel
{
    public List<Elevator> Elevators { get; set; }
    public List<Passenger> Passengers { get; set; }

    public ElevatorDashboardViewModel()
    {
        Elevators = new List<Elevator>();
        Passengers = new List<Passenger>();
    }
}
