import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, DashboardAnalytics } from '../models/api.models';

export interface HiringFunnel {
  applications: number;
  screening: number;
  shortlisted: number;
  interview: number;
  offered: number;
  hired: number;
  rejected: number;
}

export interface DepartmentBreakdownItem {
  department: string;
  openJobs: number;
  applications: number;
  hired: number;
}

export interface ExtendedAnalytics {
  totalUsers: number;
  totalJobs: number;
  openJobs: number;
  closedJobs: number;
  totalApplications: number;
  totalInterviews: number;
  offeredCandidates: number;
  hiredCandidates: number;
  rejectedCandidates: number;
  offerAcceptanceRate: number;
  jobFillRate: number;
  candidateConversionRate: number;
  averageInterviewScore: number;
  funnel: HiringFunnel;
  departmentBreakdown: DepartmentBreakdownItem[];
  monthlyTrends: { month: string; candidates: number; interviews: number; hires: number }[];
}

@Injectable({ providedIn: 'root' })
export class AnalyticsService {
  private readonly base = `${environment.apiUrl}/analytics`;

  constructor(private http: HttpClient) {}

  getDashboard(): Observable<DashboardAnalytics> {
    return this.http
      .get<ApiResponse<DashboardAnalytics>>(`${this.base}/dashboard`)
      .pipe(map(r => r.data));
  }

  getExtended(): Observable<ExtendedAnalytics> {
    return this.http
      .get<ApiResponse<ExtendedAnalytics>>(`${this.base}/extended`)
      .pipe(map(r => r.data));
  }

  getFunnel(): Observable<HiringFunnel> {
    return this.http
      .get<ApiResponse<HiringFunnel>>(`${this.base}/funnel`)
      .pipe(map(r => r.data));
  }
}
