# FeedApp
This application is intended to handle user feeds in text, image and video url format.

# Tech Stack
- ASP.NET Core 10.0 - Minimal API
- Entity Framework Core 10 - Code First with SQL Server
- JWT Bearer - authentication & authorization
- Serilog - structured logging (console + rolling file)
- Swagger / Swashbuckle - API documentation
- BCrypt — password hashing

# Software needs to be installed
- Visual Studio 2026 (Used free community version)
- Microsoft SQL Server 2025 (Used free developer version)

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
10. Enter the User Id and Password in the DefaultConnection of appsettings.json

# Build
1. Open the solution in Visual Studio 2026
2. Build Solution

# Run
In Visual Studio start the IIS Express launch profile

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

# Create initial migration script in Developer PowerShell
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project Api

# Applying the script to the database:
dotnet ef database update --project Api