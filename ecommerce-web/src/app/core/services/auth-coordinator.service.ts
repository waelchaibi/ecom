import { Injectable, inject } from '@angular/core';
import { AdminAuthService } from './admin-auth.service';
import { CustomerAuthService } from './customer-auth.service';

/** Ensures only one storefront role session is active at a time. */
@Injectable({ providedIn: 'root' })
export class AuthCoordinatorService {
  private readonly admin = inject(AdminAuthService);
  private readonly customer = inject(CustomerAuthService);

  onAdminLoginSuccess(): void {
    this.customer.logout();
  }

  onCustomerLoginSuccess(): void {
    this.admin.logout();
  }

  /** If both JWTs exist (e.g. after switching accounts), keep customer on the storefront. */
  resolveConflictingSessions(): void {
    if (this.customer.isCustomerSession() && this.admin.isAdminSession()) {
      this.admin.logout();
    }
  }
}
