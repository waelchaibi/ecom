import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Order } from '../models/order';
import { CustomerProfile } from '../models/customer-profile';

@Injectable({ providedIn: 'root' })
export class CustomerAccountApiService {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiBaseUrl;

  getProfile(): Observable<CustomerProfile> {
    return this.http.get<CustomerProfile>(`${this.base}/me`);
  }

  getMyOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.base}/me/orders`);
  }

  getMyOrder(id: number): Observable<Order> {
    return this.http.get<Order>(`${this.base}/me/orders/${id}`);
  }

  /** Emulated payment — no card processor; marks order Confirmed and applies gifts. */
  payOrder(id: number): Observable<Order> {
    return this.http.post<Order>(`${this.base}/me/orders/${id}/pay`, {});
  }

  cancelOrder(id: number): Observable<Order> {
    return this.http.post<Order>(`${this.base}/me/orders/${id}/cancel`, {});
  }
}
