# RecruitPro — Enterprise Recruitment Management System

A high-performance Enterprise Recruitment Management System built with **ASP.NET Core 10 Clean Architecture** and **Angular 20 (Standalone & Signals)**. Features candidate lifecycle management, visual recruitment pipelines, role-based security, interview scheduling, executive analytics, and resume parsing.

---

## 🌟 Tech Stack

### Backend
- **Framework**: ASP.NET Core 10 Web API
- **Architecture**: Clean Architecture (API, Application, Domain, Infrastructure)
- **Data Access**: Entity Framework Core 10 (SQLite / SQL Server)
- **Security & Auth**: JWT Bearer Tokens, ASP.NET Core Identity & Role Authorization
- **Documentation & Logging**: Serilog, Swagger / OpenAPI 3.0

### Frontend
- **Framework**: Angular 20 (Standalone Components, Signals, RxJS, Reactive Forms)
- **Styling**: Vanilla SCSS Design System, Angular Material & Bootstrap 5 Grid
- **State & Architecture**: Reactive Signal State Management, Role Guards, Dedicated Layouts

---

## 🚀 Quick Start

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+ & npm](https://nodejs.org/)

### 1. Run Backend API
```bash
cd backend/API
dotnet run
# API runs at http://localhost:5136
# Swagger UI at http://localhost:5136/swagger
```

### 2. Run Angular Frontend
```bash
cd frontend
npm install
npm start
# App runs at http://localhost:4200
```

---

## 🔑 Default System Credentials

| Role | Email | Password |
|---|---|---|
| **Admin** | `admin@recruit.com` | `Admin123!` |
| **HR Manager** | `sarah@recruit.com` | `HrManager123!` |
| **Recruiter** | `michael@recruit.com` | `Recruiter123!` |
| **Candidate** | `alex@candidate.com` | `Candidate123!` |

---

## 📁 Solution Architecture

```
.
├── backend/
│   ├── API/                    # Presentation Layer (Controllers, Middleware, Swagger)
│   ├── Application/           # Application Layer (DTOs, Services, Interfaces, Mappings)
│   ├── Domain/                # Core Domain Layer (Entities, Enums, Interfaces)
│   └── Infrastructure/        # Infrastructure Layer (EF Core DbContext, Repositories, Seeding)
├── frontend/
│   └── src/
│       ├── app/
│       │   ├── core/          # Services, Guards, Interceptors, Models
│       │   ├── features/      # Feature Modules (Public, Candidate, HR, Admin, Analytics)
│       │   ├── layouts/       # Dedicated Workspaces (Public, Candidate, HR, Admin)
│       │   └── shared/        # Reusable Design System Components
│       └── styles.scss        # Global CSS Tokens & Variables
├── .gitignore
├── LICENSE
└── README.md
```

---

## 🛡️ Role-Based Workspaces & Portals

1. **Public Website** (`/`) — Company landing page, career opportunities, job detail view, login & registration.
2. **Candidate Portal** (`/candidate/*`) — Candidate overview, application status pipeline, PDF/DOCX resume file upload (5MB max), interview invitation schedule, account settings.
3. **HR Workspace** (`/hr/*`) — Candidate directory, job posting management, 5-stage visual recruitment pipeline (`Applied` ➔ `Screening` ➔ `Interview` ➔ `Offered` ➔ `Hired`), and interview feedback recording.
4. **Admin Portal** (`/admin/*`) — User management & role assignment, 8-module CRUD permission matrix, system settings, email template editor with merge tags, audit logs, and broadcast announcements.
5. **Executive Analytics** (`/hr/analytics`) — Hiring conversion funnel, 12-month trend charts, department breakdown, offer acceptance rate, and CSV data exporter.

---

## 📜 License

This project is licensed under the [MIT License](LICENSE).
