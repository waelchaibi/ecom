import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Order } from '../../../core/models/order';
import { Customer } from '../../../core/models/customer';
import { AdminApiService } from '../../../core/services/admin-api.service';
import { MaterialModule } from '../../../shared/material.module';

@Component({
  selector: 'app-admin-customer-orders',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, MaterialModule],
  templateUrl: './admin-customer-orders.component.html',
  styleUrl: './admin-customer-orders.component.scss'
})
export class AdminCustomerOrdersComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(AdminApiService);

  customerId = 0;
  customer: Customer | null = null;
  orders: Order[] = [];
  err = '';

  ngOnInit(): void {
    this.customerId = Number(this.route.snapshot.paramMap.get('id'));
    if (!Number.isFinite(this.customerId)) {
      this.err = 'Invalid customer id.';
      return;
    }
    this.api.getCustomer(this.customerId).subscribe({
      next: (c) => (this.customer = c),
      error: () => (this.err = 'Could not load customer.')
    });
    this.api.getCustomerOrders(this.customerId).subscribe({
      next: (o) => (this.orders = o),
      error: () => (this.err = 'Could not load order history.')
    });
  }
}
