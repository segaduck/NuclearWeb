# Backend Dockerfile for NuclearWeb .NET 9 API
# Multi-stage build for optimized production image

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/NuclearWeb.API/NuclearWeb.API.csproj", "src/NuclearWeb.API/"]
COPY ["src/NuclearWeb.Core/NuclearWeb.Core.csproj", "src/NuclearWeb.Core/"]
COPY ["src/NuclearWeb.Infrastructure/NuclearWeb.Infrastructure.csproj", "src/NuclearWeb.Infrastructure/"]
COPY ["src/NuclearWeb.Application/NuclearWeb.Application.csproj", "src/NuclearWeb.Application/"]

# Restore dependencies
RUN dotnet restore "src/NuclearWeb.API/NuclearWeb.API.csproj"

# Copy all source files
COPY . .

# Build the API project
WORKDIR "/src/src/NuclearWeb.API"
RUN dotnet build "NuclearWeb.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "NuclearWeb.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Create non-root user for security
RUN groupadd -r nuclearweb && useradd -r -g nuclearweb nuclearweb

# Copy published files
COPY --from=publish /app/publish .

# Set ownership
RUN chown -R nuclearweb:nuclearweb /app

# Switch to non-root user
USER nuclearweb

# Expose port
EXPOSE 5000

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:5000/health || exit 1

# Start the application
ENTRYPOINT ["dotnet", "NuclearWeb.API.dll"]
