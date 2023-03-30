#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0-buster-slim-arm64v8 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0-buster-slim-arm64v8 AS build
WORKDIR /src
COPY ["PaydayAssistant.csproj", ""]
RUN dotnet restore "PaydayAssistant.csproj" -r linux-arm64
COPY . .
WORKDIR "/src/"
RUN dotnet build "PaydayAssistant.csproj" -c Release -r linux-arm64 --no-self-contained -o /app/build

FROM build AS publish
RUN dotnet publish "PaydayAssistant.csproj" -r linux-arm64 -c Release --no-self-contained -o /app/publish

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaydayAssistant.dll"]
