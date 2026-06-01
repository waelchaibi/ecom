import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AdminAuthService } from '../../../core/services/admin-auth.service';

@Component({
  selector: 'app-admin-login',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './admin-login.component.html',
  styleUrl: './admin-login.component.scss'
})
export class AdminLoginComponent implements OnInit {
  username = '';
  password = '';
  error = '';
  loading = false;

  private readonly auth = inject(AdminAuthService);
  private readonly router = inject(Router);

  ngOnInit(): void {
    if (this.auth.token()) {
      void this.router.navigateByUrl('/admin/dashboard');
    }
  }

  submit(): void {
    this.error = '';
    this.loading = true;
    this.auth.login(this.username, this.password).subscribe({
      next: () => {
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
