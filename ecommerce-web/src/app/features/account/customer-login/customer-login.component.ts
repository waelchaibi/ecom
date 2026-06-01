import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CustomerAuthService } from '../../../core/services/customer-auth.service';

@Component({
  selector: 'app-customer-login',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './customer-login.component.html',
  styleUrl: './customer-login.component.scss'
})
export class CustomerLoginComponent implements OnInit {
  email = '';
  password = '';
  error = '';
  loading = false;
  returnUrl = '/order';

  private readonly auth = inject(CustomerAuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  ngOnInit(): void {
    if (this.auth.token()) {
      void this.router.navigateByUrl('/account/orders');
      return;
    }
    const ret = this.route.snapshot.queryParamMap.get('returnUrl');
    if (ret) {
      this.returnUrl = ret;
    }
  }

  submit(): void {
    this.error = '';
    this.loading = true;
    this.auth.login({ email: this.email, password: this.password }).subscribe({
      next: () => {
        this.loading = false;
        void this.router.navigateByUrl(this.returnUrl);
      },
      error: () => {
        this.loading = false;
        this.error = 'Invalid email or password.';
      }
    });
  }
}
