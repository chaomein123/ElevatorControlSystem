namespace ElevatorControlSystem.Models
{
    public class Passenger
    {
        public int Id { get; set; }
        public ElevatorRequest Request { get; set; }
        public PassengerStatus Status { get; set; }
        public int? AssignedElevatorId { get; set; }
        public int CurrentFloor { get; set; }
        public int DestinationFloor { get; set; }

        public Passenger(int id, ElevatorRequest request)
        {
            Id = id;
            Request = request;
            Status = PassengerStatus.Waiting;
            AssignedElevatorId = null;

            // Initialize floors
            CurrentFloor = request.StartFloor;
            DestinationFloor = request.DestinationFloor;
        }
    }
}
