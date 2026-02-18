FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files first (layer caching for restore)
COPY FeedApp.slnx .
COPY Domain/FeedApp.Domain.csproj Domain/
COPY Application/FeedApp.Application.csproj Application/
COPY Infrastructure/FeedApp.Infrastructure.csproj Infrastructure/
COPY Api/FeedApp.Api.csproj Api/

RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet build Api/FeedApp.Api.csproj -c Release --no-restore

FROM build AS publish
RUN dotnet publish Api/FeedApp.Api.csproj -c Release -o /app/publish --no-build

# Runtime image
FROM base AS final
WORKDIR /app

# Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser

COPY --from=publish /app/publish .

# Create logs directory with correct permissions
RUN mkdir -p /app/logs && chown -R appuser:appuser /app/logs

USER appuser

ENTRYPOINT ["dotnet", "FeedApp.Api.dll"]
