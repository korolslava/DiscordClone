FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["DiscordClone.API/DiscordClone.API.csproj", "DiscordClone.API/"]
COPY ["DiscordClone.Application/DiscordClone.Application.csproj", "DiscordClone.Application/"]
COPY ["DiscordClone.Infrastructure/DiscordClone.Infrastructure.csproj", "DiscordClone.Infrastructure/"]
COPY ["DiscordClone.Domain/DiscordClone.Domain.csproj", "DiscordClone.Domain/"]
COPY ["DiscordClone.Shared/DiscordClone.Shared.csproj", "DiscordClone.Shared/"]

RUN dotnet restore "DiscordClone.API/DiscordClone.API.csproj"

COPY . .

WORKDIR "/src/DiscordClone.API"
RUN dotnet publish "DiscordClone.API.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .
RUN mkdir -p uploads

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "DiscordClone.API.dll"]