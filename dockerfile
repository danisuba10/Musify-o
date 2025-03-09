ARG DOTNET_RUNTIME=mcr.microsoft.com/dotnet/aspnet:8.0
ARG DOTNET_SDK=mcr.microsoft.com/dotnet/sdk:8.0

FROM ${DOTNET_RUNTIME} AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM ${DOTNET_SDK} AS build
WORKDIR /src

ENV ASPNETCORE_ENVIRONMENT=Production

COPY ["API/API.csproj", "API/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Persistence/Persistence.csproj", "Persistence/"]
COPY ["Tests/Tests.csproj", "Tests/"]

RUN dotnet restore "API/API.csproj"

COPY . .

WORKDIR /src/API
RUN dotnet build "API.csproj" -c Release -o /app/build

# RUN dotnet ef migrations add MoveDBToDocker --context ApplicationDbContext --project /src/Persistence --startup-project /src/API
# RUN dotnet ef database update --context ApplicationDbContext --project /src/Persistence --startup-project /src/API

FROM build as publish
WORKDIR /src/API
RUN dotnet publish "API.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT [ "dotnet", "API.dll" ]
