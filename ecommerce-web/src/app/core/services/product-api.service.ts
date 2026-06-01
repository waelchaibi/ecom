import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Product } from '../models/product';

@Injectable({ providedIn: 'root' })
export class ProductApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/products`;

  getAll(categoryId?: number | null): Observable<Product[]> {
    const url =
      categoryId != null && categoryId > 0 ? `${this.base}?categoryId=${categoryId}` : this.base;
    return this.http.get<Product[]>(url);
  }

  getById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.base}/${id}`);
  }
}
