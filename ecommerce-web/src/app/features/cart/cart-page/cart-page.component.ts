import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CartApiService } from '../../../core/services/cart-api.service';
import { CustomerAccountApiService } from '../../../core/services/customer-account-api.service';
import { Cart } from '../../../core/models/cart';
import { Order } from '../../../core/models/order';

@Component({
  selector: 'ecom-cart-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './cart-page.component.html',
  styleUrl: './cart-page.component.scss'
})
export class CartPageComponent implements OnInit {
  private readonly cartApi = inject(CartApiService);
  private readonly accountApi = inject(CustomerAccountApiService);
  private readonly router = inject(Router);

  cart: Cart | null = null;
  taxAmount = 0;
  shippingAmount = 0;
  promotionCode = '';
  loading = true;
  busy = false;
  paying = false;
  error: string | null = null;
  message: string | null = null;
  lastOrder: Order | null = null;

  ngOnInit(): void {
    this.reload();
  }

  reload(): void {
    this.loading = true;
    this.error = null;
    this.cartApi.get().subscribe({
      next: (c) => {
        this.cart = c;
        this.loading = false;
      },
      error: (e) => {
        this.error = e.error?.error ?? e.message ?? 'Could not load cart';
        this.loading = false;
      }
    });
  }

  setQty(productId: number, quantity: number): void {
    if (quantity < 0) return;
    this.busy = true;
    this.cartApi.upsertItem(productId, quantity).subscribe({
      next: (c) => {
        this.cart = c;
        this.busy = false;
      },
      error: (e) => {
        this.busy = false;
        this.error = e.error?.error ?? e.message ?? 'Update failed';
      }
    });
  }

  remove(productId: number): void {
    this.busy = true;
    this.cartApi.removeItem(productId).subscribe({
      next: (c) => {
        this.cart = c;
        this.busy = false;
      },
      error: (e) => {
        this.busy = false;
        this.error = e.error?.error ?? e.message ?? 'Remove failed';
      }
    });
  }

  checkout(): void {
    if (!this.cart?.items.length) return;
    this.busy = true;
    this.error = null;
    this.message = null;
    const promo = this.promotionCode.trim();
    this.cartApi
      .checkout({
        taxAmount: this.taxAmount,
        shippingAmount: this.shippingAmount,
        promotionCode: promo.length ? promo : undefined
      })
      .subscribe({
        next: (order) => {
          this.lastOrder = order;
          this.busy = false;
          this.message = `Order #${order.id} placed. Total: ${order.totalAmount}.`;
          this.reload();
        },
        error: (e) => {
          this.busy = false;
          this.error = e.error?.error ?? e.message ?? 'Checkout failed';
        }
      });
  }

  payLastOrder(): void {
    if (!this.lastOrder || this.lastOrder.status !== 'Pending') return;
    this.paying = true;
    this.accountApi.payOrder(this.lastOrder.id).subscribe({
      next: (order) => {
        this.lastOrder = order;
        this.paying = false;
        this.message = `Order #${order.id} paid (simulated).`;
      },
      error: (e) => {
        this.paying = false;
        this.error = e.error?.error ?? e.message ?? 'Payment failed';
      }
    });
  }
}
