import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ProductApiService } from '../../../core/services/product-api.service';
import { Product } from '../../../core/models/product';

@Component({
  selector: 'ecom-product-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.scss'
})
export class ProductListComponent implements OnInit {
  private readonly productsApi = inject(ProductApiService);

  products: Product[] = [];
  error: string | null = null;
  loading = true;

  ngOnInit(): void {
    this.productsApi.getAll().subscribe({
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
