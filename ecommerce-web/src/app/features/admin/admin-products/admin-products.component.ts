import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Product } from '../../../core/models/product';
import { AdminApiService } from '../../../core/services/admin-api.service';
import { ConfirmDialogService } from '../../../core/services/confirm-dialog.service';
import { ProductApiService } from '../../../core/services/product-api.service';

interface CategoryOption {
  id: number;
  name: string;
}

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-products.component.html',
  styleUrl: './admin-products.component.scss'
})
export class AdminProductsComponent implements OnInit {
  private readonly productsApi = inject(ProductApiService);
  private readonly adminApi = inject(AdminApiService);
  private readonly confirmDialog = inject(ConfirmDialogService);

  products: Product[] = [];
  categories: CategoryOption[] = [];
  editId: number | null = null;
  draft: Partial<Product> = {};
  stockInput: Record<number, number> = {};
  newProduct = {
    name: '',
    description: '',
    price: 0,
    stockQuantity: 0,
    categoryId: null as number | null,
    imageUrl: ''
  };
  msg = '';
  err = '';

  ngOnInit(): void {
    this.adminApi.getCategories().subscribe({
      next: (c) => (this.categories = c),
      error: () => {}
    });
    this.reload();
  }

  async createProduct(): Promise<void> {
    if (!this.newProduct.name.trim()) return;
    const confirmed = await this.confirmDialog.open({
      title: 'Create product',
      message: `Create product "${this.newProduct.name.trim()}"?`,
      confirmLabel: 'Create'
    });
    if (!confirmed) return;
    this.err = '';
    this.adminApi
      .createProduct({
        name: this.newProduct.name.trim(),
        description: this.newProduct.description?.trim() ?? '',
        price: Number(this.newProduct.price),
        stockQuantity: Number(this.newProduct.stockQuantity),
        categoryId: this.newProduct.categoryId ?? undefined,
        imageUrl: this.newProduct.imageUrl.trim() || undefined
      })
      .subscribe({
        next: () => {
          this.msg = 'Product created.';
          this.newProduct = {
            name: '',
            description: '',
            price: 0,
            stockQuantity: 0,
            categoryId: null,
            imageUrl: ''
          };
          this.reload();
        },
        error: () => (this.err = 'Create failed (validation or not authorized).')
      });
  }

  reload(): void {
    this.err = '';
    this.productsApi.getAll().subscribe({
      next: (list) => {
        this.products = list;
        for (const p of list) {
          this.stockInput[p.id] = p.stockQuantity;
        }
      },
      error: () => (this.err = 'Failed to load products.')
    });
  }

  startEdit(p: Product): void {
    this.editId = p.id;
    this.draft = { ...p };
    this.msg = '';
    this.err = '';
  }

  cancelEdit(): void {
    this.editId = null;
    this.draft = {};
  }

  async save(): Promise<void> {
    if (this.editId === null || !this.draft.name) return;
    const confirmed = await this.confirmDialog.open({
      title: 'Update product',
      message: `Save changes to "${this.draft.name}"?`,
      confirmLabel: 'Save'
    });
    if (!confirmed) return;
    this.adminApi
      .updateProduct(this.editId, {
        name: this.draft.name!,
        description: this.draft.description ?? '',
        price: Number(this.draft.price),
        stockQuantity: Number(this.draft.stockQuantity),
        categoryId: this.draft.categoryId ?? undefined,
        imageUrl: this.draft.imageUrl?.trim() || undefined
      })
      .subscribe({
        next: () => {
          this.msg = 'Product updated.';
          this.cancelEdit();
          this.reload();
        },
        error: () => (this.err = 'Update failed (validation or conflict).')
      });
  }

  async delete(p: Product): Promise<void> {
    const confirmed = await this.confirmDialog.open({
      title: 'Delete product',
      message: `Delete "${p.name}"? This cannot be undone.`,
      confirmLabel: 'Delete',
      tone: 'danger'
    });
    if (!confirmed) return;
    this.adminApi.deleteProduct(p.id).subscribe({
      next: () => {
        this.msg = 'Product deleted.';
        this.reload();
      },
      error: () =>
        (this.err = 'Delete not allowed if the product appears on past orders, or server error.')
    });
  }

  async applyStock(p: Product): Promise<void> {
    const qty = this.stockInput[p.id];
    if (qty === undefined || qty < 0) return;
    const confirmed = await this.confirmDialog.open({
      title: 'Update stock',
      message: `Set stock for "${p.name}" to ${qty}?`,
      confirmLabel: 'Update stock'
    });
    if (!confirmed) return;
    this.adminApi.updateStock(p.id, qty).subscribe({
      next: () => {
        this.msg = `Stock updated for ${p.name}.`;
        this.reload();
      },
      error: () => (this.err = 'Stock update failed.')
    });
  }

  categoryLabel(p: Product): string {
    return p.categoryName ?? (p.categoryId ? `#${p.categoryId}` : '—');
  }
}
