# Registration Form API

An ASP.NET Core Web API that performs full CRUD operations for a user registration system, using PostgreSQL as the database with stored procedures/functions (no Entity Framework).

## Technology Stack

- ASP.NET Core 8 Web API
- PostgreSQL (via Npgsql)
- Swagger for API testing

## Architecture

This project follows the Three-Tier Architecture:

1. **Presentation Layer**: Web API Controllers
2. **Business Layer**: Handles logic and validation
3. **Data Access Layer (DAL)**: Handles database interactions via stored procedures

## Features

- Full CRUD operations for user registrations
- Password hashing using SHA256
- Database access via stored procedures
- Proper error handling and response messages
- API documentation via Swagger UI

## Setup Instructions

### Prerequisites

- .NET 8 SDK
- PostgreSQL 13 or higher

### Database Setup

1. Open PostgreSQL and execute the SQL scripts from the `postgresql-setup.sql` file to:
   - Create the database
   - Create the users table
   - Create all stored procedures/functions

2. Update the connection string in `appsettings.json` with your PostgreSQL credentials.

### Project Setup

1. Clone the repository:
   ```
   git clone [repository-url]
   ```

2. Navigate to the project directory:
   ```
   cd RegistrationApi
   ```

3. Build the project:
   ```
   dotnet build
   ```

4. Run the application:
   ```
   dotnet run
   ```

5. Open a browser and navigate to:
   ```
   https://localhost:7001/swagger
   ```
   (The port may vary depending on your environment)

## API Endpoints

| Endpoint              | Method | Description            |
|-----------------------|--------|------------------------|
| `/api/users`          | GET    | Get all users          |
| `/api/users/{id}`     | GET    | Get user by ID         |
| `/api/users`          | POST   | Create a new user      |
| `/api/users/{id}`     | PUT    | Update an existing user|
| `/api/users/{id}`     | DELETE | Delete a user          |

## Security

- Passwords are hashed using SHA256 before storage
- Email uniqueness is enforced at the database level
