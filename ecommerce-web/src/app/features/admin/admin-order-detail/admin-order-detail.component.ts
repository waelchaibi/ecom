import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Order } from '../../../core/models/order';
import { AdminApiService } from '../../../core/services/admin-api.service';
import { ConfirmDialogService } from '../../../core/services/confirm-dialog.service';

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
  private readonly confirmDialog = inject(ConfirmDialogService);

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

  async confirm(): Promise<void> {
    if (!this.order) {
      return;
    }
    const confirmed = await this.confirmDialog.open({
      title: 'Confirm payment',
      message: `Confirm payment for order #${this.order.id}? Gifts will be applied if eligible.`,
      confirmLabel: 'Confirm payment'
    });
    if (!confirmed) return;
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

  async cancel(): Promise<void> {
    if (!this.order) {
      return;
    }
    const confirmed = await this.confirmDialog.open({
      title: 'Cancel order',
      message: `Cancel order #${this.order.id}? Stock will be restored.`,
      confirmLabel: 'Cancel order',
      tone: 'danger'
    });
    if (!confirmed) {
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
