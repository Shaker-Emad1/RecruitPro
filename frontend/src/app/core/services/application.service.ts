import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, Application, PaginatedResult } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class ApplicationService {
  private readonly base = `${environment.apiUrl}/applications`;

  constructor(private http: HttpClient) {}

  getAll(page = 1, pageSize = 50): Observable<PaginatedResult<Application>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http
      .get<ApiResponse<PaginatedResult<Application>>>(this.base, { params })
      .pipe(map((r) => r.data));
  }

  getById(id: string): Observable<Application> {
    return this.http
      .get<ApiResponse<Application>>(`${this.base}/${id}`)
      .pipe(map((r) => r.data));
  }

  updateStatus(id: string, status: string): Observable<Application> {
    return this.http
      .put<ApiResponse<Application>>(`${this.base}/${id}/status`, { status })
      .pipe(map((r) => r.data));
  }
}
