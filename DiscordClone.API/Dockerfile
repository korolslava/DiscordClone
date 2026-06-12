FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/DiscordClone.API/DiscordClone.API.csproj",
      "src/DiscordClone.API/"]
COPY ["src/DiscordClone.Application/DiscordClone.Application.csproj",
      "src/DiscordClone.Application/"]
COPY ["src/DiscordClone.Infrastructure/DiscordClone.Infrastructure.csproj",
      "src/DiscordClone.Infrastructure/"]
COPY ["src/DiscordClone.Domain/DiscordClone.Domain.csproj",
      "src/DiscordClone.Domain/"]
COPY ["src/DiscordClone.Shared/DiscordClone.Shared.csproj",
      "src/DiscordClone.Shared/"]

RUN dotnet restore "src/DiscordClone.API/DiscordClone.API.csproj"

COPY . .
WORKDIR "/src/src/DiscordClone.API"
RUN dotnet publish "DiscordClone.API.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN addgroup --system appgroup && \
    adduser --system --ingroup appgroup appuser

COPY --from=build /app/publish .

RUN mkdir -p uploads && chown appuser:appgroup uploads

USER appuser

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "DiscordClone.API.dll"]