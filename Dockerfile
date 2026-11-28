# All-in-one image: Angular frontend built and served by the ASP.NET Core API.
# Two stages max so it builds on free container tiers.

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Node 22 (official binary, matches the builder architecture) for the Angular build
RUN arch=$(dpkg --print-architecture) \
 && case "$arch" in amd64) n=x64 ;; arm64) n=arm64 ;; *) echo "unsupported arch $arch" && exit 1 ;; esac \
 && curl -fsSL https://nodejs.org/dist/v22.14.0/node-v22.14.0-linux-$n.tar.gz | tar -xz -C /usr/local --strip-components=1 \
 && node --version && npm --version

# Frontend
WORKDIR /web
COPY frontend/package.json frontend/package-lock.json ./
RUN npm ci --no-audit --no-fund
COPY frontend/ .
RUN npm run build

# Backend
WORKDIR /src
COPY backend/*.slnx backend/
COPY backend/Domain/*.csproj backend/Domain/
COPY backend/Application/*.csproj backend/Application/
COPY backend/Infrastructure/*.csproj backend/Infrastructure/
COPY backend/API/*.csproj backend/API/
RUN dotnet restore backend/API/RecruitPro.API.csproj
COPY backend/ backend/
RUN mkdir -p backend/API/wwwroot && cp -r /web/dist/frontend/browser/. backend/API/wwwroot/
RUN dotnet publish backend/API/RecruitPro.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
# Jwt__Key must be provided as a runtime env var; the app refuses to boot without it.
ENTRYPOINT ["dotnet", "RecruitPro.API.dll"]
