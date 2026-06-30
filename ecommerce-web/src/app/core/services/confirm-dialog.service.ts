import { Injectable, signal } from '@angular/core';
import { ConfirmDialogConfig, ConfirmDialogState } from '../models/confirm-dialog.model';

@Injectable({ providedIn: 'root' })
export class ConfirmDialogService {
  readonly dialog = signal<ConfirmDialogState | null>(null);

  private resolver: ((confirmed: boolean) => void) | null = null;

  open(config: ConfirmDialogConfig): Promise<boolean> {
    return new Promise((resolve) => {
      this.closePending(false);
      this.resolver = resolve;
      this.dialog.set({
        title: config.title,
        message: config.message,
        confirmLabel: config.confirmLabel ?? 'Confirm',
        cancelLabel: config.cancelLabel ?? 'Cancel',
        tone: config.tone ?? 'default'
      });
    });
  }

  accept(): void {
    this.closePending(true);
  }

  dismiss(): void {
    this.closePending(false);
  }

  private closePending(result: boolean): void {
    this.dialog.set(null);
    this.resolver?.(result);
    this.resolver = null;
  }
}
