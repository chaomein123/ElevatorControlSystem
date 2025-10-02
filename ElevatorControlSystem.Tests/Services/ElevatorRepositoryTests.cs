using Microsoft.VisualStudio.TestTools.UnitTesting;
using ElevatorControlSystem.Services;
using ElevatorControlSystem.Models;
using ElevatorControlSystem.Config;
using Microsoft.Extensions.Options;
using System.Linq;

namespace ElevatorControlSystem.Tests.Services
{
    [TestClass]
    public class ElevatorRepositoryTests
    {
        private ElevatorRepository _repository;
        private ElevatorSettings _settings;

        [TestInitialize]
        public void Setup()
        {
            _settings = new ElevatorSettings
            {
                ElevatorCount = 3,
                FloorCount = 10,
                DelayMs = 1
            };
            var options = Options.Create(_settings);
            _repository = new ElevatorRepository(options);
        }

        [TestMethod]
        public void ElevatorRepository_ShouldInitializeElevators()
        {
            // Assert
            Assert.IsNotNull(_repository.Elevators);
            Assert.AreEqual(_settings.ElevatorCount, _repository.Elevators.Count);
            
            // Ensure elevator IDs are correct
            for (int i = 0; i < _settings.ElevatorCount; i++)
            {
                Assert.AreEqual(i + 1, _repository.Elevators[i].Id);
                Assert.AreEqual(1, _repository.Elevators[i].CurrentFloor); // assuming default floor is 1
            }
        }

        [TestMethod]
        public void ElevatorRepository_ShouldInitializePassengersListEmpty()
        {
            // Assert
            Assert.IsNotNull(_repository.Passengers);
            Assert.AreEqual(0, _repository.Passengers.Count);
        }

        [TestMethod]
        public void NextPassengerId_ShouldIncrementCorrectly()
        {
            // Act & Assert
            int id1 = _repository.NextPassengerId();
            int id2 = _repository.NextPassengerId();
            int id3 = _repository.NextPassengerId();

            Assert.AreEqual(1, id1);
            Assert.AreEqual(2, id2);
            Assert.AreEqual(3, id3);
        }
    }
}
