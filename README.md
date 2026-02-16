# FeedApp
This application is intended to handle user feeds in text, image and video url format.

# Application User
A login needs to be created in Microsoft SQL Server.
Login to the server with windows authentication or sa user.
Go to Security -> Logins
Right on Logins -> New Login
On General page , select SQL authentication
Enter Login name and password
Uncheck Enforce password policy
On Server Roles page, select dbcreator role
Click OK (User is created)
Enter the User Id and Password in the DefaultConnection of appsettings.json

# Build
Open the solution in Visual Studio 2026
Build Solution

# Run
In Visual Studio start the IIS Express launch profile

# The full auth flow to test in Swagger:
POST /api/auth/register or POST /api/auth/login → get a JWT token
Click "Authorize" in Swagger UI → paste the token
All /api/feeds endpoints work with your authenticated identity
Trying to update/delete another user's feed → 403 Forbidden

The seed users all have password 123456 so you can login as johndoe, janedoe, or bobsmith to test owner authorization.

# Migrations
The initial migration script is in the repo. 
Running the solution will create the db and populate it with seed data.
For reference, here are the migration commands.

# Create initial migration script in Developer PowerShell
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project Api

# Applying the script to the database:
dotnet ef database update --project Api