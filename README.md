# JewelryInventory

An ASP.NET Core Web API developed to support inventory management for a small business producing handmade jewelry.

The project was developed as part of the **Advanced C# Programming** course at University West (Högskolan Väst).

## Project Description

JewelryInventory aims to digitalize the management of jewelry components, replacing an existing manual inventory process. The API provides functionality for registering components, managing incoming stock, performing manual inventory adjustments, and tracking inventory movements.

The application is designed to serve as the backend for a separate React Native mobile application.

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/jojo-git67/JewelryInventory.git
cd JewelryInventory
```

### Restore Dependencies

```bash
dotnet restore
```

### Build the Project

```bash
dotnet build
```

### Run the API

Navigate to the API project directory and execute:

```bash
dotnet run
```

The API uses SQLite for data storage. Entity Framework Core migrations are applied automatically when the application starts.

### Note
When the application runs for the first time, the SQLite database will be initialized without any inventory data.

## Running Unit Tests

The project includes unit tests written using xUnit and NSubstitute.

To execute the tests, run the following command from the solution directory:

```bash
dotnet test
```

The tests cover selected service operations and business rules, using mocked dependencies to isolate the functionality being tested.

## Design Pattern – AI-Assisted Implementation

### Chain of Responsibility 

As part of the individual design pattern assignment, the Chain of Responsibility pattern was implemented with the assistance of generative AI (Claude).

The implementation refactors the validation logic in `ComponentService.CreateComponentAsync()` into individual validation handlers.

After evaluating the implementation, the original validation approach was retained in the main branch to avoid unnecessary complexity.

The alternative implementation and its associated unit tests are available in the [chain-of-responsibilty branch](https://github.com/jojo-git67/JewelryInventory/tree/chain-of-responsibility).

## Author

Joakim Johansson

Advanced C# Programming – University West (Högskolan Väst)
