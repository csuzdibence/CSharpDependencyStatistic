# Use the official .NET SDK image for building the solution
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the solution files and restore dependencies
COPY . . 
RUN dotnet restore

# Build the solution without specifying --output
RUN dotnet build CSharpDependencyStatistic.sln -c Release

# Publish the application from the individual project instead of the solution
RUN dotnet publish CSharpDependencyStatistic/CSharpDependencyStatistic.csproj -c Release -o /app/publish

# Use the runtime image for the final build
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENV SOLUTION_BASE_DIRECTORY=/app/solution

# Run the application
ENTRYPOINT ["dotnet", "CSharpDependencyStatistic.dll"]