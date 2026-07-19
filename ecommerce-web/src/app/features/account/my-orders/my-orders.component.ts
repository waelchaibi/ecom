import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { MaterialModule } from '../../../shared/material.module';
import { Order } from '../../../core/models/order';
import { CustomerAccountApiService } from '../../../core/services/customer-account-api.service';
import { CustomerAuthService } from '../../../core/services/customer-auth.service';

@Component({
  selector: 'app-my-orders',
  standalone: true,
  imports: [CommonModule, RouterLink, MaterialModule],
  templateUrl: './my-orders.component.html',
  styleUrl: './my-orders.component.scss'
})
export class MyOrdersComponent implements OnInit {
  private readonly api = inject(CustomerAccountApiService);
  private readonly router = inject(Router);
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
    void this.router.navigateByUrl('/account/login');
  }

  statusClass(status: string): string {
    const s = status.toLowerCase();
    if (s === 'pending') return 'badge badge-pending';
    if (s === 'cancelled') return 'badge badge-cancelled';
    return 'badge badge-confirmed';
  }
}
