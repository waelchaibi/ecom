import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { PaymentGatewayComponent } from '../../../core/components/payment-gateway/payment-gateway.component';
import { MaterialModule } from '../../../shared/material.module';
import { CartApiService } from '../../../core/services/cart-api.service';
import { ConfirmDialogService } from '../../../core/services/confirm-dialog.service';
import { CustomerAccountApiService } from '../../../core/services/customer-account-api.service';
import { Cart } from '../../../core/models/cart';
import { Order } from '../../../core/models/order';
import { SimulatePaymentPayload } from '../../../core/models/payment';
import { resolveMediaUrl } from '../../../core/utils/media-url';

@Component({
  selector: 'ecom-cart-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, PaymentGatewayComponent, MaterialModule],
  templateUrl: './cart-page.component.html',
  styleUrl: './cart-page.component.scss'
})
export class CartPageComponent implements OnInit {
  private readonly cartApi = inject(CartApiService);
  private readonly accountApi = inject(CustomerAccountApiService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly router = inject(Router);

  cart: Cart | null = null;
  promotionCode = '';
  loading = true;
  busy = false;
  paying = false;
  error: string | null = null;
  message: string | null = null;
  lastOrder: Order | null = null;

  mediaSrc(url: string | null | undefined): string | null {
    return resolveMediaUrl(url);
  }

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

  async remove(productId: number): Promise<void> {
    const item = this.cart?.items.find((i) => i.productId === productId);
    const confirmed = await this.confirmDialog.open({
      title: 'Remove item',
      message: item ? `Remove "${item.productName}" from your cart?` : 'Remove this item from your cart?',
      confirmLabel: 'Remove',
      tone: 'danger'
    });
    if (!confirmed) {
      return;
    }
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

  async checkout(): Promise<void> {
    if (!this.cart?.items.length) return;
    const confirmed = await this.confirmDialog.open({
      title: 'Place order',
      message: `Place order for ${this.cart.items.length} item(s)? Estimated total: ${this.cart.estimatedTotal.toFixed(2)}.`,
      confirmLabel: 'Place order'
    });
    if (!confirmed) {
      return;
    }
    this.busy = true;
    this.error = null;
    this.message = null;
    const promo = this.promotionCode.trim();
    this.cartApi
      .checkout({
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

  payLastOrder(payload: SimulatePaymentPayload): void {
    if (!this.lastOrder || this.lastOrder.status !== 'Pending') return;
    this.paying = true;
    this.error = null;
    this.accountApi.payOrder(this.lastOrder.id, payload).subscribe({
      next: (order) => {
        this.lastOrder = order;
        this.paying = false;
        this.message = `Order #${order.id} paid. Status: ${order.status}.`;
      },
      error: (e) => {
        this.paying = false;
        this.error = e.error?.error ?? e.message ?? 'Payment failed';
      }
    });
  }
}
