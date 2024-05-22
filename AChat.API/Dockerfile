FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AChat.API/AChat.API.csproj", "AChat.API/"]
COPY ["AChat.Application/AChat.Application.csproj", "AChat.Application/"]
COPY ["AChat.Domain/AChat.Domain.csproj", "AChat.Domain/"]
COPY ["AChat.Infrastructure/AChat.Infrastructure.csproj", "AChat.Infrastructure/"]
RUN dotnet restore "AChat.API/AChat.API.csproj"
COPY . .
WORKDIR "/src/AChat.API"
RUN dotnet build "AChat.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AChat.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /src/AChat.API/https/aspnetapp.pfx /https/
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AChat.API.dll"]
