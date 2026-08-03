// ─── Shared API wrapper ───────────────────────────────────────────────────────
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

// ─── Domain Models ────────────────────────────────────────────────────────────
export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
}

export interface AuthPayload {
  token: string;
  user: User;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface Skill {
  name: string;
  category: string;
  proficiencyLevel: string;
}

export interface Candidate {
  id: string;
  fullName: string;
  email: string;
  phone: string;
  location: string;
  status: string;
  classification: string;
  aiScore: number;
  yearsOfExperience: number;
  education: string;
  certifications: string;
  linkedInUrl: string;
  gitHubUrl: string;
  portfolioUrl?: string;
  aiSummary: string;
  aiRecommendations: string;
  aiStrengths: string;
  aiWeaknesses: string;
  aiInterviewQuestions: string;
  aiHiringRecommendation: string;
  skills: Skill[];
  createdAt: string;
}

export interface Job {
  id: string;
  title: string;
  department: string;
  location: string;
  status: string;
  description: string;
  requirements: string;
  requiredSkills: string;
  minimumExperience: number;
  salaryMin: number;
  salaryMax: number;
  applicationCount: number;
  createdAt: string;
}

export interface Application {
  id: string;
  candidateId: string;
  candidateName: string;
  candidateEmail: string;
  jobId: string;
  jobTitle: string;
  status: string;
  coverLetter: string;
  notes: string;
  matchScore: number;
  createdAt: string;
}

export interface Interview {
  id: string;
  type: string;
  status: string;
  startTime: string;
  endTime: string;
  meetingLink: string;
  location: string;
  notes: string;
  candidateId: string;
  candidateName: string;
  jobId: string;
  jobTitle: string;
  interviewerId: string;
  interviewerName: string;
}

export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface MonthlyTrend {
  month: string;
  candidates: number;
  interviews: number;
  hires: number;
}

export interface DashboardAnalytics {
  totalCandidates: number;
  qualifiedCandidates: number;
  rejectedCandidates: number;
  interviewsScheduled: number;
  openPositions: number;
  hiringConversionRate: number;
  averageCandidateScore: number;
  monthlyTrends: MonthlyTrend[];
}
