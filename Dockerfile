# Use a .NET SDK image as the base
# This image contains the .NET SDK required to build your project
FROM mcr.microsoft.com/playwright/dotnet:v1.52.0 AS build

WORKDIR /app

# Copy the .csproj file and restore dependencies
# This is an optimization: if the csproj doesn't change, Docker can use a cached layer
COPY SaverBackendApiClient ./SaverBackendApiClient
COPY SaverBackendApiTests ./SaverBackendApiTests
#WORKDIR SaverBackendApiTests
RUN dotnet restore SaverBackendApiTests/SaverBackendApiTests.csproj

COPY . .

# Publish the test project
# --configuration Release: Builds in Release mode
# -o /app/publish: Output directory for the published application
RUN dotnet publish SaverBackendApiTests/SaverBackendApiTests.csproj -c Debug -o ../app/publish

# Use a smaller runtime image for the final stage
# This image only contains the .NET runtime, making the final image smaller
FROM mcr.microsoft.com/playwright/dotnet:v1.52.0 AS final

WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Define the entry point for running the tests
# Assuming you're using `dotnet test`. You might need to specify the test assembly.
# For example, if your test assembly is YourApiTests.dll
ENTRYPOINT ["dotnet", "test", "SaverBackendApiTests.dll"] 
# OR if your project file is in the root and dotnet test can discover it:
# ENTRYPOINT ["dotnet", "test"]

#CMD ["bash"]