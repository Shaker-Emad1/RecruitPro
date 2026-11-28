import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, Job, PaginatedResult } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class JobService {
  private readonly base = `${environment.apiUrl}/jobs`;

  constructor(private http: HttpClient) {}

  getAll(page = 1, pageSize = 50, search = '', status = ''): Observable<PaginatedResult<Job>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (search) {
      params = params.set('search', search);
    }
    if (status) {
      params = params.set('status', status);
    }
    return this.http
      .get<ApiResponse<PaginatedResult<Job>>>(this.base, { params })
      .pipe(map((r) => r.data));
  }

  getById(id: string): Observable<Job> {
    return this.http
      .get<ApiResponse<Job>>(`${this.base}/${id}`)
      .pipe(map((r) => r.data));
  }
}
