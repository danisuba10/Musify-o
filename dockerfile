FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

ENV ASPNETCORE_ENVIRONMENT=Development

COPY ["API/API.csproj", "API/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Persistence/Persistence.csproj", "Persistence/"]
COPY ["Tests/Tests.csproj", "Tests/"]

RUN dotnet restore "API/API.csproj"

COPY . .

WORKDIR /src/API
RUN dotnet build "API.csproj" -c Release -o /app/build

RUN dotnet publish "API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT [ "dotnet", "API.dll" ]