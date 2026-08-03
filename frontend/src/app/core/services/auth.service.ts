import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, catchError, throwError, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, AuthPayload, LoginRequest, User } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY = 'recruit_token';
  private readonly _currentUser = signal<User | null>(null);

  readonly currentUser = this._currentUser.asReadonly();
  readonly isAuthenticated = computed(() => this._currentUser() !== null);
  readonly userRole = computed(() => this._currentUser()?.role ?? '');
  readonly userDisplayName = computed(() => {
    const u = this._currentUser();
    return u ? `${u.firstName} ${u.lastName}`.trim() : '';
  });

  constructor(
    private http: HttpClient,
    private router: Router,
  ) {}

  /** Call on app startup to restore session */
  initialize(): Observable<User> {
    return this.http.get<ApiResponse<User>>(`${environment.apiUrl}/auth/me`).pipe(
      map((res) => res.data),
      tap((user) => this._currentUser.set(user)),
      catchError((err) => {
        this.clearToken();
        return throwError(() => err);
      }),
    );
  }

  login(request: LoginRequest): Observable<AuthPayload> {
    return this.http
      .post<ApiResponse<AuthPayload>>(`${environment.apiUrl}/auth/login`, request)
      .pipe(
        map((res) => res.data),
        tap((data) => {
          this.setToken(data.token);
          this._currentUser.set(data.user);
        }),
      );
  }

  logout(): void {
    this.clearToken();
    this._currentUser.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  hasRole(role: string): boolean {
    return this._currentUser()?.role === role;
  }

  hasAnyRole(...roles: string[]): boolean {
    const userRole = this._currentUser()?.role;
    return roles.some((r) => r === userRole);
  }

  private setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  private clearToken(): void {
    localStorage.removeItem(this.TOKEN_KEY);
  }
}
