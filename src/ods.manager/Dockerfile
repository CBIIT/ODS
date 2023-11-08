#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/ods.manager/ods.manager.csproj", "src/ods.manager/"]
COPY ["src/ods.models/ods.models.csproj", "src/ods.models/"]
RUN dotnet restore "src/ods.manager/ods.manager.csproj"
COPY . .
WORKDIR "/src/src/ods.manager"
RUN dotnet build "ods.manager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ods.manager.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ods.manager.dll"]