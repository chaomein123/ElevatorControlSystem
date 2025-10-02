namespace ElevatorControlSystem.Models
{
    public class ElevatorRequest
    {
        /// <summary>
        /// Floor where the passenger is waiting.
        /// </summary>
        public int StartFloor { get; set; }

        /// <summary>
        /// Floor where the passenger wants to go.
        /// </summary>
        public int DestinationFloor { get; set; }

        /// <summary>
        /// Direction of travel (Up or Down).
        /// </summary>
        public Direction Direction { get; set; }

        
        public ElevatorRequest(int startFloor, int destinationFloor)
        {
            StartFloor = startFloor;
            DestinationFloor = destinationFloor;

            // Automatically set direction
            Direction = destinationFloor > startFloor ? Direction.Up : Direction.Down;
        }
    }
}