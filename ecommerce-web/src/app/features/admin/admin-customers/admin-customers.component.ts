import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Customer } from '../../../core/models/customer';
import { AdminApiService } from '../../../core/services/admin-api.service';
import { MaterialModule } from '../../../shared/material.module';

@Component({
  selector: 'app-admin-customers',
  standalone: true,
  imports: [CommonModule, RouterLink, MaterialModule],
  templateUrl: './admin-customers.component.html',
  styleUrl: './admin-customers.component.scss'
})
export class AdminCustomersComponent implements OnInit {
  private readonly api = inject(AdminApiService);
  customers: Customer[] = [];
  err = '';

  ngOnInit(): void {
    this.api.getAdminCustomers().subscribe({
      next: (c) => (this.customers = c),
      error: () => (this.err = 'Failed to load customers.')
    });
  }
}
