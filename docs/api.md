# API Reference

Base URL: `http://localhost:5136/api`

All endpoints return JSON responses. Authentication is via JWT Bearer token obtained from the login endpoint.

---

## Authentication

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/login` | Authenticate user and return JWT token | No |

**POST /api/auth/login**

Request body:
```json
{
  "email": "admin@recruit.com",
  "password": "Admin123!"
}
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "refreshToken": "dGhpcyBpcyBhIHJlZnJl...",
  "expiresAt": "2026-06-15T01:00:00Z",
  "user": {
    "id": "uuid",
    "email": "admin@recruit.com",
    "firstName": "Admin",
    "lastName": "User",
    "role": "Admin"
  }
}
```

---

## Candidates

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/candidates` | List all candidates | Yes |
| POST | `/api/candidates` | Create a new candidate | Yes |
| GET | `/api/candidates/{id}` | Get candidate details | Yes |
| PUT | `/api/candidates/{id}` | Update candidate | Yes |
| DELETE | `/api/candidates/{id}` | Delete candidate | Admin |

**GET /api/candidates**

Query parameters:
| Parameter | Type | Description |
|-----------|------|-------------|
| search | string | Search by name, email, or skills |
| status | string | Filter by status (New, Reviewed, Contacted, Hired, Rejected) |
| minScore | number | Minimum AiScore filter (0-100) |
| maxScore | number | Maximum AiScore filter (0-100) |
| page | integer | Page number (default: 1) |
| pageSize | integer | Items per page (default: 20) |

Response:
```json
{
  "items": [
    {
      "id": "uuid",
      "name": "Jane Smith",
      "email": "jane@example.com",
      "phone": "+1234567890",
      "position": "Software Engineer",
      "experience": 5,
      "skills": ["C#", "React", "SQL"],
      "aiScore": 85,
      "status": "Reviewed",
      "createdAt": "2026-06-13T10:00:00Z"
    }
  ],
  "totalCount": 42,
  "page": 1,
  "pageSize": 20,
  "totalPages": 3
}
```

**POST /api/candidates**

Request body:
```json
{
  "name": "Jane Smith",
  "email": "jane@example.com",
  "phone": "+1234567890",
  "position": "Software Engineer",
  "experience": 5,
  "skills": ["C#", "React", "SQL"],
  "aiScore": 85,
  "status": "New"
}
```

**GET /api/candidates/{id}**

Response includes all candidate fields plus:
```json
{
  "id": "uuid",
  "name": "Jane Smith",
  "email": "jane@example.com",
  "phone": "+1234567890",
  "position": "Software Engineer",
  "experience": 5,
  "skills": ["C#", "React", "SQL"],
  "aiScore": 85,
  "status": "Reviewed",
  "applications": [
    {
      "id": "uuid",
      "jobId": "uuid",
      "jobTitle": "Senior Developer",
      "status": "Screening"
    }
  ],
  "createdAt": "2026-06-13T10:00:00Z",
  "updatedAt": "2026-06-13T14:00:00Z"
}
```

---

## Jobs

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/jobs` | List all jobs | Yes |
| POST | `/api/jobs` | Create a new job | HR Manager, Admin |
| GET | `/api/jobs/{id}` | Get job details | Yes |
| PUT | `/api/jobs/{id}` | Update job | HR Manager, Admin |
| DELETE | `/api/jobs/{id}` | Delete job | Admin |

**GET /api/jobs**

Query parameters: `search`, `status` (Active, Closed, Draft), `department`, `page`, `pageSize`

Response:
```json
{
  "items": [
    {
      "id": "uuid",
      "title": "Senior Software Engineer",
      "department": "Engineering",
      "location": "Remote",
      "type": "Full-Time",
      "status": "Active",
      "salaryRange": "$120k - $160k",
      "description": "We are looking for...",
      "requirements": ["5+ years C#", "ASP.NET Core", "React"],
      "applicationCount": 12,
      "createdAt": "2026-06-10T08:00:00Z"
    }
  ],
  "totalCount": 15,
  "page": 1,
  "pageSize": 20,
  "totalPages": 1
}
```

**POST /api/jobs**

Request body:
```json
{
  "title": "Senior Software Engineer",
  "department": "Engineering",
  "location": "Remote",
  "type": "Full-Time",
  "status": "Active",
  "salaryRange": "$120k - $160k",
  "description": "Detailed job description",
  "requirements": ["5+ years C#", "ASP.NET Core", "React"],
  "benefits": ["Health insurance", "Remote work", "Equity"]
}
```

---

## Applications

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/applications` | List all applications | Yes |
| POST | `/api/applications` | Create a new application | Yes |
| GET | `/api/applications/{id}` | Get application details | Yes |
| PUT | `/api/applications/{id}` | Update application status | HR Manager, Admin |

**GET /api/applications**

Query parameters: `candidateId`, `jobId`, `status` (New, Screening, Interview, Offered, Hired, Rejected), `page`, `pageSize`

Response:
```json
{
  "items": [
    {
      "id": "uuid",
      "candidateId": "uuid",
      "candidateName": "Jane Smith",
      "jobId": "uuid",
      "jobTitle": "Senior Software Engineer",
      "status": "Screening",
      "aiScore": 85,
      "notes": "Strong technical background",
      "appliedAt": "2026-06-12T09:00:00Z"
    }
  ],
  "totalCount": 28,
  "page": 1,
  "pageSize": 20,
  "totalPages": 2
}
```

**POST /api/applications**

Request body:
```json
{
  "candidateId": "uuid",
  "jobId": "uuid",
  "status": "New",
  "notes": "Initial application"
}
```

---

## Interviews

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/interviews` | List all interviews | Yes |
| POST | `/api/interviews` | Schedule a new interview | HR Manager, Admin |
| GET | `/api/interviews/{id}` | Get interview details | Yes |
| PUT | `/api/interviews/{id}` | Update interview | HR Manager, Admin |
| DELETE | `/api/interviews/{id}` | Cancel interview | Admin |

**GET /api/interviews**

Query parameters: `candidateId`, `jobId`, `status` (Scheduled, Completed, Cancelled), `fromDate`, `toDate`, `page`, `pageSize`

Response:
```json
{
  "items": [
    {
      "id": "uuid",
      "candidateId": "uuid",
      "candidateName": "Jane Smith",
      "jobId": "uuid",
      "jobTitle": "Senior Software Engineer",
      "scheduledDate": "2026-06-15T14:00:00Z",
      "status": "Scheduled",
      "interviewType": "Technical",
      "interviewerName": "Sarah Johnson",
      "notes": "Focus on system design",
      "createdAt": "2026-06-13T10:00:00Z"
    }
  ],
  "totalCount": 8,
  "page": 1,
  "pageSize": 20,
  "totalPages": 1
}
```

**POST /api/interviews**

Request body:
```json
{
  "candidateId": "uuid",
  "jobId": "uuid",
  "applicationId": "uuid",
  "scheduledDate": "2026-06-15T14:00:00Z",
  "status": "Scheduled",
  "interviewType": "Technical",
  "interviewerName": "Sarah Johnson",
  "notes": "Technical interview - system design focus"
}
```

---

## Analytics

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/analytics/dashboard` | Dashboard summary metrics | Yes |
| GET | `/api/analytics/candidates` | Candidate analytics breakdown | Admin |
| GET | `/api/analytics/jobs` | Job performance analytics | Admin |
| GET | `/api/analytics/timeline` | Activity timeline data | Yes |

**GET /api/analytics/dashboard**

Response:
```json
{
  "newCandidates": 24,
  "activeJobs": 15,
  "pendingApplications": 42,
  "todayInterviews": 6,
  "totalCandidates": 156,
  "totalJobs": 28,
  "hiredThisMonth": 8,
  "averageAiScore": 72.5,
  "statusBreakdown": {
    "new": 24,
    "screening": 18,
    "interview": 12,
    "offered": 5,
    "hired": 3,
    "rejected": 8
  },
  "interviewsThisWeek": 14,
  "applicationsByJob": [
    { "jobTitle": "Senior Engineer", "count": 12 },
    { "jobTitle": "Product Manager", "count": 8 }
  ],
  "lastUpdated": "2026-06-14T06:00:00Z"
}
```

---

## Email

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/email/send` | Send an email notification | Yes |
| GET | `/api/email/templates` | List email templates | Admin |
| POST | `/api/email/templates` | Create email template | Admin |

**POST /api/email/send**

Request body:
```json
{
  "to": "candidate@example.com",
  "subject": "Application Status Update",
  "body": "Your application is being reviewed...",
  "type": "candidate_notification"
}
```

Response:
```json
{
  "success": true,
  "messageId": "uuid",
  "sentAt": "2026-06-14T06:30:00Z"
}
```

---

## Webhooks

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/webhook/candidate` | External candidate data ingestion | No (API key) |

**POST /api/webhook/candidate**

Accepts candidate data from external sources. Expects API key in header `X-API-Key`.

Request body:
```json
{
  "name": "Jane Smith",
  "email": "jane@example.com",
  "phone": "+1234567890",
  "source": "LinkedIn",
  "profileUrl": "https://linkedin.com/in/janesmith",
  "skills": ["C#", "React"],
  "experience": 5
}
```

Response:
```json
{
  "success": true,
  "candidateId": "uuid",
  "message": "Candidate created successfully"
}
```

---

## Error Responses

All endpoints return standardized error responses:

```json
{
  "success": false,
  "statusCode": 400,
  "message": "Validation failed",
  "errors": {
    "email": ["Email is required"],
    "name": ["Name must be at least 2 characters"]
  }
}
```

| Status Code | Description |
|-------------|-------------|
| 200 | Success |
| 201 | Created |
| 400 | Bad Request / Validation Error |
| 401 | Unauthorized (missing or invalid token) |
| 403 | Forbidden (insufficient permissions) |
| 404 | Resource not found |
| 409 | Conflict (duplicate resource) |
| 500 | Internal server error |
