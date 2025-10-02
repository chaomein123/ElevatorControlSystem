using Microsoft.VisualStudio.TestTools.UnitTesting;
using ElevatorControlSystem.Services;
using ElevatorControlSystem.Models;
using ElevatorControlSystem.Interfaces;
using ElevatorControlSystem.Config;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace ElevatorControlSystem.Tests.Services
{
    // Dummy repository for testing
    public class TestElevatorRepository : IElevatorRepository
    {
        public List<Elevator> Elevators { get; } = new List<Elevator>();
        public List<Passenger> Passengers { get; } = new List<Passenger>();
        private int _counter = 1;

        public int NextPassengerId() => _counter++;
    }

    [TestClass]
    public class ElevatorStatusServiceTests
    {
        private ElevatorStatusService _service;
        private TestElevatorRepository _repo;

        [TestInitialize]
        public void Setup()
        {
            _repo = new TestElevatorRepository();
            _repo.Elevators.Add(new Elevator(1, 1));
            _repo.Elevators.Add(new Elevator(2, 3));

            _service = new ElevatorStatusService(_repo);
        }

        [TestMethod]
        public void GetElevators_ShouldReturnAllElevatorsFromRepository()
        {
            // Act
            var elevators = _service.GetElevators();

            // Assert
            Assert.IsNotNull(elevators);
            Assert.AreEqual(2, elevators.Count);
            Assert.AreEqual(1, elevators[0].Id);
            Assert.AreEqual(2, elevators[1].Id);
        }
    }
}
