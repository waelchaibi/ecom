import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CustomerAuthService } from '../../../core/services/customer-auth.service';
import { AuthCoordinatorService } from '../../../core/services/auth-coordinator.service';

@Component({
  selector: 'app-customer-register',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './customer-register.component.html',
  styleUrl: './customer-register.component.scss'
})
export class CustomerRegisterComponent {
  name = '';
  email = '';
  phone = '';
  password = '';
  error = '';
  loading = false;

  private readonly auth = inject(CustomerAuthService);
  private readonly coordinator = inject(AuthCoordinatorService);
  private readonly router = inject(Router);

  submit(): void {
    this.error = '';
    this.loading = true;
    this.auth.register({ name: this.name, email: this.email, phone: this.phone, password: this.password }).subscribe({
      next: () => {
        this.coordinator.onCustomerLoginSuccess();
        this.loading = false;
        void this.router.navigateByUrl('/order');
      },
      error: (e) => {
        this.loading = false;
        this.error = e.error?.error ?? e.message ?? 'Registration failed.';
      }
    });
  }
}
