import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { TablerIconsModule } from 'angular-tabler-icons';
import { AdminAuthService } from '../../../../core/services/admin-auth.service';
import { MaterialModule } from '../../../../shared/material.module';

@Component({
  selector: 'ecom-admin-header',
  standalone: true,
  imports: [RouterModule, CommonModule, TablerIconsModule, MaterialModule],
  template: `
    <mat-toolbar class="topbar">
      <button mat-icon-button (click)="toggleMobileNav.emit()" class="d-flex d-md-none justify-content-center">
        <i-tabler name="menu-2" class="icon-20 d-flex"></i-tabler>
      </button>

      <span class="f-w-600 m-l-8 d-none d-md-inline">Admin console</span>
      <span class="flex-1-auto"></span>

      <button mat-stroked-button color="primary" type="button" class="m-r-8" (click)="viewStore()">
        View store
      </button>

      <button mat-icon-button [matMenuTriggerFor]="profilemenu" aria-label="Account">
        <i-tabler name="user-circle" class="icon-24 d-flex"></i-tabler>
      </button>
      <mat-menu #profilemenu="matMenu" class="cardWithShadow">
        <button mat-menu-item disabled>
          <mat-icon class="d-flex align-items-center"
            ><i-tabler name="user" class="icon-18 d-flex"></i-tabler
          ></mat-icon>
          Admin
        </button>
        <div class="p-x-12 m-t-12 m-b-12">
          <button mat-stroked-button color="primary" class="w-100" type="button" (click)="logout()">Logout</button>
        </div>
      </mat-menu>
    </mat-toolbar>
  `
})
export class AdminHeaderComponent {
  @Input() showToggle = true;
  @Output() toggleMobileNav = new EventEmitter<void>();
  @Output() toggleCollapsed = new EventEmitter<void>();

  private readonly auth = inject(AdminAuthService);
  private readonly router = inject(Router);

  viewStore(): void {
    void this.router.navigateByUrl('/products');
  }

  logout(): void {
    this.auth.logout();
    void this.router.navigateByUrl('/login');
  }
}
