#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM --platform=$TARGETPLATFORM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=$TARGETPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PaydayAssistant.csproj", ""]
RUN dotnet restore "PaydayAssistant.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "PaydayAssistant.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaydayAssistant.csproj" --os linux --arch arm64 -c Release -o /app/publish /p:UseAppHost=false

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaydayAssistant.dll"]
