# Stage 1: Build API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-api
WORKDIR /src

# Copy solution and project files
COPY Confermation.sln .
COPY Domain/Domain.csproj Domain/
COPY Application/Application.csproj Application/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY Api/API.csproj Api/

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY Domain/ Domain/
COPY Application/ Application/
COPY Infrastructure/ Infrastructure/
COPY Api/ Api/

# Build and publish API
WORKDIR /src/Api
RUN dotnet publish -c Release -o /app/api

# Stage 2: Build Client
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-client
WORKDIR /src

COPY Confermation.sln .
COPY Client/Client.csproj Client/

RUN dotnet restore Client/Client.csproj

COPY Client/ Client/

WORKDIR /src/Client
RUN dotnet publish -c Release -o /app/client

# Stage 3: API Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS api
WORKDIR /app
COPY --from=build-api /app/api .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "API.dll"]

# Stage 4: Client Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS client
WORKDIR /app
COPY --from=build-client /app/client .
EXPOSE 8081
ENV ASPNETCORE_URLS=http://+:8081
ENTRYPOINT ["dotnet", "Client.dll"]
