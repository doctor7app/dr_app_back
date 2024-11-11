# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
# Prometheus port for metrics
EXPOSE 9090


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Patients/Patients.Api/Patients.Api.csproj", "Patients/Patients.Api/"]
COPY ["Patients/Patients.Infrastructure/Patients.Infrastructure.csproj", "Patients/Patients.Infrastructure/"]
COPY ["Patients/Patients.Application/Patients.Application.csproj", "Patients/Patients.Application/"]
COPY ["Common/Common/Common.csproj", "Common/Common/"]
COPY ["Patients/Patients.Domain/Patients.Domain.csproj", "Patients/Patients.Domain/"]
RUN dotnet restore "./Patients/Patients.Api/Patients.Api.csproj"
COPY . .
WORKDIR "/src/Patients/Patients.Api"
RUN dotnet build "./Patients.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Patients.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Patients.Api.dll"]