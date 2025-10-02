using Microsoft.VisualStudio.TestTools.UnitTesting;
using ElevatorControlSystem.Services;
using ElevatorControlSystem.Models;
using ElevatorControlSystem.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElevatorControlSystem.Tests.Services
{
    // Dummy status service for testing
    public class TestStatusService : IElevatorStatusService
    {
        public List<Elevator> Elevators { get; } = new();
        public List<Elevator> GetElevators() => Elevators;
    }

    // Dummy passenger service for testing
    public class TestPassengerService : IPassengerService
    {
        private int _counter = 1;
        private readonly List<Passenger> _passengers = new();

        public List<Passenger> GetPassengers() => _passengers;

        public Passenger AddPassenger(ElevatorRequest request)
        {
            // Use the real constructor: (int id, ElevatorRequest request)
            var passenger = new Passenger(_counter++, request);
            _passengers.Add(passenger);
            return passenger;
        }
    }

    // Dummy assignment service for testing
    public class TestAssignmentService : IElevatorAssignmentService
    {
        public Elevator AssignNearestElevator(List<Elevator> elevators, int startFloor, int destinationFloor)
        {
            // Return the first elevator for simplicity
            return elevators[0];
        }
    }

    [TestClass]
    public class ElevatorRequestServiceTests
    {
        private ElevatorRequestService _service;
        private TestStatusService _statusService;
        private TestPassengerService _passengerService;
        private TestAssignmentService _assignmentService;

        [TestInitialize]
        public void Setup()
        {
            _statusService = new TestStatusService();
            _passengerService = new TestPassengerService();
            _assignmentService = new TestAssignmentService();

            // create elevators
            _statusService.Elevators.Add(new Elevator(1, 1));
            _statusService.Elevators.Add(new Elevator(2, 3));

            _service = new ElevatorRequestService(_statusService, _passengerService, _assignmentService);
        }

        [TestMethod]
        public async Task RequestElevatorAsync_ShouldAssignPassengerToNearestElevator()
        {
            // Arrange
            var request = new ElevatorRequest(2, 5); // <-- fixed constructor usage

            // Act
            await _service.RequestElevatorAsync(request);

            // Assert
            var elevator = _statusService.Elevators[0]; // first elevator chosen
            Assert.AreEqual(1, elevator.Passengers.Count);
            Assert.AreEqual(elevator.Id, elevator.Passengers[0].AssignedElevatorId);
            Assert.AreEqual(request.StartFloor, elevator.Passengers[0].CurrentFloor);
            Assert.AreEqual(request.DestinationFloor, elevator.Passengers[0].DestinationFloor);
        }
    }
}
