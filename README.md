# Arganmehr Hospital Information System (HIS)

## Overview

Arganmehr HIS is a modular Hospital Information System designed to replace traditional hospital systems with a modern, scalable solution. The system is split into two main components:

- **Client:** Installed locally at each healthcare facility (hospital, clinic, or laboratory). Each client instance integrates specific modules (e.g., Operating Room, CCU, ICU, Laboratory) tailored to the facility's needs.
- **Server:** Centrally hosted to manage client installations. The server controls module activation/deactivation, configuration updates, and base data distribution across all nodes.

Built on C# and targeting the .NET Framework 4.5, the solution is developed in Visual Studio 2013 following a layered architecture for maintainability and scalability.

## Project Structure

The project is divided into distinct layers and projects to separate concerns:

- **DataLayer:**  
  Contains data access logic; it utilizes Entity Framework and performance profiling tools.
  
- **DomainClasses:**  
  Defines business models critical to hospital operations such as patients, departments, and staff.
  
- **ServiceLayer:**  
  Implements business logic that bridges between data access and presentation layers.
  
- **IocConfig:**  
  Manages dependency injection using StructureMap to keep the components loosely coupled.
  
- **Common & Utility:**  
  Provide shared helpers and utilities used across multiple modules.
  
- **AutoMapperProfiles:**  
  Uses AutoMapper to facilitate object-to-object mapping between domain models and ViewModels.
  
- **ViewModel & Web (Client Only):**  
  Hosts the ASP.NET MVC web application for client-side interactions.
  
- **Tests:**  
  Contains unit and integration tests to ensure the robustness of various components.

The Server solution mirrors much of the client’s structure to ensure seamless management and synchronization between systems.

## Key Dependencies

The project leverages numerous NuGet packages to enhance functionality:

| **Package**                         | **Description**                                                                       |
|-------------------------------------|---------------------------------------------------------------------------------------|
| **AngularJS & Bootstrap**           | Enhance UI interactivity and responsive design.                                      |
| **EntityFramework**                 | Simplifies data access and ORM-based development.                                     |
| **AutoMapper**                      | Reduces boilerplate by automating domain-to-ViewModel mapping.                         |
| **FluentValidation**                | Provides a fluent interface for input validation rules.                               |
| **Elmah & Elmah.Mvc**               | Enables comprehensive error logging and diagnostics.                                  |
| **Microsoft.AspNet.Identity**       | Manages authentication and authorization securely.                                    |
| **Owin & Microsoft.Owin.Security**  | Facilitates middleware-based authentication, including external login providers.       |
| **StructureMap**                    | Implements dependency injection for modularity and maintainability.                     |
| **Glimpse & MiniProfiler**          | Offer performance diagnostics and real-time debugging insights.                        |

## Installation and Setup

### Prerequisites

- Visual Studio 2013 (or later)
- .NET Framework 4.5
- NuGet Package Manager

### Steps

1. **Clone Repositories:**  
   Clone both the Client and Server projects from GitHub.
   ```bash
   git clone https://github.com/Kasraco/Arganmehr-HIS.git
   
   
# Installation and Setup
## Open Solutions
Open the solution files (`*.sln`) in Visual Studio.

## Restore NuGet Packages
Ensure all NuGet packages are restored (ideally, Visual Studio will prompt or you can run `nuget restore`).

## Build the Solution
Clean and build the projects to ensure all dependencies are correctly linked.

## Configure Settings
Update configuration files (e.g., `web.config`) with connection strings and other environment-specific settings.

## Deploy the Server Application
Set up the server on a central host to manage module configurations and base data distribution.

## Install the Client Application
Deploy client instances at the healthcare facilities. On first run, each client synchronizes with the server to receive its configuration and data.

---

# Configuration and Customization

## Central Administration
The Server provides an admin interface to manage the modules active on each client. Customize the functionality based on the facility’s requirements.

## Modular Activation
Modules can be dynamically enabled or disabled via server commands, ensuring each client runs only the necessary features.

## Dependency Injection
Extend or modify services by updating the `IocConfig` project via StructureMap.

## UI Customization
Tailor the frontend by modifying Razor views and leveraging frameworks such as Bootstrap and AngularJS.

---

# Testing and Quality Assurance

## Unit and Integration Testing
The Tests project ensures that individual components as well as integration points function correctly.

## Continuous Integration
It is recommended to implement a CI/CD pipeline (e.g., using GitHub Actions or Azure DevOps) to run tests automatically on commits.

---

# Contribution Guidelines

We welcome contributions! Please ensure:

- **Your code adheres to the project's layered architecture.**
- **Unit tests are added or updated.**
- **Documentation is updated to reflect any changes.**

For issues or feature requests, please submit an issue in the repository.

# License

This project is licensed under the MIT License.

# Additional Information

## Extensibility
The modular design allows for easy expansion, such as adding new clinical modules or reporting features.

## Error Handling & Performance Diagnostics
Tools like Elmah, Glimpse, and MiniProfiler help monitor application performance and log any unexpected errors.

## Security
Integration with Microsoft.AspNet.Identity and Owin ensures robust authentication and authorization mechanisms.

Enjoy building and enhancing the Arganmehr HIS!

> **Note:** This project was originally developed a long time ago when I wasn't yet familiar with GitHub. Although it is an older project, it remains a valuable resource that has helped many new programmers. I have recently added it to GitHub, and as a result, it does not have a commit history.




