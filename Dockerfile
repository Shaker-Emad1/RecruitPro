# All-in-one image: Angular frontend built and served by the ASP.NET Core API.
FROM node:22-alpine AS frontend
WORKDIR /web
COPY frontend/package.json frontend/package-lock.json ./
RUN npm ci --no-audit --no-fund
COPY frontend/ .
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY backend/*.slnx backend/
COPY backend/Domain/*.csproj backend/Domain/
COPY backend/Application/*.csproj backend/Application/
COPY backend/Infrastructure/*.csproj backend/Infrastructure/
COPY backend/API/*.csproj backend/API/
RUN dotnet restore backend/API/RecruitPro.API.csproj
COPY backend/ backend/
COPY --from=frontend /web/dist/frontend/browser/ backend/API/wwwroot/
RUN dotnet publish backend/API/RecruitPro.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
# Demo default so the container boots on a free host without extra config.
# Override with your own Jwt__Key env var for anything real.
ENV Jwt__Key=d3c8fb518e54b9706db78f99dc3e3322d636d4d6419bae97a13473dbc1a228dc
ENTRYPOINT ["dotnet", "RecruitPro.API.dll"]
