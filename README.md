# AutoShop API
*a test assignment for one of the interviews meant to showcase applicable best practices in modern frontend and backend development
A robust and scalable RESTful API for the Auto Shop Management System, built with .NET 8 and PostgreSQL. This backend provides all the necessary endpoints for managing users and cars, with a focus on performance and security.

## Features

- **Full CRUD Operations**: For both Users and Cars.
- **Entity Framework Core**: For ORM and database management.
- **PostgreSQL Integration**: A powerful open-source object-relational database system.
- **RESTful Architecture**: Clean, predictable, and resource-oriented API design.
- **Dependency Injection**: Following best practices for maintainable and testable code.
- **Database Migrations**: Easy database schema management with EF Core migrations.

## Technology Stack

### Backend

- **.NET 8**: The latest version of the .NET framework for building high-performance applications.
- **ASP.NET Core**: For building web APIs.
- **Entity Framework Core**: Modern object-database mapper for .NET.
- **PostgreSQL**: As the relational database.
- **Npgsql**: .NET data provider for PostgreSQL.

### Development & Tools

- **Visual Studio / Rider / VS Code**: Recommended IDEs.
- **.NET CLI**: Command-line interface for .NET.
- **Docker**: For containerization.

## Installation & Setup

### Prerequisites

- .NET 8 SDK
- PostgreSQL Server
- Git

- Supply appsettings.json or set enviromental variables for the following:
    - `ConnectionStrings_DefaultConnection`
    - `corsConfig` // comma separated allowed hosts for cors

## API Endpoints

The API exposes a set of RESTful endpoints for interacting with the application's resources. You can explore the available endpoints using the Swagger UI, which is available at `/swagger` when the application is running in a development environment.
