import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AdminApiService, GiftRuleRow } from '../../../core/services/admin-api.service';

const RULE_LABELS: Record<number, string> = {
  0: 'Amount (min order total)',
  1: 'Loyalty (min prior orders)',
  2: 'Promotion (code)'
};

@Component({
  selector: 'app-admin-gift-rules',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-gift-rules.component.html',
  styleUrl: './admin-gift-rules.component.scss'
})
export class AdminGiftRulesComponent implements OnInit {
  private readonly api = inject(AdminApiService);

  rules: GiftRuleRow[] = [];
  gifts: { id: number; name: string; description: string; stockQuantity: number }[] = [];

  editId: number | null = null;
  draft: Partial<GiftRuleRow> = {};

  newRule = {
    ruleType: 0,
    conditionValue: '',
    giftId: 0,
    isActive: true,
    priority: 0
  };

  newGift = { name: '', description: '', stockQuantity: 0 };

  msg = '';
  err = '';

  readonly ruleLabels = RULE_LABELS;

  ngOnInit(): void {
    this.reloadAll();
  }

  ruleLabel(t: number): string {
    return this.ruleLabels[t] ?? String(t);
  }

  reloadAll(): void {
    this.err = '';
    this.api.getGiftRules().subscribe({
      next: (r) => (this.rules = r),
      error: () => (this.err = 'Failed to load rules.')
    });
    this.api.getGifts().subscribe({
      next: (g) => (this.gifts = g),
      error: () => {}
    });
  }

  startEdit(r: GiftRuleRow): void {
    this.editId = r.id;
    this.draft = { ...r };
    this.msg = '';
    this.err = '';
  }

  cancelEdit(): void {
    this.editId = null;
    this.draft = {};
  }

  saveRule(): void {
    if (this.editId === null) {
      return;
    }
    this.api
      .updateGiftRule(this.editId, {
        ruleType: this.draft.ruleType,
        conditionValue: this.draft.conditionValue,
        giftId: this.draft.giftId,
        isActive: this.draft.isActive,
        priority: this.draft.priority
      })
      .subscribe({
        next: () => {
          this.msg = 'Rule updated.';
          this.cancelEdit();
          this.reloadAll();
        },
        error: () => (this.err = 'Update failed (conflict, validation, or missing gift).')
      });
  }

  toggleActive(r: GiftRuleRow): void {
    this.api.setGiftRuleActive(r.id, !r.isActive).subscribe({
      next: () => {
        this.msg = 'Rule status updated.';
        this.reloadAll();
      },
      error: () => (this.err = 'Could not change active state (e.g. conflicting active rule).')
    });
  }

  deleteRule(r: GiftRuleRow): void {
    if (!confirm(`Delete rule #${r.id}?`)) {
      return;
    }
    this.api.deleteGiftRule(r.id).subscribe({
      next: () => {
        this.msg = 'Rule deleted.';
        this.reloadAll();
      },
      error: () => (this.err = 'Delete blocked if rule was used on orders.')
    });
  }

  createRule(): void {
    if (!this.newRule.giftId) {
      this.err = 'Select a gift.';
      return;
    }
    this.api.createGiftRule(this.newRule).subscribe({
      next: () => {
        this.msg = 'Rule created.';
        this.newRule = { ruleType: 0, conditionValue: '', giftId: 0, isActive: true, priority: 0 };
        this.reloadAll();
      },
      error: () => (this.err = 'Create failed (duplicate active rule or invalid data).')
    });
  }

  createGift(): void {
    if (!this.newGift.name.trim()) {
      return;
    }
    this.api.createGift(this.newGift).subscribe({
      next: () => {
        this.msg = 'Gift created.';
        this.newGift = { name: '', description: '', stockQuantity: 0 };
        this.reloadAll();
      },
      error: () => (this.err = 'Gift create failed.')
    });
  }
}
