import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { PaymentGatewayComponent } from '../../../core/components/payment-gateway/payment-gateway.component';
import { CheckoutApiService } from '../../../core/services/checkout-api.service';
import { ProductApiService } from '../../../core/services/product-api.service';
import { OrderApiService } from '../../../core/services/order-api.service';
import { CustomerAccountApiService } from '../../../core/services/customer-account-api.service';
import { CustomerAuthService } from '../../../core/services/customer-auth.service';
import { ConfirmDialogService } from '../../../core/services/confirm-dialog.service';
import { CheckoutPricing } from '../../../core/models/checkout-pricing';
import { Product } from '../../../core/models/product';
import { Order } from '../../../core/models/order';
import { SimulatePaymentPayload } from '../../../core/models/payment';

interface LineDraft {
  productId: number | null;
  quantity: number | null;
}

@Component({
  selector: 'ecom-order-create',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, PaymentGatewayComponent],
  templateUrl: './order-create.component.html',
  styleUrl: './order-create.component.scss'
})
export class OrderCreateComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly productsApi = inject(ProductApiService);
  private readonly ordersApi = inject(OrderApiService);
  private readonly checkoutApi = inject(CheckoutApiService);
  private readonly accountApi = inject(CustomerAccountApiService);
  readonly auth = inject(CustomerAuthService);
  private readonly confirmDialog = inject(ConfirmDialogService);

  products: Product[] = [];
  promotionCode = '';
  pricing: CheckoutPricing | null = null;
  lines: LineDraft[] = [{ productId: null, quantity: 1 }];
  submitting = false;
  paying = false;
  message: string | null = null;
  error: string | null = null;
  lastOrder: Order | null = null;

  ngOnInit(): void {
    this.productsApi.getAll().subscribe({
      next: (products) => {
        this.products = products;
        const pid = Number(this.route.snapshot.queryParamMap.get('productId'));
        if (!Number.isNaN(pid) && pid > 0) {
          this.lines = [{ productId: pid, quantity: 1 }];
        }
        this.refreshPricing();
      },
      error: (e) => {
        this.error = e.error?.error ?? e.message ?? 'Could not load products';
      }
    });
  }

  addLine(): void {
    this.lines.push({ productId: null, quantity: 1 });
    this.syncUrl();
    this.refreshPricing();
  }

  removeLine(index: number): void {
    this.lines.splice(index, 1);
    if (this.lines.length === 0) {
      this.lines.push({ productId: null, quantity: 1 });
    }
    this.syncUrl();
    this.refreshPricing();
  }

  onLineChange(): void {
    this.syncUrl();
    this.refreshPricing();
  }

  private syncUrl(): void {
    const q: Record<string, string | number> = {};
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

  private refreshPricing(): void {
    const subtotal = this.computeSubtotal();
    if (subtotal <= 0) {
      this.pricing = null;
      return;
    }
    this.checkoutApi.getPricing(subtotal).subscribe({
      next: (p) => (this.pricing = p),
      error: () => (this.pricing = null)
    });
  }

  private computeSubtotal(): number {
    return this.lines.reduce((sum, line) => {
      if (line.productId == null || line.quantity == null || line.quantity <= 0) {
        return sum;
      }
      const product = this.products.find((p) => p.id === line.productId);
      return sum + (product?.price ?? 0) * line.quantity;
    }, 0);
  }

  async submit(): Promise<void> {
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

    const subtotal = this.computeSubtotal();
    const confirmed = await this.confirmDialog.open({
      title: 'Place order',
      message: `Place order with ${items.length} line(s)? Estimated subtotal: ${subtotal.toFixed(2)}.`,
      confirmLabel: 'Place order'
    });
    if (!confirmed) {
      return;
    }

    this.submitting = true;
    const promo = this.promotionCode.trim();
    this.ordersApi
      .create({
        items,
        promotionCode: promo.length ? promo : undefined
      })
      .subscribe({
        next: (order) => {
          this.lastOrder = order;
          this.message = `Order #${order.id} placed. Total: ${order.totalAmount}. Pay below to confirm.`;
          this.submitting = false;
        },
        error: (e) => {
          this.error = e.error?.error ?? e.message ?? 'Order failed';
          this.submitting = false;
        }
      });
  }

  payLastOrder(payload: SimulatePaymentPayload): void {
    if (!this.lastOrder || this.lastOrder.status !== 'Pending') {
      return;
    }
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
        this.error = e.error?.error ?? e.message ?? 'Payment failed.';
      }
    });
  }
}
