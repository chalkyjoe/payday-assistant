#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0-bullseye-slim-arm64 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0-bullseye-slim-arm64v8 AS build
WORKDIR /src
COPY ["PaydayAssistant.csproj", ""]
RUN dotnet restore "PaydayAssistant.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "PaydayAssistant.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaydayAssistant.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:7.0-bullseye-slim-arm64v8
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaydayAssistant.dll"]
