namespace ElevatorControlSystem.Models
{
    public class Elevator
    {
        public int Id { get; set; }
        public int CurrentFloor { get; set; }
        public ElevatorState State { get; set; }
        public List<Passenger> Passengers { get; set; }

        public Elevator(int id, int startFloor = 1)
        {
            Id = id;
            CurrentFloor = startFloor;
            State = ElevatorState.Idle;
            Passengers = new List<Passenger>();
        }
        
    }
}