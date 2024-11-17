## ASP.NET Core 8 Web API
ASP.NET Core 8 Web API Overview This project is an ASP.NET Core 8 Web API that demonstrates the implementation of various features including Entity Framework Core with SQL Server, JWT authentication and authorization, refresh token handling, repository pattern with UnitOfWork, ASP.NET Identity for authorization, and basic CRUD operations with pagination and filtering.

Features • Entity Framework Core with SQL Server: Integration with SQL Server for database operations.

- JWT Authentication and Authorization: Secure the API using JSON Web Tokens.

- Refresh Token Handling: Implement refresh tokens to maintain user sessions.

- Repository Pattern with UnitOfWork: Structured data access with repository and unit of work patterns.

- ASP.NET Identity: User management system.

- CRUD Endpoints: Basic API endpoints for Create, Read, Update, and Delete operations.

- Pagination and Filtering: Implement pagination and filtering for data retrieval.

- Unit tests using NUnit and MOQ


### Basics

Run *update-database*, then start the application, it will automatically create one Admin:
Username: admin@gmail.com
Password: admin123

### Api Endpoints

**User controller**
- **Login** (enter credentials for admin)
    - Get generated AccessToken and *Authorize Swagger calls* with string "Bearer [accessToken]", or add it to Header Authorization Key using Postman
- **Refresh**
    - Provide AccessToken and RefreshToken generated in returned object result in Login call
    - It will create new valid AccessToken, new RefreshToken and destroy old RefreshToken
