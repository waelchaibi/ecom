import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Order } from '../../../core/models/order';
import { AdminApiService } from '../../../core/services/admin-api.service';

@Component({
  selector: 'app-admin-customer-orders',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './admin-customer-orders.component.html',
  styleUrl: './admin-customer-orders.component.scss'
})
export class AdminCustomerOrdersComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(AdminApiService);

  customerId = 0;
  orders: Order[] = [];
  err = '';

  ngOnInit(): void {
    this.customerId = Number(this.route.snapshot.paramMap.get('id'));
    if (!Number.isFinite(this.customerId)) {
      this.err = 'Invalid customer id.';
      return;
    }
    this.api.getCustomerOrders(this.customerId).subscribe({
      next: (o) => (this.orders = o),
      error: () => (this.err = 'Could not load order history.')
    });
  }
}
