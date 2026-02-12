# FeedApp
This application is intended to handle user feeds in text, image and video url format.

# Create initial migration script
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project Api

# Applying the script to the database:
dotnet ef database update --project Api