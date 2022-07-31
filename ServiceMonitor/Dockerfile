FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ServiceMonitor/ServiceMonitor.csproj", "ServiceMonitor/"]
RUN dotnet restore "ServiceMonitor/ServiceMonitor.csproj"
COPY . .
WORKDIR "/src/ServiceMonitor"
RUN dotnet build "ServiceMonitor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ServiceMonitor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServiceMonitor.dll"]
