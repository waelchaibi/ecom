import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { CustomerApiService } from '../../../core/services/customer-api.service';
import { ProductApiService } from '../../../core/services/product-api.service';
import { OrderApiService } from '../../../core/services/order-api.service';
import { Customer } from '../../../core/models/customer';
import { Product } from '../../../core/models/product';
import { Order } from '../../../core/models/order';

interface LineDraft {
  productId: number | null;
  quantity: number | null;
}

@Component({
  selector: 'ecom-order-create',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './order-create.component.html',
  styleUrl: './order-create.component.scss'
})
export class OrderCreateComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly productsApi = inject(ProductApiService);
  private readonly customersApi = inject(CustomerApiService);
  private readonly ordersApi = inject(OrderApiService);

  products: Product[] = [];
  customers: Customer[] = [];
  customerId = 1;
  promotionCode = '';
  lines: LineDraft[] = [{ productId: null, quantity: 1 }];
  submitting = false;
  message: string | null = null;
  error: string | null = null;
  lastOrder: Order | null = null;

  ngOnInit(): void {
    forkJoin({
      products: this.productsApi.getAll(),
      customers: this.customersApi.getAll()
    }).subscribe({
      next: ({ products, customers }) => {
        this.products = products;
        this.customers = customers;

        if (customers.length === 0) {
          this.error = 'No customers available.';
          return;
        }

        const q = this.route.snapshot.queryParamMap;
        const cid = Number(q.get('customerId'));
        if (!Number.isNaN(cid) && cid > 0 && customers.some((c) => c.id === cid)) {
          this.customerId = cid;
        } else if (customers.length > 0) {
          this.customerId = customers[0].id;
        }

        const pid = Number(q.get('productId'));
        if (!Number.isNaN(pid) && pid > 0) {
          this.lines = [{ productId: pid, quantity: 1 }];
        }
      },
      error: (e) => {
        this.error = e.error?.error ?? e.message ?? 'Could not load products or customers';
      }
    });
  }

  addLine(): void {
    this.lines.push({ productId: null, quantity: 1 });
    this.syncUrl();
  }

  removeLine(index: number): void {
    this.lines.splice(index, 1);
    if (this.lines.length === 0) {
      this.lines.push({ productId: null, quantity: 1 });
    }
    this.syncUrl();
  }

  onCustomerChange(): void {
    this.syncUrl();
  }

  onLineChange(): void {
    this.syncUrl();
  }

  private syncUrl(): void {
    const q: Record<string, string | number> = { customerId: this.customerId };
    const first = this.lines[0];
    if (first?.productId != null && first.productId > 0) {
      q['productId'] = first.productId;
    }
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: q,
      replaceUrl: true,
      queryParamsHandling: ''
    });
  }

  submit(): void {
    this.message = null;
    this.error = null;
    this.lastOrder = null;

    const items = this.lines
      .filter((l) => l.productId != null && l.quantity != null && l.quantity > 0)
      .map((l) => ({
        productId: l.productId as number,
        quantity: l.quantity as number
      }));

    if (items.length === 0) {
      this.error = 'Add at least one line with product and quantity.';
      return;
    }

    this.submitting = true;
    const promo = this.promotionCode.trim();
    this.ordersApi
      .create({
        customerId: this.customerId,
        items,
        promotionCode: promo.length ? promo : undefined
      })
      .subscribe({
        next: (order) => {
          this.lastOrder = order;
          this.message = `Order #${order.id} created. Total: ${order.totalAmount}`;
          this.submitting = false;
        },
        error: (e) => {
          this.error = e.error?.error ?? e.message ?? 'Order failed';
          this.submitting = false;
        }
      });
  }
}
