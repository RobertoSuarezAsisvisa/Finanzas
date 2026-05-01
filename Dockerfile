FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restore first to leverage layer caching.
COPY FinanzasMCP.sln ./
COPY src/FinanzasMCP.Application/FinanzasMCP.Application.csproj src/FinanzasMCP.Application/
COPY src/FinanzasMCP.Domain/FinanzasMCP.Domain.csproj src/FinanzasMCP.Domain/
COPY src/FinanzasMCP.Infrastructure/FinanzasMCP.Infrastructure.csproj src/FinanzasMCP.Infrastructure/
COPY src/FinanzasMCP.McpServer/FinanzasMCP.McpServer.csproj src/FinanzasMCP.McpServer/

RUN dotnet restore FinanzasMCP.sln

COPY . .
RUN dotnet publish src/FinanzasMCP.McpServer/FinanzasMCP.McpServer.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FinanzasMCP.McpServer.dll"]
