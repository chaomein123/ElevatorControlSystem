using ElevatorControlSystem.Models;
using ElevatorControlSystem.Interfaces;

namespace ElevatorControlSystem.Services
{
    public class ElevatorRequestService : IElevatorRequestService
    {
        private readonly IElevatorStatusService _statusService;
        private readonly IPassengerService _passengerService;
        private readonly IElevatorAssignmentService _assignmentService;

        public ElevatorRequestService(
            IElevatorStatusService statusService,
            IPassengerService passengerService,
            IElevatorAssignmentService assignmentService)
        {
            _statusService = statusService;
            _passengerService = passengerService;
            _assignmentService = assignmentService;
        }

        public Task RequestElevatorAsync(ElevatorRequest request)
        {
            var elevators = _statusService.GetElevators();
            var passenger = _passengerService.AddPassenger(request);

            var assignedElevator = _assignmentService.AssignNearestElevator(
                elevators, request.StartFloor, request.DestinationFloor);

            passenger.AssignedElevatorId = assignedElevator.Id;
            assignedElevator.Passengers.Add(passenger);

            return Task.CompletedTask;
        }
    }

}
