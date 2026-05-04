import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'products' },
  {
    path: 'products',
    loadComponent: () =>
      import('./features/products/product-list/product-list.component').then((m) => m.ProductListComponent)
  },
  {
    path: 'order',
    loadComponent: () =>
      import('./features/orders/order-create/order-create.component').then((m) => m.OrderCreateComponent)
  }
];
