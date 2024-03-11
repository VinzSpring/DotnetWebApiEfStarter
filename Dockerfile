# Use the official Microsoft .NET Core runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /dockering
COPY backend.csproj ./backend.csproj
RUN dotnet restore backend.csproj

COPY src/ ./src
COPY appsettings.json ./
COPY Migrations/ ./Migrations
RUN dotnet build backend.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish backend.csproj -c Release -o /app/publish --no-restore

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "backend.dll"]