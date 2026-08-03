FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY backend/*.slnx backend/
COPY backend/Domain/*.csproj backend/Domain/
COPY backend/Application/*.csproj backend/Application/
COPY backend/Infrastructure/*.csproj backend/Infrastructure/
COPY backend/API/*.csproj backend/API/

RUN dotnet restore backend/API/*.csproj

COPY backend/ .
RUN dotnet publish backend/API/*.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "SalesAutomation.API.dll"]
