import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { Product } from '../../../core/models/product';
import { AdminApiService } from '../../../core/services/admin-api.service';
import { ConfirmDialogService } from '../../../core/services/confirm-dialog.service';
import { resolveMediaUrl } from '../../../core/utils/media-url';
import { MaterialModule } from '../../../shared/material.module';

interface CategoryOption {
  id: number;
  name: string;
}

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [CommonModule, FormsModule, MaterialModule],
  templateUrl: './admin-products.component.html',
  styleUrl: './admin-products.component.scss'
})
export class AdminProductsComponent implements OnInit {
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
  newImageFile: File | null = null;
  newImagePreview: string | null = null;
  editImageFile: File | null = null;
  editImagePreview: string | null = null;
  uploading = false;
  msg = '';
  err = '';

  ngOnInit(): void {
    this.adminApi.getCategories().subscribe({
      next: (c) => (this.categories = c),
      error: () => {}
    });
    this.reload();
  }

  mediaSrc(url: string | null | undefined): string | null {
    return resolveMediaUrl(url);
  }

  onNewImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    this.newImageFile = file;
    this.revokePreview(this.newImagePreview);
    this.newImagePreview = file ? URL.createObjectURL(file) : null;
    if (!file) {
      this.newProduct.imageUrl = '';
    }
  }

  clearNewImage(input?: HTMLInputElement): void {
    this.newImageFile = null;
    this.revokePreview(this.newImagePreview);
    this.newImagePreview = null;
    this.newProduct.imageUrl = '';
    if (input) input.value = '';
  }

  onEditImageSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    this.editImageFile = file;
    this.revokePreview(this.editImagePreview);
    this.editImagePreview = file ? URL.createObjectURL(file) : null;
  }

  clearEditImage(input?: HTMLInputElement): void {
    this.editImageFile = null;
    this.revokePreview(this.editImagePreview);
    this.editImagePreview = null;
    this.draft.imageUrl = '';
    if (input) input.value = '';
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
    this.uploading = true;
    try {
      let imageUrl = this.newProduct.imageUrl.trim() || undefined;
      if (this.newImageFile) {
        const uploaded = await firstValueFrom(this.adminApi.uploadProductImage(this.newImageFile));
        imageUrl = uploaded.imageUrl;
      }
      await firstValueFrom(
        this.adminApi.createProduct({
          name: this.newProduct.name.trim(),
          description: this.newProduct.description?.trim() ?? '',
          price: Number(this.newProduct.price),
          stockQuantity: Number(this.newProduct.stockQuantity),
          categoryId: this.newProduct.categoryId ?? undefined,
          imageUrl
        })
      );
      this.msg = 'Product created.';
      this.newProduct = {
        name: '',
        description: '',
        price: 0,
        stockQuantity: 0,
        categoryId: null,
        imageUrl: ''
      };
      this.clearNewImage();
      this.reload();
    } catch {
      this.err = 'Create failed (image upload, validation, or not authorized).';
    } finally {
      this.uploading = false;
    }
  }

  reload(): void {
    this.err = '';
    this.adminApi.getAdminProducts().subscribe({
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
    this.editImageFile = null;
    this.revokePreview(this.editImagePreview);
    this.editImagePreview = null;
    this.msg = '';
    this.err = '';
  }

  cancelEdit(): void {
    this.editId = null;
    this.draft = {};
    this.editImageFile = null;
    this.revokePreview(this.editImagePreview);
    this.editImagePreview = null;
  }

  async save(): Promise<void> {
    if (this.editId === null || !this.draft.name) return;
    const confirmed = await this.confirmDialog.open({
      title: 'Update product',
      message: `Save changes to "${this.draft.name}"?`,
      confirmLabel: 'Save'
    });
    if (!confirmed) return;
    this.uploading = true;
    try {
      let imageUrl = this.draft.imageUrl?.trim() || undefined;
      if (this.editImageFile) {
        const uploaded = await firstValueFrom(this.adminApi.uploadProductImage(this.editImageFile));
        imageUrl = uploaded.imageUrl;
      }
      await firstValueFrom(
        this.adminApi.updateProduct(this.editId, {
          name: this.draft.name!,
          description: this.draft.description ?? '',
          price: Number(this.draft.price),
          stockQuantity: Number(this.draft.stockQuantity),
          categoryId: this.draft.categoryId ?? undefined,
          imageUrl
        })
      );
      this.msg = 'Product updated.';
      this.cancelEdit();
      this.reload();
    } catch {
      this.err = 'Update failed (image upload, validation, or conflict).';
    } finally {
      this.uploading = false;
    }
  }

  async delete(p: Product): Promise<void> {
    const confirmed = await this.confirmDialog.open({
      title: 'Archive product',
      message: `Archive "${p.name}"? It will be hidden from the shop but kept for order history. You can restore it later.`,
      confirmLabel: 'Archive',
      tone: 'danger'
    });
    if (!confirmed) return;
    this.adminApi.deleteProduct(p.id).subscribe({
      next: () => {
        this.msg = 'Product archived.';
        this.reload();
      },
      error: () => (this.err = 'Archive failed.')
    });
  }

  async restore(p: Product): Promise<void> {
    const confirmed = await this.confirmDialog.open({
      title: 'Restore product',
      message: `Restore "${p.name}" to the shop?`,
      confirmLabel: 'Restore'
    });
    if (!confirmed) return;
    this.adminApi.restoreProduct(p.id).subscribe({
      next: () => {
        this.msg = 'Product restored.';
        this.reload();
      },
      error: () => (this.err = 'Restore failed.')
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

  private revokePreview(url: string | null): void {
    if (url?.startsWith('blob:')) {
      URL.revokeObjectURL(url);
    }
  }
}
