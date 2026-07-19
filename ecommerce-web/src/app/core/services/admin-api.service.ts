import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Customer } from '../models/customer';
import { CreateProductPayload, Product } from '../models/product';
import { Order } from '../models/order';

export interface SalesSummary {
  totalRevenue: number;
  orderCount: number;
  averageOrderValue: number;
}

export interface BestSellingProduct {
  productId: number;
  productName: string;
  quantitySold: number;
}

export interface LowStockProduct {
  productId: number;
  productName: string;
  stockQuantity: number;
}

export interface LoyalCustomer {
  customerId: number;
  customerName: string;
  orderCount: number;
}

export interface HighValueCustomer {
  customerId: number;
  customerName: string;
  totalPurchaseValue: number;
}

export interface GiftAssignmentStat {
  giftId: number;
  giftName: string;
  timesAssigned: number;
}

export interface AnalyticsDashboard {
  sales: SalesSummary;
  bestSellingProducts: BestSellingProduct[];
  lowStockProducts: LowStockProduct[];
  topCustomersByOrders: LoyalCustomer[];
  topCustomersByRevenue: HighValueCustomer[];
  mostAssignedGifts: GiftAssignmentStat[];
}

export interface AdminOrderListItem {
  id: number;
  customerId: number;
  customerName: string;
  totalAmount: number;
  createdAt: string;
  status: string;
  lineItemCount: number;
}

export interface PagedOrders {
  page: number;
  pageSize: number;
  totalCount: number;
  items: AdminOrderListItem[];
}

export interface AuditLogRow {
  id: number;
  adminUsername: string;
  action: string;
  entityType: string;
  entityId?: number | null;
  details?: string | null;
  createdAt: string;
}

export interface PagedAuditLogs {
  page: number;
  pageSize: number;
  totalCount: number;
  items: AuditLogRow[];
}

export interface GiftRuleRow {
  id: number;
  ruleType: number;
  conditionValue: string;
  giftId: number;
  isActive: boolean;
  priority: number;
  gift?: { id: number; name: string; description: string; stockQuantity: number };
}

@Injectable({ providedIn: 'root' })
export class AdminApiService {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiBaseUrl;

  getDashboard(lowStockThreshold = 10): Observable<AnalyticsDashboard> {
    const params = new HttpParams().set('lowStockThreshold', String(lowStockThreshold));
    return this.http.get<AnalyticsDashboard>(`${this.base}/analytics/dashboard`, { params });
  }

  downloadAnalyticsCsv(lowStockThreshold = 10): Observable<Blob> {
    const params = new HttpParams().set('lowStockThreshold', String(lowStockThreshold));
    return this.http.get(`${this.base}/analytics/export/csv`, { params, responseType: 'blob' });
  }

  downloadAnalyticsExcel(lowStockThreshold = 10): Observable<Blob> {
    const params = new HttpParams().set('lowStockThreshold', String(lowStockThreshold));
    return this.http.get(`${this.base}/analytics/export/xlsx`, { params, responseType: 'blob' });
  }

  getOrders(
    page = 1,
    pageSize = 20,
    filters?: { status?: string; customerSearch?: string; customerId?: number; fromUtc?: string; toUtc?: string }
  ): Observable<PagedOrders> {
    let params = new HttpParams().set('page', String(page)).set('pageSize', String(pageSize));
    if (filters?.status) params = params.set('status', filters.status);
    if (filters?.customerSearch) params = params.set('customerSearch', filters.customerSearch);
    if (filters?.customerId) params = params.set('customerId', String(filters.customerId));
    if (filters?.fromUtc) params = params.set('fromUtc', filters.fromUtc);
    if (filters?.toUtc) params = params.set('toUtc', filters.toUtc);
    return this.http.get<PagedOrders>(`${this.base}/admin/orders`, { params });
  }

  getAuditLogs(
    page = 1,
    pageSize = 50,
    filters?: { action?: string; entityType?: string; fromUtc?: string; toUtc?: string }
  ): Observable<PagedAuditLogs> {
    let params = new HttpParams().set('page', String(page)).set('pageSize', String(pageSize));
    if (filters?.action) params = params.set('action', filters.action);
    if (filters?.entityType) params = params.set('entityType', filters.entityType);
    if (filters?.fromUtc) params = params.set('fromUtc', filters.fromUtc);
    if (filters?.toUtc) params = params.set('toUtc', filters.toUtc);
    return this.http.get<PagedAuditLogs>(`${this.base}/admin/audit-logs`, { params });
  }

  getCategories(): Observable<{ id: number; name: string; description?: string | null }[]> {
    return this.http.get<{ id: number; name: string; description?: string | null }[]>(
      `${this.base}/admin/categories`
    );
  }

  createCategory(body: { name: string; description?: string }): Observable<{ id: number; name: string }> {
    return this.http.post<{ id: number; name: string }>(`${this.base}/admin/categories`, body);
  }

  deleteCategory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/admin/categories/${id}`);
  }

  getOrder(id: number): Observable<Order> {
    return this.http.get<Order>(`${this.base}/admin/orders/${id}`);
  }

  getAdminCustomers(): Observable<Customer[]> {
    return this.http.get<Customer[]>(`${this.base}/admin/customers`);
  }

  getCustomer(customerId: number): Observable<Customer> {
    return this.http.get<Customer>(`${this.base}/admin/customers/${customerId}`);
  }

  getCustomerOrders(customerId: number): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.base}/admin/customers/${customerId}/orders`);
  }

  confirmPayment(orderId: number): Observable<Order> {
    return this.http.post<Order>(`${this.base}/admin/orders/${orderId}/confirm-payment`, {});
  }

  cancelOrder(orderId: number): Observable<Order> {
    return this.http.post<Order>(`${this.base}/admin/orders/${orderId}/cancel`, {});
  }

  updateProduct(
    id: number,
    body: {
      name: string;
      description: string;
      price: number;
      stockQuantity: number;
      categoryId?: number | null;
      imageUrl?: string | null;
    }
  ): Observable<Product> {
    return this.http.put<Product>(`${this.base}/admin/products/${id}`, body);
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/admin/products/${id}`);
  }

  updateStock(id: number, stockQuantity: number): Observable<Product> {
    return this.http.patch<Product>(`${this.base}/admin/products/${id}/stock`, { stockQuantity });
  }

  createProduct(body: CreateProductPayload): Observable<Product> {
    return this.http.post<Product>(`${this.base}/admin/products`, body);
  }

  uploadProductImage(file: File): Observable<{ imageUrl: string }> {
    const form = new FormData();
    form.append('file', file, file.name);
    return this.http.post<{ imageUrl: string }>(`${this.base}/admin/products/image`, form);
  }

  getGiftRules(): Observable<GiftRuleRow[]> {
    return this.http.get<GiftRuleRow[]>(`${this.base}/admin/gift-rules`);
  }

  createGiftRule(body: {
    ruleType: number;
    conditionValue: string;
    giftId: number;
    isActive: boolean;
    priority: number;
  }): Observable<GiftRuleRow> {
    return this.http.post<GiftRuleRow>(`${this.base}/admin/gift-rules`, body);
  }

  updateGiftRule(
    id: number,
    body: { ruleType?: number; conditionValue?: string; giftId?: number; isActive?: boolean; priority?: number }
  ): Observable<GiftRuleRow> {
    return this.http.put<GiftRuleRow>(`${this.base}/admin/gift-rules/${id}`, body);
  }

  setGiftRuleActive(id: number, isActive: boolean): Observable<GiftRuleRow> {
    return this.http.patch<GiftRuleRow>(`${this.base}/admin/gift-rules/${id}/active`, { isActive });
  }

  deleteGiftRule(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/admin/gift-rules/${id}`);
  }

  createGift(body: { name: string; description: string; stockQuantity: number }): Observable<{
    id: number;
    name: string;
    description: string;
    stockQuantity: number;
  }> {
    return this.http.post<{ id: number; name: string; description: string; stockQuantity: number }>(
      `${this.base}/admin/gifts`,
      body
    );
  }

  getGifts(): Observable<{ id: number; name: string; description: string; stockQuantity: number }[]> {
    return this.http.get<{ id: number; name: string; description: string; stockQuantity: number }[]>(`${this.base}/gifts`);
  }
}
