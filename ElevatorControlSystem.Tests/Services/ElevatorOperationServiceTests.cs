using Microsoft.VisualStudio.TestTools.UnitTesting;
using ElevatorControlSystem.Services;
using ElevatorControlSystem.Models;
using ElevatorControlSystem.Config;
using ElevatorControlSystem.Interfaces;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElevatorControlSystem.Tests.Services
{
    // Dummy status service for testing
    public class TestElevatorStatusService : IElevatorStatusService
    {
        private readonly List<Elevator> _elevators;
        public TestElevatorStatusService(List<Elevator> elevators)
        {
            _elevators = elevators;
        }
        public List<Elevator> GetElevators() => _elevators;
    }

    [TestClass]
    public class ElevatorOperationServiceTests
    {
        private ElevatorOperationService _service;
        private ElevatorSettings _settings;

        [TestInitialize]
        public void Setup()
        {
            _settings = new ElevatorSettings
            {
                DelayMs = 1,  // minimal delay for fast tests
                FloorCount = 10
            };
        }

        [TestMethod]
        public async Task ServeElevatorAsync_ShouldLoadAndUnloadPassengers()
        {
            // Arrange
            var passengers = new List<Passenger>
            {
                new Passenger(1, new ElevatorRequest(1, 5)),
                new Passenger(2, new ElevatorRequest(2, 3))
            };

            var elevator = new Elevator(1, 1)
            {
                State = ElevatorState.Idle,
                Passengers = passengers
            };

            var statusService = new TestElevatorStatusService(new List<Elevator> { elevator });
            var options = Options.Create(_settings);

            // Disable monitoring loop in tests
            _service = new ElevatorOperationService(statusService, options, startMonitoring: false);

            // Act
            await _service.ServeElevatorAsync(elevator);

            // Assert
            Assert.AreEqual(ElevatorState.Idle, elevator.State);
            Assert.AreEqual(0, elevator.Passengers.Count);
            foreach (var p in passengers)
                Assert.AreEqual(PassengerStatus.Disembarked, p.Status);
        }

        [TestMethod]
        public async Task ServeElevatorAsync_ShouldMoveUpAndDownCorrectly()
        {
            // Arrange
            var passengers = new List<Passenger>
            {
                new Passenger(1, new ElevatorRequest(2, 5)),
                new Passenger(2, new ElevatorRequest(5, 2))
            };

            var elevator = new Elevator(1, 2)
            {
                State = ElevatorState.Idle,
                Passengers = passengers
            };

            var statusService = new TestElevatorStatusService(new List<Elevator> { elevator });
            var options = Options.Create(_settings);

            _service = new ElevatorOperationService(statusService, options, startMonitoring: false);

            // Act
            await _service.ServeElevatorAsync(elevator);

            // Assert
            Assert.AreEqual(ElevatorState.Idle, elevator.State);
            Assert.AreEqual(0, elevator.Passengers.Count);
            foreach (var p in passengers)
                Assert.AreEqual(PassengerStatus.Disembarked, p.Status);
        }
    }
}
