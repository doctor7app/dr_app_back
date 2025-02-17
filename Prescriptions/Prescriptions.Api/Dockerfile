# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Prescriptions/Prescriptions.Api/Prescriptions.Api.csproj", "Prescriptions/Prescriptions.Api/"]
COPY ["Prescriptions/Prescriptions.Infrastructure/Prescriptions.Infrastructure.csproj", "Prescriptions/Prescriptions.Infrastructure/"]
COPY ["Prescriptions/Prescriptions.Application/Prescriptions.Application.csproj", "Prescriptions/Prescriptions.Application/"]
COPY ["Prescriptions/Prescriptions.Domain/Prescriptions.Domain.csproj", "Prescriptions/Prescriptions.Domain/"]
COPY ["Common/Common/Common.csproj", "Common/Common/"]
RUN dotnet restore "./Prescriptions/Prescriptions.Api/Prescriptions.Api.csproj"
COPY . .
WORKDIR "/src/Prescriptions/Prescriptions.Api"
RUN dotnet build "./Prescriptions.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Prescriptions.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Prescriptions.Api.dll"]