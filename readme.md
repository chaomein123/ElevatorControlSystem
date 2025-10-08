# Elevator Control System

A simple elevator control system built using **ASP.NET MVC**, demonstrating the use of **SOLID principles** and unit testing with **MSTest**. This project allows users to simulate elevator operations in a building with customizable settings.

---

## Technologies Used

- **ASP.NET** – Web application framework  
- **MVC Pattern** – Model-View-Controller architecture for separation of concerns  
- **SOLID Principles** – Ensuring clean, maintainable, and scalable code  
- **MSTest** – Unit testing framework for verifying functionality  

---

## Getting Started

### Prerequisites

- Visual Studio Code (VSCode) or Visual Studio 2022 or later  
- .NET 8  
- Browser to test the application  

---

### Configuration

User-configurable settings are available in the `appsettings.json` file:

```json
"ElevatorSettings": {
  "DelayMs": 5000,
  "ElevatorCount": 4,
  "FloorCount": 10
}
```

- **DelayMs**: Time (in milliseconds) the elevator waits between moves.  
- **ElevatorCount**: Number of elevators in the building.  
- **FloorCount**: Total number of floors in the building.  

You can modify these values to simulate different building scenarios.

---

### Running the Application

1. Open the solution in VSCode or Visual Studio.  
2. Restore NuGet packages if needed.  
3. Set the project as the startup project.  
4. Press **F5** to run the application.  
5. Interact with the elevators through the browser interface.
---

### Data Tables

#### **ElevatorsInfo Table**
| Field | Description |
|--------|-------------|
| **ID** | The unique identifier of the elevator. |
| **Current Floor** | The current floor where the elevator is located. |
| **State** | The current state of the elevator. Possible values:  
• *Idle* – Elevator is not moving and waiting for requests.  
• *Loading Up* – Elevator is loading passengers that are going up (not moving yet).  
• *Loading Down* – Elevator is loading passengers that are going down (not moving yet).  
• *Moving Up* – Elevator is currently moving upward.  
• *Moving Down* – Elevator is currently moving downward. |
| **Assigned Passengers** | List of passengers that are assigned to this elevator for loading and unloading. |

#### **Passengers Table**
| Field | Description |
|--------|-------------|
| **ID** | The unique identifier of the passenger. |
| **StartFloor** | The floor where the passenger is currently located. |
| **Destination Floor** | The floor that the passenger requested to go to. |
| **Status** | The passenger's current status. Possible values:  
• *Waiting* – Waiting for an elevator to arrive.  
• *InElevator* – Passenger is currently inside the elevator.  
• *Disembarked* – Passenger has reached their destination floor and exited the elevator. |
| **Assigned Elevator** | The ID of the elevator assigned to serve this passenger. |

---

### Running Tests

Unit tests are implemented using **MSTest** for the services only.  

To run tests:

1. Open **Test Explorer** in Visual Studio or VSCode with the .NET Test Explorer extension.  
2. Build the solution.  
3. Run all tests to verify the elevator logic works correctly.  

You can check the code coverage in `coverage-report/index.html`.

---

### Features

- Multiple elevators serving multiple floors  
- Passengers can request elevators to move up or down  
- Elevators optimize serving passengers based on direction and proximity  
- Fully customizable via `appsettings.json`  

---

### Notes

- Elevator movement is simulated asynchronously.  
- Ensure `DelayMs` is reasonable to observe elevator operations without too fast or too slow simulation.

