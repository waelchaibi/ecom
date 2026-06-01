import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ProductApiService } from '../../../core/services/product-api.service';
import { CategoryApiService } from '../../../core/services/category-api.service';
import { CartApiService } from '../../../core/services/cart-api.service';
import { CustomerAuthService } from '../../../core/services/customer-auth.service';
import { Product } from '../../../core/models/product';
import { Category } from '../../../core/models/category';

@Component({
  selector: 'ecom-product-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.scss'
})
export class ProductListComponent implements OnInit {
  private readonly productsApi = inject(ProductApiService);
  private readonly categoriesApi = inject(CategoryApiService);
  private readonly cartApi = inject(CartApiService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  readonly auth = inject(CustomerAuthService);

  products: Product[] = [];
  categories: Category[] = [];
  categoryId: number | null = null;
  error: string | null = null;
  cartMsg: string | null = null;
  loading = true;

  ngOnInit(): void {
    this.categoriesApi.getAll().subscribe({
      next: (c) => (this.categories = c),
      error: () => {}
    });

    this.route.queryParamMap.subscribe((params) => {
      const raw = params.get('categoryId');
      this.categoryId = raw && !Number.isNaN(+raw) ? +raw : null;
      this.loadProducts();
    });
  }

  selectCategory(id: number | null): void {
    const q: Record<string, number> = id != null ? { categoryId: id } : {};
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: q,
      queryParamsHandling: ''
    });
  }

  addToCart(p: Product): void {
    if (!this.auth.isLoggedIn()) {
      this.router.navigate(['/account/login'], { queryParams: { returnUrl: '/products' } });
      return;
    }
    this.cartMsg = null;
    this.cartApi.upsertItem(p.id, 1).subscribe({
      next: () => (this.cartMsg = `Added ${p.name} to cart.`),
      error: (e) => (this.error = e.error?.error ?? e.message ?? 'Could not add to cart')
    });
  }

  private loadProducts(): void {
    this.loading = true;
    this.error = null;
    this.productsApi.getAll(this.categoryId).subscribe({
      next: (data) => {
        this.products = data;
        this.loading = false;
      },
      error: (e) => {
        this.error = e.error?.error ?? e.message ?? 'Failed to load products';
        this.loading = false;
      }
    });
  }
}
