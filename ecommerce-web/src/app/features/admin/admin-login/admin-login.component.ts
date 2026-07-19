import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AdminAuthService } from '../../../core/services/admin-auth.service';
import { AuthCoordinatorService } from '../../../core/services/auth-coordinator.service';
import { MaterialModule } from '../../../shared/material.module';

@Component({
  selector: 'app-admin-login',
  standalone: true,
  imports: [FormsModule, RouterLink, MaterialModule],
  templateUrl: './admin-login.component.html',
  styleUrl: './admin-login.component.scss'
})
export class AdminLoginComponent implements OnInit {
  username = '';
  password = '';
  error = '';
  loading = false;

  private readonly auth = inject(AdminAuthService);
  private readonly coordinator = inject(AuthCoordinatorService);
  private readonly router = inject(Router);

  ngOnInit(): void {
    if (this.auth.isAdminSession()) {
      void this.router.navigateByUrl('/admin/dashboard');
    }
  }

  submit(): void {
    this.error = '';
    this.loading = true;
    this.auth.login(this.username, this.password).subscribe({
      next: () => {
        this.coordinator.onAdminLoginSuccess();
        this.loading = false;
        void this.router.navigateByUrl('/admin/dashboard');
      },
      error: () => {
        this.loading = false;
        this.error = 'Invalid credentials or server error.';
      }
    });
  }
}
