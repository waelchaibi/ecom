import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Cart, CartCheckoutPayload } from '../models/cart';
import { Order } from '../models/order';

@Injectable({ providedIn: 'root' })
export class CartApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/me/cart`;

  get(): Observable<Cart> {
    return this.http.get<Cart>(this.base);
  }

  upsertItem(productId: number, quantity: number): Observable<Cart> {
    return this.http.post<Cart>(`${this.base}/items`, { productId, quantity });
  }

  removeItem(productId: number): Observable<Cart> {
    return this.http.delete<Cart>(`${this.base}/items/${productId}`);
  }

  clear(): Observable<void> {
    return this.http.delete<void>(this.base);
  }

  checkout(payload: CartCheckoutPayload): Observable<Order> {
    return this.http.post<Order>(`${this.base}/checkout`, payload);
  }
}
