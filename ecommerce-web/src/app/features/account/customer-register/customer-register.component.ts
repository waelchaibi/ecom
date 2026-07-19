import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CustomerAuthService } from '../../../core/services/customer-auth.service';
import { AuthCoordinatorService } from '../../../core/services/auth-coordinator.service';
import { MaterialModule } from '../../../shared/material.module';

@Component({
  selector: 'app-customer-register',
  standalone: true,
  imports: [FormsModule, RouterLink, MaterialModule],
  templateUrl: './customer-register.component.html',
  styleUrl: './customer-register.component.scss'
})
export class CustomerRegisterComponent {
  name = '';
  email = '';
  phone = '';
  identityCard = '';
  password = '';
  error = '';
  loading = false;

  private readonly auth = inject(CustomerAuthService);
  private readonly coordinator = inject(AuthCoordinatorService);
  private readonly router = inject(Router);

  submit(): void {
    this.error = '';
    if (!/^\d{8}$/.test(this.identityCard.trim())) {
      this.error = 'Identity card (CIN) must be exactly 8 digits.';
      return;
    }
    this.loading = true;
    this.auth
      .register({
        name: this.name,
        email: this.email,
        phone: this.phone,
        identityCard: this.identityCard.trim(),
        password: this.password
      })
      .subscribe({
        next: () => {
          this.coordinator.onCustomerLoginSuccess();
          this.loading = false;
          void this.router.navigateByUrl('/products');
        },
        error: (e) => {
          this.loading = false;
          this.error = e.error?.error ?? e.message ?? 'Registration failed.';
        }
      });
  }
}
