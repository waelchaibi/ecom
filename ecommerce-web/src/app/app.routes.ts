import { Routes } from '@angular/router';
import { adminAuthGuard } from './core/guards/admin-auth.guard';
import { customerAuthGuard } from './core/guards/customer-auth.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'products' },
  {
    path: 'products',
    loadComponent: () =>
      import('./features/products/product-list/product-list.component').then((m) => m.ProductListComponent)
  },
  {
    path: 'cart',
    canActivate: [customerAuthGuard],
    loadComponent: () =>
      import('./features/cart/cart-page/cart-page.component').then((m) => m.CartPageComponent)
  },
  {
    path: 'order',
    canActivate: [customerAuthGuard],
    loadComponent: () =>
      import('./features/orders/order-create/order-create.component').then((m) => m.OrderCreateComponent)
  },
  {
    path: 'account/login',
    loadComponent: () =>
      import('./features/account/customer-login/customer-login.component').then((m) => m.CustomerLoginComponent)
  },
  {
    path: 'account/register',
    loadComponent: () =>
      import('./features/account/customer-register/customer-register.component').then((m) => m.CustomerRegisterComponent)
  },
  {
    path: 'account/orders',
    canActivate: [customerAuthGuard],
    loadComponent: () =>
      import('./features/account/my-orders/my-orders.component').then((m) => m.MyOrdersComponent)
  },
  {
    path: 'account/orders/:id',
    canActivate: [customerAuthGuard],
    loadComponent: () =>
      import('./features/account/my-order-detail/my-order-detail.component').then((m) => m.MyOrderDetailComponent)
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/admin/admin-login/admin-login.component').then((m) => m.AdminLoginComponent)
  },
  { path: 'admin/login', redirectTo: 'login', pathMatch: 'full' },
  {
    path: 'admin',
    loadComponent: () =>
      import('./features/admin/admin-layout/admin-layout.component').then((m) => m.AdminLayoutComponent),
    canActivate: [adminAuthGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./features/admin/admin-dashboard/admin-dashboard.component').then((m) => m.AdminDashboardComponent)
      },
      {
        path: 'products',
        loadComponent: () =>
          import('./features/admin/admin-products/admin-products.component').then((m) => m.AdminProductsComponent)
      },
      {
        path: 'categories',
        loadComponent: () =>
          import('./features/admin/admin-categories/admin-categories.component').then(
            (m) => m.AdminCategoriesComponent
          )
      },
      {
        path: 'gift-rules',
        loadComponent: () =>
          import('./features/admin/admin-gift-rules/admin-gift-rules.component').then((m) => m.AdminGiftRulesComponent)
      },
      {
        path: 'orders',
        loadComponent: () =>
          import('./features/admin/admin-orders/admin-orders.component').then((m) => m.AdminOrdersComponent)
      },
      {
        path: 'audit-logs',
        loadComponent: () =>
          import('./features/admin/admin-audit-logs/admin-audit-logs.component').then(
            (m) => m.AdminAuditLogsComponent
          )
      },
      {
        path: 'orders/:id',
        loadComponent: () =>
          import('./features/admin/admin-order-detail/admin-order-detail.component').then(
            (m) => m.AdminOrderDetailComponent
          )
      },
      {
        path: 'customers',
        loadComponent: () =>
          import('./features/admin/admin-customers/admin-customers.component').then((m) => m.AdminCustomersComponent)
      },
      {
        path: 'customers/:id',
        loadComponent: () =>
          import('./features/admin/admin-customer-orders/admin-customer-orders.component').then(
            (m) => m.AdminCustomerOrdersComponent
          )
      }
    ]
  }
];
