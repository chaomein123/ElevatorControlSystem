using Microsoft.VisualStudio.TestTools.UnitTesting;
using ElevatorControlSystem.Services;
using ElevatorControlSystem.Models;
using ElevatorControlSystem.Interfaces;
using System.Collections.Generic;

namespace ElevatorControlSystem.Tests.Services
{
    // Unique dummy repository for this test file
    public class TestElevatorRepositoryForPassengerService : IElevatorRepository
    {
        public List<Elevator> Elevators { get; } = new List<Elevator>();
        public List<Passenger> Passengers { get; } = new List<Passenger>();
        private int _counter = 1;

        public int NextPassengerId() => _counter++;
    }

    [TestClass]
    public class PassengerServiceTests
    {
        private PassengerService _service;
        private TestElevatorRepositoryForPassengerService _repo;

        [TestInitialize]
        public void Setup()
        {
            _repo = new TestElevatorRepositoryForPassengerService();
            _service = new PassengerService(_repo);
        }

        [TestMethod]
        public void GetPassengers_ShouldReturnAllPassengersFromRepository()
        {
            // Arrange
            _repo.Passengers.Add(new Passenger(1, new ElevatorRequest(1, 5)));
            _repo.Passengers.Add(new Passenger(2, new ElevatorRequest(3, 7)));

            // Act
            var passengers = _service.GetPassengers();

            // Assert
            Assert.IsNotNull(passengers);
            Assert.AreEqual(2, passengers.Count);
            Assert.AreEqual(1, passengers[0].Id);
            Assert.AreEqual(2, passengers[1].Id);
        }

        [TestMethod]
        public void AddPassenger_ShouldCreateAndAddPassengerToRepository()
        {
            // Arrange
            var request = new ElevatorRequest(2, 5);

            // Act
            var passenger = _service.AddPassenger(request);

            // Assert
            Assert.IsNotNull(passenger);
            Assert.AreEqual(1, passenger.Id);
            Assert.AreEqual(request.StartFloor, passenger.CurrentFloor);
            Assert.AreEqual(request.DestinationFloor, passenger.DestinationFloor);
            Assert.AreEqual(1, _repo.Passengers.Count);
            Assert.AreSame(passenger, _repo.Passengers[0]);
        }

        [TestMethod]
        public void AddPassenger_ShouldIncrementPassengerId()
        {
            // Arrange
            var request1 = new ElevatorRequest(1, 4);
            var request2 = new ElevatorRequest(2, 5);

            // Act
            var passenger1 = _service.AddPassenger(request1);
            var passenger2 = _service.AddPassenger(request2);

            // Assert
            Assert.AreEqual(1, passenger1.Id);
            Assert.AreEqual(2, passenger2.Id);
        }
    }
}
