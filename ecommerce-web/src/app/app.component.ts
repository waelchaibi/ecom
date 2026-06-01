import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { CustomerAuthService } from './core/services/customer-auth.service';

@Component({
  selector: 'ecom-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  readonly customerAuth = inject(CustomerAuthService);
  private readonly router = inject(Router);

  customerLogout(): void {
    this.customerAuth.logout();
    void this.router.navigateByUrl('/products');
  }
}
