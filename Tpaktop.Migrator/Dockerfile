FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Tpaktop.Migrator/Tpaktop.Migrator.fsproj", "Tpaktop.Migrator/"]
RUN dotnet restore "Tpaktop.Migrator/Tpaktop.Migrator.fsproj"
COPY . .
WORKDIR "/src/Tpaktop.Migrator"
RUN dotnet build "Tpaktop.Migrator.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tpaktop.Migrator.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tpaktop.Migrator.dll"]
