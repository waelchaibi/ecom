import { HttpClient } from '@angular/common/http';
import { Injectable, signal, inject } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginResponse } from '../models/auth';

const STORAGE_KEY = 'ecom_admin_jwt';

@Injectable({ providedIn: 'root' })
export class AdminAuthService {
  private readonly http = inject(HttpClient);
  readonly token = signal<string | null>(null);

  constructor() {
    const existing = localStorage.getItem(STORAGE_KEY);
    if (existing) {
      this.token.set(existing);
    }
  }

  login(username: string, password: string): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${environment.apiBaseUrl}/auth/login`, { username, password })
      .pipe(
        tap((res) => {
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
