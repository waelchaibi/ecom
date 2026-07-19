import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RouterModule } from '@angular/router';
import { TablerIconsModule } from 'angular-tabler-icons';
import { MaterialModule } from '../../../../shared/material.module';
import { AdminBrandingComponent } from './branding.component';

@Component({
  selector: 'ecom-admin-sidebar',
  standalone: true,
  imports: [AdminBrandingComponent, TablerIconsModule, MaterialModule, RouterModule],
  template: `
    <div class="d-flex align-items-center justify-content-between">
      <ecom-admin-branding />
      @if (showToggle) {
        <a
          href="javascript:void(0)"
          (click)="toggleMobileNav.emit()"
          class="d-flex justify-content-center icon-40 align-items-center mat-body-1"
        >
          <i-tabler name="x" class="icon-20 d-flex"></i-tabler>
        </a>
      }
    </div>
  `
})
export class AdminSidebarComponent {
  @Input() showToggle = true;
  @Output() toggleMobileNav = new EventEmitter<void>();
}
