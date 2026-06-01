import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Order } from '../../../core/models/order';
import { AdminApiService } from '../../../core/services/admin-api.service';

@Component({
  selector: 'app-admin-order-detail',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './admin-order-detail.component.html',
  styleUrl: './admin-order-detail.component.scss'
})
export class AdminOrderDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(AdminApiService);

  orderId = 0;
  order: Order | null = null;
  err = '';
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
    this.api.getOrder(this.orderId).subscribe({
      next: (o) => (this.order = o),
      error: () => (this.err = 'Order not found or access denied.')
    });
  }

  confirm(): void {
    if (!this.order) {
      return;
    }
    this.err = '';
    this.busy = true;
    this.api.confirmPayment(this.order.id).subscribe({
      next: (o) => {
        this.order = o;
        this.busy = false;
      },
      error: (e) => {
        this.busy = false;
        this.err = e.error?.error ?? e.message ?? 'Confirm failed';
      }
    });
  }

  cancel(): void {
    if (!this.order) {
      return;
    }
    if (!confirm(`Cancel order #${this.order.id}? Stock will be restored.`)) {
      return;
    }
    this.err = '';
    this.busy = true;
    this.api.cancelOrder(this.order.id).subscribe({
      next: (o) => {
        this.order = o;
        this.busy = false;
      },
      error: (e) => {
        this.busy = false;
        this.err = e.error?.error ?? e.message ?? 'Cancel failed';
      }
    });
  }
}
