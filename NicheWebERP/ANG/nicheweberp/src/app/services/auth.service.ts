import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { ApiURLService } from './ApiURLService.service';
import { CurrentUser, LoginRequest, LoginResponse } from '../models/auth.model';

const TOKEN_KEY = 'niche_erp_token';
const USER_KEY = 'niche_erp_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(private http: HttpClient) {}

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${ApiURLService.BASE_URL}/Auth/Login`, request)
      .pipe(
        tap((response) => {
          localStorage.setItem(TOKEN_KEY, response.token);
          localStorage.setItem(USER_KEY, JSON.stringify(response.user));
        })
      );
  }

  logout(): void {
    // Best-effort server call - pure JWT has nothing to invalidate server-side today, but the
    // endpoint exists so a future server-side blacklist doesn't need an API shape change.
    this.http.post(`${ApiURLService.BASE_URL}/Auth/Logout`, {}).subscribe({
      error: () => void 0
    });
    this.clearSession();
  }

  clearSession(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  getCurrentUser(): CurrentUser | null {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? (JSON.parse(raw) as CurrentUser) : null;
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}
