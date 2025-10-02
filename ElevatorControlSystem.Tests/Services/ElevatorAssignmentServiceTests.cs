using Microsoft.VisualStudio.TestTools.UnitTesting;
using ElevatorControlSystem.Services;
using ElevatorControlSystem.Models;
using System.Collections.Generic;

namespace ElevatorControlSystem.Tests.Services
{
    [TestClass]
    public class ElevatorAssignmentServiceTests
    {
        private ElevatorAssignmentService _service;

        [TestInitialize]
        public void Setup()
        {
            _service = new ElevatorAssignmentService();
        }

        [TestMethod]
        public void AssignNearestElevator_ShouldReturnIdleElevator_WhenElevatorsAreIdle()
        {
            // Arrange
            var elevators = new List<Elevator>
            {
                new Elevator(1, 2) { State = ElevatorState.Idle },
                new Elevator(2, 5) { State = ElevatorState.Idle }
            };
            int startFloor = 4;
            int destinationFloor = 8;

            // Act
            var result = _service.AssignNearestElevator(elevators, startFloor, destinationFloor);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.CurrentFloor); // Nearest to startFloor 4
        }

        [TestMethod]
        public void AssignNearestElevator_ShouldReturnElevatorMovingInSameDirection_WhenAvailable()
        {
            // Arrange
            var elevators = new List<Elevator>
            {
                new Elevator(1, 3) { State = ElevatorState.MovingUp },
                new Elevator(2, 6) { State = ElevatorState.MovingDown }
            };
            int startFloor = 4;
            int destinationFloor = 8;

            // Act
            var result = _service.AssignNearestElevator(elevators, startFloor, destinationFloor);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.CurrentFloor); // Moving up elevator chosen
        }

        [TestMethod]
        public void AssignNearestElevator_ShouldReturnNearestElevator_WhenNoSuitableDirection()
        {
            // Arrange
            var elevators = new List<Elevator>
            {
                new Elevator(1, 1) { State = ElevatorState.MovingDown },
                new Elevator(2, 10) { State = ElevatorState.MovingDown }
            };
            int startFloor = 5;
            int destinationFloor = 8;

            // Act
            var result = _service.AssignNearestElevator(elevators, startFloor, destinationFloor);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.CurrentFloor); // Nearest elevator used as fallback
        }

        [TestMethod]
        public void AssignNearestElevator_ShouldReturnElevatorForPassengerGoingDown()
        {
            // Arrange
            var elevators = new List<Elevator>
            {
                new Elevator(1, 7) { State = ElevatorState.MovingDown },
                new Elevator(2, 3) { State = ElevatorState.MovingUp }
            };
            int startFloor = 6;
            int destinationFloor = 2;

            // Act
            var result = _service.AssignNearestElevator(elevators, startFloor, destinationFloor);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(7, result.CurrentFloor); // Moving down elevator chosen
        }

        [TestMethod]
        public void AssignNearestElevator_ShouldReturnNearestElevator_WhenMultipleEquallySuitable()
        {
            // Arrange
            var elevators = new List<Elevator>
            {
                new Elevator(1, 4) { State = ElevatorState.Idle },
                new Elevator(2, 4) { State = ElevatorState.Idle }
            };
            int startFloor = 5;
            int destinationFloor = 8;

            // Act
            var result = _service.AssignNearestElevator(elevators, startFloor, destinationFloor);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.CurrentFloor); // Either elevator is fine
        }
    }
}
