import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ProductMediaComponent } from '../../../core/components/product-media/product-media.component';
import { MaterialModule } from '../../../shared/material.module';
import { Product } from '../../../core/models/product';
import { CartApiService } from '../../../core/services/cart-api.service';
import { ConfirmDialogService } from '../../../core/services/confirm-dialog.service';
import { CustomerAuthService } from '../../../core/services/customer-auth.service';
import { ProductApiService } from '../../../core/services/product-api.service';

@Component({
  selector: 'ecom-product-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, ProductMediaComponent, MaterialModule],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.scss'
})
export class ProductDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly productsApi = inject(ProductApiService);
  private readonly cartApi = inject(CartApiService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  readonly auth = inject(CustomerAuthService);

  product: Product | null = null;
  categoryId: number | null = null;
  loading = true;
  error: string | null = null;
  cartMsg: string | null = null;

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const id = Number(params.get('id'));
      if (!Number.isFinite(id) || id <= 0) {
        this.error = 'Invalid product.';
        this.loading = false;
        return;
      }
      this.loadProduct(id);
    });

    this.route.queryParamMap.subscribe((params) => {
      const raw = params.get('categoryId');
      this.categoryId = raw && !Number.isNaN(+raw) ? +raw : null;
    });
  }

  shopLink(): string[] {
    return ['/products'];
  }

  shopQueryParams(): Record<string, number> | null {
    return this.categoryId != null ? { categoryId: this.categoryId } : null;
  }

  async addToCart(): Promise<void> {
    if (!this.product) {
      return;
    }
    if (!this.auth.isLoggedIn()) {
      void this.router.navigate(['/account/login'], {
        queryParams: { returnUrl: this.router.url }
      });
      return;
    }
    const confirmed = await this.confirmDialog.open({
      title: 'Add to cart',
      message: `Add "${this.product.name}" to your cart?`,
      confirmLabel: 'Add to cart'
    });
    if (!confirmed) {
      return;
    }
    this.cartMsg = null;
    this.cartApi.upsertItem(this.product.id, 1).subscribe({
      next: () => (this.cartMsg = `Added ${this.product!.name} to cart.`),
      error: (e) => (this.error = e.error?.error ?? e.message ?? 'Could not add to cart')
    });
  }

  private loadProduct(id: number): void {
    this.loading = true;
    this.error = null;
    this.cartMsg = null;
    this.productsApi.getById(id).subscribe({
      next: (p) => {
        this.product = p;
        this.loading = false;
      },
      error: (e) => {
        this.error = e.status === 404 ? 'Product not found.' : e.error?.error ?? e.message ?? 'Failed to load product';
        this.loading = false;
      }
    });
  }
}
