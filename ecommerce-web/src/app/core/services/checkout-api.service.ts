import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CheckoutPricing } from '../models/checkout-pricing';

@Injectable({ providedIn: 'root' })
export class CheckoutApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/checkout`;

  getPricing(subtotal: number): Observable<CheckoutPricing> {
    const params = new HttpParams().set('subtotal', String(subtotal));
    return this.http.get<CheckoutPricing>(`${this.base}/pricing`, { params });
  }
}
