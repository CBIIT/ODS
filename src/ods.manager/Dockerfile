#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ods.manager/ods.manager.csproj", "ods.manager.csproj/"]
RUN dotnet restore "ods.manager/ods.manager.csproj"
COPY . .
WORKDIR "/src/ods.manager"
RUN dotnet build "ods.manager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ods.manager.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ods.manager.csproj.dll"]
