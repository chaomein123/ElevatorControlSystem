using ElevatorControlSystem.Models;
using ElevatorControlSystem.Interfaces;
using ElevatorControlSystem.Config;
using Microsoft.Extensions.Options;

namespace ElevatorControlSystem.Services
{
    public class ElevatorOperationService : IElevatorOperationService
    {
        private readonly IElevatorStatusService _statusService;
        private readonly ElevatorSettings _settings;
        private readonly bool _startMonitoring;

        public ElevatorOperationService(
            IElevatorStatusService statusService,
            IOptions<ElevatorSettings> settings,
            bool startMonitoring = true)
        {
            _statusService = statusService;
            _settings = settings.Value;
            _startMonitoring = startMonitoring;

            if (_startMonitoring)
            {
                _ = MonitorElevatorsAsync(); // only start in production, not in tests
            }
        }

        public async Task MonitorElevatorsAsync()
        {
            while (true)
            {
                var elevators = _statusService.GetElevators();

                var tasks = elevators
                    .Where(e => e.State == ElevatorState.Idle && e.Passengers.Any())
                    .Select(e => ServeElevatorAsync(e))
                    .ToList();

                await Task.WhenAll(tasks);

                await Task.Delay(1000);
            }
        }

        public async Task ServeElevatorAsync(Elevator elevator)
        {
            List<int> floorsPassed = new List<int>();
            if (!elevator.Passengers.Any()) return;

            while (elevator.Passengers.Any())
            {

                // Determine direction based on the first passenger (pickup if waiting, destination if already in elevator)
                var firstPassenger = elevator.Passengers.FirstOrDefault();
                if (firstPassenger == null) break; // no passengers, exit

                Console.WriteLine($"Elevator requested is at Floor:{firstPassenger.CurrentFloor} by Passenger: {firstPassenger.Id}.");

                bool movingUp = firstPassenger is not null
                    ? (firstPassenger.CurrentFloor == elevator.CurrentFloor
                        ? firstPassenger.DestinationFloor > firstPassenger.CurrentFloor
                        : firstPassenger.CurrentFloor > elevator.CurrentFloor)
                    : true;

                bool batchDirection = firstPassenger != null && firstPassenger.DestinationFloor > firstPassenger.CurrentFloor;

                while (true)
                {

                    Console.WriteLine($"Elevator is moving now and now at Floor:{elevator.CurrentFloor}.");
                    // Clamp current floor
                    elevator.CurrentFloor = Math.Max(1, Math.Min(10, elevator.CurrentFloor));

                    // Load passengers at current floor going same direction
                    await LoadPassengersAsync(elevator, batchDirection);

                    // Unload passengers at current floor
                    await UnloadPassengersAsync(elevator, batchDirection);

                    var priorityPassengerIds = elevator.Passengers
                        .Where(p => p.Status == PassengerStatus.InElevator ||
                                (p.Status == PassengerStatus.Waiting && ((batchDirection && p.DestinationFloor > p.CurrentFloor) || (!batchDirection && p.DestinationFloor < p.CurrentFloor))))
                        .Select(p => p.Id)
                        .ToList();

                    bool allDisembarked = elevator.Passengers
                        .Where(p => priorityPassengerIds.Contains(p.Id))
                        .All(p => p.Status == PassengerStatus.Disembarked);

                    var remainingPassengerIds = elevator.Passengers
                        .Where(p => priorityPassengerIds.Contains(p.Id) && p.Status != PassengerStatus.Disembarked)
                        .Select(p => p.Id)
                        .ToList();

                    Console.ForegroundColor = remainingPassengerIds.Count == 0 ? ConsoleColor.Green : ConsoleColor.Yellow;
                    Console.WriteLine($"[BATCH STATUS] Elevator {elevator.Id} batch {(remainingPassengerIds.Count == 0 ? "completed ✅" : "still in progress ⏳")} | Remaining Passenger IDs: {string.Join(", ", remainingPassengerIds)}");
                    Console.ResetColor();

                    if (allDisembarked)
                    {
                        // Batch finished
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"[BATCH COMPLETE] Elevator {elevator.Id} finished batch. Floors passed: {string.Join(", ", floorsPassed)}");
                        Console.ResetColor();

                        // Reset for next batch
                        floorsPassed.Clear();
                        break; // exit inner loop
                    }

                    // Check if there are any waiting passengers in the batch direction
                    bool hasWaitingPassengerInBatchDirection = elevator.Passengers.Any(p => p.Status == PassengerStatus.Waiting &&
                        ((batchDirection && p.DestinationFloor > p.CurrentFloor) ||
                        (!batchDirection && p.DestinationFloor < p.CurrentFloor)));

                    // If there are waiting passengers in batch direction, move elevator toward the nearest one
                    if (hasWaitingPassengerInBatchDirection)
                    {
                        // Get nearest passenger in batch direction
                        var nearestPassenger = elevator.Passengers
                            .Where(p => p.Status == PassengerStatus.Waiting &&
                                        ((batchDirection && p.DestinationFloor > p.CurrentFloor) ||
                                        (!batchDirection && p.DestinationFloor < p.CurrentFloor)))
                            .OrderBy(p => Math.Abs(p.CurrentFloor - elevator.CurrentFloor))
                            .First();

                        if (!floorsPassed.Contains(nearestPassenger.CurrentFloor))
                        {
                            // Passenger is ahead or on a floor not yet passed
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($"[SERVE] Elevator {elevator.Id} will move toward floor {nearestPassenger.CurrentFloor} for passenger {nearestPassenger.Id}");
                            Console.ResetColor();

                            movingUp = nearestPassenger.CurrentFloor > elevator.CurrentFloor;
                        }
                        else
                        {
                            if (!HasWaitingPassengersInDirection(elevator, movingUp))
                            {
                                Console.WriteLine($"[NO WAITING PASSENGERS] Elevator {elevator.Id} has no waiting passengers in direction {(movingUp ? "UP" : "DOWN")}");
                                movingUp = nearestPassenger.CurrentFloor > elevator.CurrentFloor;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"[SKIP] Elevator {elevator.Id} already passed floor {nearestPassenger.CurrentFloor} of passenger {nearestPassenger.Id}");
                                Console.ResetColor();
                            }
                        }

                    }

                    // Add the floor to the list if not already added
                    if (!floorsPassed.Contains(elevator.CurrentFloor))
                        floorsPassed.Add(elevator.CurrentFloor);

                    // Log the current state of floors passed
                    Console.WriteLine($"[OVERALL FLOORS PASSED] Elevator {elevator.Id}: {string.Join(", ", floorsPassed)}");

                    Console.WriteLine($"[Current Floor] Elevator {elevator.CurrentFloor} : {movingUp}");

                    movingUp = elevator.CurrentFloor == _settings.FloorCount ? false : elevator.CurrentFloor == 1 ? true : movingUp;
                    // Move one floor
                    if (movingUp)
                    {
                        elevator.CurrentFloor = Math.Min(elevator.CurrentFloor + 1, _settings.FloorCount);
                    }
                    else
                    {
                        elevator.CurrentFloor = Math.Max(elevator.CurrentFloor - 1, 1);
                    }

                    elevator.State = movingUp ? ElevatorState.MovingUp : ElevatorState.MovingDown;
                    await Task.Delay(_settings.DelayMs);
                }
            }
            Console.WriteLine($"[IDLE] Elevator {elevator.Id} is IDLING -------------------------------------------------");
            elevator.State = ElevatorState.Idle;
        }
        private void SortPassengersByBatchDirection(Elevator elevator, bool batchDirection)
        {
            Console.WriteLine($"Sorting list now.");
            if (!elevator.Passengers.Any())
                return;

            if (batchDirection)
            {
                elevator.Passengers = elevator.Passengers
                    .OrderByDescending(p => p.DestinationFloor > p.CurrentFloor)
                    .ThenBy(p => p.DestinationFloor)
                    .ThenByDescending(p => p.DestinationFloor)
                    .ToList();
            }
            else
            {
                elevator.Passengers = elevator.Passengers
                    .OrderByDescending(p => p.DestinationFloor < p.CurrentFloor)
                    .ThenByDescending(p => p.DestinationFloor)
                    .ThenBy(p => p.DestinationFloor)
                    .ToList();
            }
        }
        private static bool HasWaitingPassengersInDirection(Elevator elevator, bool movingUp) =>
            elevator.Passengers.Any(p => p.Status == PassengerStatus.Waiting &&
                                        (movingUp ? p.CurrentFloor > elevator.CurrentFloor
                                                : p.CurrentFloor < elevator.CurrentFloor));
        private async Task LoadPassengersAsync(Elevator elevator, bool batchDirection)
        {
            var passengersHere = elevator.Passengers
                .Where(p => p.CurrentFloor == elevator.CurrentFloor && p.Status == PassengerStatus.Waiting)
                .ToList();

            if (!passengersHere.Any()) return;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[LOADING] Elevator {elevator.Id} loading {passengersHere.Count} passenger(s) at floor {elevator.CurrentFloor}. IDs: {string.Join(", ", passengersHere.Select(p => p.Id))}");
            Console.ResetColor();

            elevator.State = batchDirection ? ElevatorState.LoadingUp : ElevatorState.LoadingDown;
            await Task.Delay(_settings.DelayMs);

            foreach (var passenger in passengersHere)
                passenger.Status = PassengerStatus.InElevator;

            SortPassengersByBatchDirection(elevator, batchDirection);
        }
        private async Task UnloadPassengersAsync(Elevator elevator, bool batchDirection)
        {
            var passengersToUnload = elevator.Passengers
                .Where(p => p.DestinationFloor == elevator.CurrentFloor && p.Status == PassengerStatus.InElevator)
                .ToList();

            if (!passengersToUnload.Any()) return;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[UNLOADING] Elevator {elevator.Id} unloading {passengersToUnload.Count} passenger(s) at floor {elevator.CurrentFloor}. IDs: {string.Join(", ", passengersToUnload.Select(p => p.Id))}");
            Console.ResetColor();

            elevator.State = ElevatorState.Unloading;
            await Task.Delay(_settings.DelayMs);

            foreach (var passenger in passengersToUnload)
                passenger.Status = PassengerStatus.Disembarked;

            elevator.Passengers.RemoveAll(p => p.Status == PassengerStatus.Disembarked);
            SortPassengersByBatchDirection(elevator, batchDirection);
        }
    }
}
