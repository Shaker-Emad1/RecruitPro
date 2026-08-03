import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, Interview, PaginatedResult } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class InterviewService {
  private readonly base = `${environment.apiUrl}/interviews`;

  constructor(private http: HttpClient) {}

  getAll(page = 1, pageSize = 50): Observable<PaginatedResult<Interview>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http
      .get<ApiResponse<PaginatedResult<Interview>>>(this.base, { params })
      .pipe(map((r) => r.data));
  }

  getById(id: string): Observable<Interview> {
    return this.http
      .get<ApiResponse<Interview>>(`${this.base}/${id}`)
      .pipe(map((r) => r.data));
  }

  updateStatus(id: string, status: string): Observable<Interview> {
    return this.http
      .put<ApiResponse<Interview>>(`${this.base}/${id}/status`, { status })
      .pipe(map((r) => r.data));
  }
}
