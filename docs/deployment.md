# Deployment Guide

## Overview

This guide covers deploying the AI Recruitment Automation Platform in various environments. The platform consists of four main services:

1. **Backend API** - ASP.NET Core 10 Web API
2. **Frontend** - Next.js 16 application
3. **n8n** - Workflow automation engine
4. **Database** - SQLite (development) or SQL Server (production)

---

## Docker Deployment (Recommended)

### Prerequisites

- Docker Engine 24+
- Docker Compose v2+

### docker-compose.yml

```yaml
version: "3.8"

services:
  api:
    build:
      context: ./backend
      dockerfile: API/Dockerfile
    ports:
      - "5136:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=db;Database=RecruitmentDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true
      - Jwt__Key=YourSuperSecretJwtKeyAtLeast32CharactersLong
      - Jwt__Issuer=RecruitmentAPI
      - Jwt__Audience=RecruitmentApp
      - OpenAi__ApiKey=${OPENAI_API_KEY}
      - Smtp__Host=${SMTP_HOST}
      - Smtp__Port=${SMTP_PORT}
      - Smtp__Username=${SMTP_USER}
      - Smtp__Password=${SMTP_PASS}
    depends_on:
      - db
    restart: unless-stopped
    networks:
      - recruitment-network

  frontend:
    build:
      context: ./frontend
      dockerfile: Dockerfile
    ports:
      - "3000:3000"
    environment:
      - NEXT_PUBLIC_API_URL=http://localhost:5136/api
      - NODE_ENV=production
    depends_on:
      - api
    restart: unless-stopped
    networks:
      - recruitment-network

  n8n:
    image: n8nio/n8n:latest
    ports:
      - "5678:5678"
    environment:
      - N8N_HOST=localhost
      - N8N_PORT=5678
      - N8N_PROTOCOL=http
      - WEBHOOK_URL=http://localhost:5678/
      - N8N_ENCRYPTION_KEY=${N8N_ENCRYPTION_KEY}
    volumes:
      - n8n-data:/home/node/.n8n
    restart: unless-stopped
    networks:
      - recruitment-network

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    ports:
      - "1433:1433"
    environment:
      - ACCEPT_EULA=Y
      - MSSQL_SA_PASSWORD=YourStrong@Passw0rd
    volumes:
      - sqlserver-data:/var/opt/mssql
    restart: unless-stopped
    networks:
      - recruitment-network

volumes:
  n8n-data:
  sqlserver-data:

networks:
  recruitment-network:
    driver: bridge
```

### Deploy with Docker

```bash
# Clone the repository
git clone <repository-url>
cd recruitment-automation

# Set environment variables
export OPENAI_API_KEY=sk-your-openai-key
export SMTP_HOST=smtp.gmail.com
export SMTP_PORT=587
export SMTP_USER=your-email@gmail.com
export SMTP_PASS=your-app-password
export N8N_ENCRYPTION_KEY=your-n8n-encryption-key

# Build and start all services
docker compose up --build -d

# View logs
docker compose logs -f

# Stop services
docker compose down

# Stop and remove volumes (clean slate)
docker compose down -v
```

### Verify Deployment

- API: http://localhost:5136/swagger
- Frontend: http://localhost:3000
- n8n: http://localhost:5678
- Health check: http://localhost:5136/health

---

## Manual Deployment

### Backend (Windows)

```powershell
# Prerequisites: .NET SDK 10.0

# Clone and navigate
git clone <repository-url>
cd recruitment-automation/backend

# Restore dependencies
dotnet restore API/API.csproj

# Apply migrations and update database
dotnet tool install --global dotnet-ef
dotnet ef database update --project Infrastructure --startup-project API

# Run the API
cd API
dotnet run --environment Production

# Or publish and run as a service
dotnet publish -c Release -o ./publish
cd publish
dotnet API.dll --urls "http://0.0.0.0:5136"
```

### Backend (Linux/macOS)

```bash
# Prerequisites: .NET SDK 10.0

# Clone and navigate
git clone <repository-url>
cd recruitment-automation/backend

# Restore dependencies
dotnet restore API/API.csproj

# Apply migrations
dotnet tool install --global dotnet-ef
dotnet ef database update --project Infrastructure --startup-project API

# Run the API
cd API
ASPNETCORE_ENVIRONMENT=Production dotnet run

# Or run as a systemd service
sudo tee /etc/systemd/system/recruitment-api.service << EOF
[Unit]
Description=Recruitment API
After=network.target

[Service]
WorkingDirectory=/opt/recruitment/API
ExecStart=/usr/bin/dotnet /opt/recruitment/API/publish/API.dll --urls "http://0.0.0.0:5136"
Restart=always
RestartSec=10
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
EOF

sudo systemctl enable recruitment-api
sudo systemctl start recruitment-api
```

### Frontend

```bash
# Prerequisites: Node.js 20+

# Navigate to frontend directory
cd recruitment-automation/frontend

# Install dependencies
npm install

# Build for production
npm run build

# Start the production server
npm start

# Or run with PM2 process manager
npm install -g pm2
pm2 start npm --name "recruitment-frontend" -- start
pm2 save
pm2 startup
```

### n8n Setup

```bash
# Option 1: Install globally
npm install -g n8n
n8n start

# Option 2: Using Docker (recommended)
docker run -d \
  --name n8n \
  -p 5678:5678 \
  -v n8n-data:/home/node/.n8n \
  -e N8N_ENCRYPTION_KEY=your-key \
  n8nio/n8n:latest

# Option 3: Using Docker Compose (see docker-compose.yml above)

# Import workflows
# 1. Open n8n UI at http://localhost:5678
# 2. Go to Workflows > Import from File
# 3. Select the JSON files from n8n-workflows/ directory
# 4. Configure any missing credentials
# 5. Activate each workflow
```

---

## Environment Variables

### Backend Variables

| Variable | Required | Description | Example |
|----------|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | No | Runtime environment | `Development`, `Production`, `Staging` |
| `ConnectionStrings__DefaultConnection` | Yes | Database connection string | See database setup below |
| `Jwt__Key` | Yes | JWT signing key (min 32 chars) | `YourSuperSecretJwtKeyAtLeast32CharactersLong` |
| `Jwt__Issuer` | No | JWT token issuer | `RecruitmentAPI` |
| `Jwt__Audience` | No | JWT token audience | `RecruitmentApp` |
| `Jwt__ExpiryMinutes` | No | Token expiration in minutes | `60` |
| `OpenAi__ApiKey` | Yes | OpenAI API key | `sk-proj-...` |
| `Smtp__Host` | For email | SMTP server hostname | `smtp.gmail.com` |
| `Smtp__Port` | For email | SMTP port | `587` |
| `Smtp__Username` | For email | SMTP username | `user@gmail.com` |
| `Smtp__Password` | For email | SMTP password or app password | `your-app-password` |
| `Smtp__FromEmail` | No | From address | `noreply@recruit.com` |
| `Smtp__UseSsl` | No | Enable SSL | `true` |
| `Cors__AllowedOrigins` | No | CORS origins (comma-separated) | `http://localhost:3000` |

### Frontend Variables

| Variable | Required | Description | Example |
|----------|----------|-------------|---------|
| `NEXT_PUBLIC_API_URL` | Yes | Backend API base URL | `http://localhost:5136/api` |
| `NEXT_PUBLIC_APP_NAME` | No | Application display name | `Recruitment Platform` |
| `NEXT_PUBLIC_N8N_URL` | No | n8n instance URL | `http://localhost:5678` |

### n8n Variables

| Variable | Required | Description | Example |
|----------|----------|-------------|---------|
| `N8N_ENCRYPTION_KEY` | Yes | Encryption key for credentials | 32+ character random string |
| `N8N_HOST` | No | n8n hostname | `localhost` |
| `N8N_PORT` | No | n8n port | `5678` |
| `N8N_PROTOCOL` | No | Protocol | `http` |
| `WEBHOOK_URL` | No | Public webhook URL | `http://your-domain.com/` |
| `N8N_METRICS` | No | Enable metrics | `true` |

---

## Database Setup

### Development (SQLite)

SQLite is used by default in development. No additional setup is needed.

Connection string (auto-configured):
```
Data Source=Recruitment.db
```

### Production (SQL Server)

#### Docker (automated)

SQL Server runs in the `db` service defined in `docker-compose.yml`. Migrations run automatically on startup.

#### Manual SQL Server Setup

```bash
# Create the database
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Passw0rd'
CREATE DATABASE RecruitmentDb;
GO
```

Connection string:
```
Server=localhost,1433;Database=RecruitmentDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true
```

#### Run Migrations

```bash
dotnet ef database update \
  --project backend/Infrastructure \
  --startup-project backend/API \
  --connection "Server=localhost,1433;Database=RecruitmentDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true"
```

---

## n8n Workflow Configuration

### Importing Workflows

1. Open n8n at `http://localhost:5678`
2. Click **Workflows** in the left sidebar
3. Click the **Import** button (or drag-and-drop the JSON files)
4. Import all three workflow files from `n8n-workflows/`

### Configuring Credentials

For HTTP Request nodes that require authentication:

1. Open each workflow
2. Click on any HTTP Request node
3. In the **Credentials** dropdown, select **Add New Credential**
4. Choose **Header Auth** or **Generic Credential** as needed
5. Enter the API credentials (or use `None` for unauthenticated endpoints)

### Activating Workflows

1. Open each workflow
2. Toggle the **Active** switch at the top-right
3. Verify the workflow status shows "Active" with a green indicator

### Testing Workflows

1. **Candidate Screening**: Send a POST request to the webhook URL displayed on the Webhook node
2. **Interview Scheduler**: Click **Execute Workflow** to run a manual test
3. **Daily Digest**: Click **Execute Workflow** to send a test digest immediately

### Webhook URL

After importing and activating the Candidate Screening workflow, the webhook URL will be:
```
http://localhost:5678/webhook/candidate
```

---

## Security Considerations

1. **JWT Key**: Use a secure, randomly generated key (minimum 32 characters). Rotate periodically.
2. **OpenAI API Key**: Store in a secrets manager. Never commit to version control.
3. **Database**: Use strong passwords. Restrict network access to the database server.
4. **n8n Encryption Key**: Required for credential encryption. Back up this key.
5. **HTTPS**: Configure TLS/SSL in production. Use a reverse proxy like Nginx or Caddy.
6. **Environment Files**: Use `.env` files locally but never commit them. Use a secrets manager in production.
7. **API Rate Limiting**: Configure rate limiting on the API to prevent abuse.
8. **CORS**: Restrict CORS origins to known frontend domains.

---

## Production Checklist

- [ ] Change all default credentials and JWT keys
- [ ] Configure HTTPS with valid SSL certificate
- [ ] Set `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Switch from SQLite to SQL Server
- [ ] Configure SMTP for email delivery
- [ ] Set up logging and monitoring (Serilog, Application Insights)
- [ ] Configure database backups
- [ ] Set up CI/CD pipeline
- [ ] Configure n8n encryption key and back it up
- [ ] Import and test all n8n workflows
- [ ] Run security audit
- [ ] Set up health check endpoints and monitoring alerts
- [ ] Configure auto-scaling policies (if cloud-hosted)
- [ ] Document incident response procedures
