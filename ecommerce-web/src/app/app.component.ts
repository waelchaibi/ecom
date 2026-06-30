import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { ConfirmDialogComponent } from './core/components/confirm-dialog/confirm-dialog.component';
import { AdminAuthService } from './core/services/admin-auth.service';
import { AuthCoordinatorService } from './core/services/auth-coordinator.service';
import { CustomerAuthService } from './core/services/customer-auth.service';

@Component({
  selector: 'ecom-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, ConfirmDialogComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  readonly customerAuth = inject(CustomerAuthService);
  readonly adminAuth = inject(AdminAuthService);
  private readonly coordinator = inject(AuthCoordinatorService);
  private readonly router = inject(Router);

  ngOnInit(): void {
    this.coordinator.resolveConflictingSessions();
  }

  customerLogout(): void {
    this.customerAuth.logout();
    void this.router.navigateByUrl('/products');
  }
}
