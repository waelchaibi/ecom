import { Component, inject, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { ChatWidgetComponent } from '../../core/components/chat-widget/chat-widget.component';
import { AdminAuthService } from '../../core/services/admin-auth.service';
import { AuthCoordinatorService } from '../../core/services/auth-coordinator.service';
import { CustomerAuthService } from '../../core/services/customer-auth.service';
import { MaterialModule } from '../../shared/material.module';

@Component({
  selector: 'ecom-storefront-layout',
  standalone: true,
  imports: [RouterOutlet, ChatWidgetComponent, MaterialModule],
  templateUrl: './storefront-layout.component.html'
})
export class StorefrontLayoutComponent implements OnInit {
  readonly customerAuth = inject(CustomerAuthService);
  readonly adminAuth = inject(AdminAuthService);
  private readonly coordinator = inject(AuthCoordinatorService);
  private readonly router = inject(Router);

  ngOnInit(): void {
    this.coordinator.resolveConflictingSessions();
  }

  go(event: Event, url: string): void {
    event.preventDefault();
    void this.router.navigateByUrl(url);
  }

  isActive(url: string): boolean {
    return this.router.isActive(url, {
      paths: 'subset',
      queryParams: 'ignored',
      fragment: 'ignored',
      matrixParams: 'ignored'
    });
  }

  customerLogout(): void {
    this.customerAuth.logout();
    void this.router.navigateByUrl('/account/login');
  }

  adminLogout(): void {
    this.adminAuth.logout();
    void this.router.navigateByUrl('/login');
  }
}
