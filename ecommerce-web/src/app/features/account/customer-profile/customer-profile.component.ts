import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MaterialModule } from '../../../shared/material.module';
import { CustomerAccountApiService } from '../../../core/services/customer-account-api.service';

@Component({
  selector: 'app-customer-profile',
  standalone: true,
  imports: [FormsModule, RouterLink, MaterialModule],
  templateUrl: './customer-profile.component.html',
  styleUrl: './customer-profile.component.scss'
})
export class CustomerProfileComponent implements OnInit {
  name = '';
  email = '';
  phone = '';
  identityCard = '';
  error = '';
  success = '';
  loading = false;

  private readonly api = inject(CustomerAccountApiService);

  ngOnInit(): void {
    this.api.getProfile().subscribe({
      next: (p) => {
        this.name = p.name;
        this.email = p.email;
        this.phone = p.phone;
        this.identityCard = p.identityCard;
      },
      error: () => (this.error = 'Could not load profile.')
    });
  }

  submit(): void {
    this.error = '';
    this.success = '';
    if (!/^\d{8}$/.test(this.identityCard.trim())) {
      this.error = 'Identity card (CIN) must be exactly 8 digits.';
      return;
    }
    this.loading = true;
    this.api
      .updateProfile({
        name: this.name,
        phone: this.phone,
        identityCard: this.identityCard.trim()
      })
      .subscribe({
        next: (p) => {
          this.loading = false;
          this.name = p.name;
          this.phone = p.phone;
          this.identityCard = p.identityCard;
          this.success = 'Profile updated.';
        },
        error: (e) => {
          this.loading = false;
          this.error = e.error?.error ?? e.message ?? 'Update failed.';
        }
      });
  }
}
