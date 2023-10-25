#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ODS-Data-Extractor/ODS-Data-Extractor.csproj.csproj", "ODS-Data-Extractor.csproj/"]
RUN dotnet restore "ODS-Data-Extractor/ODS-Data-Extractor.csproj.csproj"
COPY . .
WORKDIR "/src/ODS-Data-Extractor"
RUN dotnet build "ODS-Data-Extractor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ODS-Data-Extractor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ODS-Data-Extractor.csproj.dll"]