using ElevatorControlSystem.Models;
using ElevatorControlSystem.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ElevatorControlSystem.Controllers
{
    public class ElevatorController : Controller
    {
        private readonly IElevatorStatusService _statusService;
        private readonly IPassengerService _passengerService;
        private readonly IElevatorRequestService _requestService;
        private readonly IElevatorOperationService _operationService;
        private readonly IRandomCallGeneratorService _randomCallGenerator;
        private static bool _monitorStarted = false;

        public ElevatorController(
            IElevatorStatusService statusService,
            IPassengerService passengerService,
            IElevatorRequestService requestService,
            IElevatorOperationService operationService,
            IRandomCallGeneratorService randomCallGenerator)
        {
            _statusService = statusService;
            _passengerService = passengerService;
            _requestService = requestService;
            _randomCallGenerator = randomCallGenerator;

            _operationService = operationService;

            // ensure it only starts once
            if (!_monitorStarted)
            {
                _monitorStarted = true;
                _ = _operationService.MonitorElevatorsAsync();
            }
        }

        public IActionResult Index()
        {
            var elevators = _statusService.GetElevators();
            var passengers = _passengerService.GetPassengers();

            var model = new ElevatorDashboardViewModel
            {
                Elevators = elevators,
                Passengers = passengers
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult RequestElevator(int startFloor, int destinationFloor)
        {
            var request = new ElevatorRequest(startFloor, destinationFloor);
            _ = _requestService.RequestElevatorAsync(request);

            TempData["Message"] = "Elevator requested!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetElevatorsStatus()
        {
            var elevators = _statusService.GetElevators()
                .Select(e => new
                {
                    id = e.Id,
                    currentFloor = e.CurrentFloor,
                    state = e.State.ToString(),
                    passengerIds = e.Passengers.Select(p => p.Id).ToList()
                });

            return Json(elevators);
        }

        [HttpGet]
        public IActionResult GetPassengersStatus()
        {
            var passengers = _passengerService.GetPassengers()
                .Select(p => new
                {
                    id = p.Id,
                    request = new
                    {
                        startFloor = p.Request.StartFloor,
                        destinationFloor = p.Request.DestinationFloor
                    },
                    status = p.Status.ToString(),
                    assignedElevatorId = p.AssignedElevatorId
                });

            return Json(passengers);
        }
        [HttpPost]
        public async Task<IActionResult> GenerateRandomCall()
        {
            await _randomCallGenerator.GenerateRandomCallAsync();
            TempData["Message"] = "Random elevator call generated!";
            return RedirectToAction("Index", "Elevator");
        }
    }
}
