FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out .

# Set the environment variable to make the app listen on 0.0.0.0
ENV ASPNETCORE_URLS="http://0.0.0.0:80"

# Expose the port your app runs on (optional)
EXPOSE 80

ENTRYPOINT ["dotnet", "docker-compose-demo-api.dll"]