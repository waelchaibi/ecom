import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AdminApiService } from '../../../core/services/admin-api.service';
import { ConfirmDialogService } from '../../../core/services/confirm-dialog.service';

@Component({
  selector: 'app-admin-categories',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-categories.component.html',
  styleUrl: './admin-categories.component.scss'
})
export class AdminCategoriesComponent implements OnInit {
  private readonly api = inject(AdminApiService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  categories: { id: number; name: string; description?: string | null }[] = [];
  name = '';
  description = '';
  err = '';

  ngOnInit(): void {
    this.reload();
  }

  reload(): void {
    this.api.getCategories().subscribe({
      next: (c) => (this.categories = c),
      error: () => (this.err = 'Failed to load categories')
    });
  }

  async create(): Promise<void> {
    this.err = '';
    if (!this.name.trim()) {
      this.err = 'Name is required';
      return;
    }
    const confirmed = await this.confirmDialog.open({
      title: 'Create category',
      message: `Create category "${this.name.trim()}"?`,
      confirmLabel: 'Create'
    });
    if (!confirmed) return;
    this.api.createCategory({ name: this.name.trim(), description: this.description.trim() || undefined }).subscribe({
      next: () => {
        this.name = '';
        this.description = '';
        this.reload();
      },
      error: (e) => (this.err = e.error?.error ?? e.message ?? 'Create failed')
    });
  }

  async remove(id: number): Promise<void> {
    const category = this.categories.find((c) => c.id === id);
    const confirmed = await this.confirmDialog.open({
      title: 'Delete category',
      message: category ? `Delete category "${category.name}"?` : 'Delete this category?',
      confirmLabel: 'Delete',
      tone: 'danger'
    });
    if (!confirmed) return;
    this.api.deleteCategory(id).subscribe({
      next: () => this.reload(),
      error: (e) => (this.err = e.error?.error ?? e.message ?? 'Delete failed')
    });
  }
}
