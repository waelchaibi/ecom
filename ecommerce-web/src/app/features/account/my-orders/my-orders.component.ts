import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Order } from '../../../core/models/order';
import { CustomerAccountApiService } from '../../../core/services/customer-account-api.service';
import { CustomerAuthService } from '../../../core/services/customer-auth.service';

@Component({
  selector: 'app-my-orders',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './my-orders.component.html',
  styleUrl: './my-orders.component.scss'
})
export class MyOrdersComponent implements OnInit {
  private readonly api = inject(CustomerAccountApiService);
  readonly auth = inject(CustomerAuthService);

  orders: Order[] = [];
  err = '';

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.api.getMyOrders().subscribe({
      next: (o) => (this.orders = o),
      error: () => (this.err = 'Could not load your orders.')
    });
  }

  logout(): void {
    this.auth.logout();
  }

  statusClass(status: string): string {
    const s = status.toLowerCase();
    if (s === 'pending') return 'badge badge-pending';
    if (s === 'cancelled') return 'badge badge-cancelled';
    return 'badge badge-confirmed';
  }
}
