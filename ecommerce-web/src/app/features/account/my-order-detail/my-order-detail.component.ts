import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Order } from '../../../core/models/order';
import { CustomerAccountApiService } from '../../../core/services/customer-account-api.service';

@Component({
  selector: 'app-my-order-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './my-order-detail.component.html',
  styleUrl: './my-order-detail.component.scss'
})
export class MyOrderDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(CustomerAccountApiService);

  orderId = 0;
  order: Order | null = null;
  err = '';
  msg = '';
  busy = false;

  ngOnInit(): void {
    this.orderId = Number(this.route.snapshot.paramMap.get('id'));
    if (!Number.isFinite(this.orderId)) {
      this.err = 'Invalid order id.';
      return;
    }
    this.load();
  }

  load(): void {
    this.err = '';
    this.api.getMyOrder(this.orderId).subscribe({
      next: (o) => (this.order = o),
      error: () => (this.err = 'Order not found.')
    });
  }

  pay(): void {
    if (!this.order || this.order.status !== 'Pending') {
      return;
    }
    this.busy = true;
    this.msg = '';
    this.api.payOrder(this.order.id).subscribe({
      next: (o) => {
        this.order = o;
        this.busy = false;
        this.msg = 'Payment simulated successfully. Your order is confirmed.';
      },
      error: (e) => {
        this.busy = false;
        this.err = e.error?.error ?? e.message ?? 'Payment failed.';
      }
    });
  }

  statusClass(status: string): string {
    const s = status.toLowerCase();
    if (s === 'pending') return 'badge badge-pending';
    if (s === 'cancelled') return 'badge badge-cancelled';
    return 'badge badge-confirmed';
  }

  cancel(): void {
    if (!this.order || this.order.status !== 'Pending') {
      return;
    }
    if (!confirm('Cancel this order and release reserved stock?')) {
      return;
    }
    this.busy = true;
    this.msg = '';
    this.api.cancelOrder(this.order.id).subscribe({
      next: (o) => {
        this.order = o;
        this.busy = false;
        this.msg = 'Order cancelled.';
      },
      error: (e) => {
        this.busy = false;
        this.err = e.error?.error ?? e.message ?? 'Cancel failed.';
      }
    });
  }
}
