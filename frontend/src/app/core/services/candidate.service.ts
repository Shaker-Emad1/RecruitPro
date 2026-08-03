import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, Candidate, PaginatedResult } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class CandidateService {
  private readonly base = `${environment.apiUrl}/candidates`;

  constructor(private http: HttpClient) {}

  getAll(page = 1, pageSize = 50, search = ''): Observable<PaginatedResult<Candidate>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (search) params = params.set('search', search);
    return this.http
      .get<ApiResponse<PaginatedResult<Candidate>>>(this.base, { params })
      .pipe(map((r) => r.data));
  }

  getById(id: string): Observable<Candidate> {
    return this.http
      .get<ApiResponse<Candidate>>(`${this.base}/${id}`)
      .pipe(map((r) => r.data));
  }

  analyze(candidateId: string, jobId?: string): Observable<unknown> {
    let params = new HttpParams();
    if (jobId) params = params.set('jobId', jobId);
    return this.http
      .post<ApiResponse<unknown>>(`${this.base}/${candidateId}/analyze`, {}, { params })
      .pipe(map((r) => r.data));
  }
}
