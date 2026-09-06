import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'ecom-admin-branding',
  standalone: true,
  imports: [RouterModule],
  template: `
    <div class="branding">
      <a [routerLink]="['/admin/dashboard']" class="d-flex align-items-center text-decoration-none m-2">
        <span
          class="d-inline-flex align-items-center justify-content-center rounded bg-primary text-white f-w-700 m-r-8"
          style="width: 36px; height: 36px"
          >S</span
        >
        <span class="f-w-600 f-s-16 text-dark">Olympia Admin</span>
      </a>
    </div>
  `
})
export class AdminBrandingComponent {}
