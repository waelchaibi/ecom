import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CustomerAuthResponse,
  CustomerLoginPayload,
  CustomerProfile,
  CustomerRegisterPayload
} from '../models/customer-profile';

const STORAGE_KEY = 'ecom_customer_jwt';
const PROFILE_KEY = 'ecom_customer_profile';

@Injectable({ providedIn: 'root' })
export class CustomerAuthService {
  private readonly http = inject(HttpClient);
  readonly token = signal<string | null>(null);
  readonly profile = signal<CustomerProfile | null>(null);
  readonly isLoggedIn = computed(() => !!this.token());

  constructor() {
    const existing = localStorage.getItem(STORAGE_KEY);
    if (existing) {
      this.token.set(existing);
    }
    const profileJson = localStorage.getItem(PROFILE_KEY);
    if (profileJson) {
      try {
        this.profile.set(JSON.parse(profileJson) as CustomerProfile);
      } catch {
        localStorage.removeItem(PROFILE_KEY);
      }
    }
  }

  register(payload: CustomerRegisterPayload): Observable<CustomerAuthResponse> {
    return this.http
      .post<CustomerAuthResponse>(`${environment.apiBaseUrl}/auth/customer/register`, payload)
      .pipe(tap((res) => this.persistSession(res)));
  }

  login(payload: CustomerLoginPayload): Observable<CustomerAuthResponse> {
    return this.http
      .post<CustomerAuthResponse>(`${environment.apiBaseUrl}/auth/customer/login`, payload)
      .pipe(tap((res) => this.persistSession(res)));
  }

  refreshProfile(): Observable<CustomerProfile> {
    return this.http.get<CustomerProfile>(`${environment.apiBaseUrl}/me`).pipe(
      tap((p) => {
        this.profile.set(p);
        localStorage.setItem(PROFILE_KEY, JSON.stringify(p));
      })
    );
  }

  logout(): void {
    localStorage.removeItem(STORAGE_KEY);
    localStorage.removeItem(PROFILE_KEY);
    this.token.set(null);
    this.profile.set(null);
  }

  private persistSession(res: CustomerAuthResponse): void {
    localStorage.setItem(STORAGE_KEY, res.token);
    localStorage.setItem(PROFILE_KEY, JSON.stringify(res.customer));
    this.token.set(res.token);
    this.profile.set(res.customer);
  }
}
