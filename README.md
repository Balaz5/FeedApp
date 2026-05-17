# FeedApp
This application is intended to handle user feeds in text, image and video url format.

# Tech Stack
- .NET 10 / ASP.NET Core 10.0 — Minimal API with built-in validation (`AddValidation`)
- Clean Architecture — Domain / Application / Infrastructure / Api layers, Repository + Unit of Work
- Entity Framework Core 10 — Code First with SQL Server 2025
- JWT Bearer — authentication & authorization
- BCrypt.Net-Next — password hashing
- Serilog — structured logging (console + rolling file, 30-day retention)
- Swagger / Swashbuckle — API documentation (with JWT bearer support)
- IHttpClientFactory + System.Xml.Linq — RSS feed ingestion
- Hosted BackgroundService — daily soft-delete cleanup job
- Custom exception-handling middleware with a typed domain-exception hierarchy
- Docker & Docker Compose — containerization

# Software needs to be installed
- Visual Studio 2026 (Used free community version)
- Microsoft SQL Server 2025 - Not needed when 'Debug in Docker' instructions are followed below (Used free developer version)
- Docker Desktop

# Application User
1. A login needs to be created in Microsoft SQL Server.
2. Login to the server with windows authentication or sa user.
3. Go to Security -> Logins
4. Right on Logins -> New Login
5. On General page , select SQL authentication
6. Enter Login name and password
7. Uncheck Enforce password policy
8. On Server Roles page, select dbcreator role
9. Click OK (User is created)
10. Enter the User Id and Password in the DefaultConnection of Api\appsettings.Development.json and Api\appsettings.json

# Build
1. Open the solution in Visual Studio 2026
2. Build Solution

# Debug in IIS Express
In Visual Studio start the IIS Express launch profile

# Debug in Docker
1. Start admin powershell
2. docker run -d --name sqlserver2025 -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Pass1word" -e "MSSQL_PID=Developer" -p 1435:1433 -v mssql_data:/var/opt/mssql mcr.microsoft.com/mssql/server:2025-latest
3. Open the FeedApp\FeedApp\Api\Properties\launchSettings.json
4. Add the User Id and Password of the Application User to the "ConnectionStrings__DefaultConnection"
- Example: "ConnectionStrings__DefaultConnection": "Server=host.docker.internal,1435;Database=AppDb;User Id=webapp;Password=webapp;MultipleActiveResultSets=true;TrustServerCertificate=True"
5. Add port 1435 to Api\appsettings.Development.json and Api\appsettings.json
- Example: "DefaultConnection": "Server=localhost,1435;Database=AppDb;User Id=webapp;Password=webapp;MultipleActiveResultSets=true;TrustServerCertificate=True"

# How to run the app in Docker
1. Open an admin powershell and navigate to the directory where docker-compose.yml file is
2. Run the following command: 'docker-compose up --build'
3. Open Swagger at http://localhost:5000/swagger
4. To exit: 'ctrl + c' in powershell

# The full auth flow to test in Swagger:
1. POST /api/auth/register or POST /api/auth/login → get a JWT token
2. Click "Authorize" in Swagger UI → paste the token
3. All endpoints except auth and health check require authentication
4. Trying to update/delete another user's feed → 403 Forbidden

- The seed users all have password 123456 so you can login as johndoe, janedoe, or bobsmith to test owner authorization.

# Migrations
The initial migration script is in the repo. 
Running the solution will create the db and populate it with seed data.
For reference, here are the migration commands.

- Create initial migration script in Developer PowerShell:

dotnet ef migrations add InitialCreate --project Infrastructure --startup-project Api

- Applying the script to the database:

dotnet ef database update --project Api
