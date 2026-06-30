import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginResponse } from '../models/auth';
import { jwtHasRole } from '../utils/jwt.util';

const STORAGE_KEY = 'ecom_admin_jwt';
const ADMIN_ROLE = 'Admin';

@Injectable({ providedIn: 'root' })
export class AdminAuthService {
  private readonly http = inject(HttpClient);
  readonly token = signal<string | null>(null);
  readonly isAdminSession = computed(() => jwtHasRole(this.token(), ADMIN_ROLE));

  constructor() {
    const existing = localStorage.getItem(STORAGE_KEY);
    if (existing && jwtHasRole(existing, ADMIN_ROLE)) {
      this.token.set(existing);
    } else if (existing) {
      localStorage.removeItem(STORAGE_KEY);
    }
  }

  login(username: string, password: string): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${environment.apiBaseUrl}/auth/login`, { username, password })
      .pipe(
        tap((res) => {
          if (!jwtHasRole(res.token, ADMIN_ROLE)) {
            throw new Error('Invalid admin token');
          }
          localStorage.setItem(STORAGE_KEY, res.token);
          this.token.set(res.token);
        })
      );
  }

  logout(): void {
    localStorage.removeItem(STORAGE_KEY);
    this.token.set(null);
  }
}
