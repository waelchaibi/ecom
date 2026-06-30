export type ConfirmDialogTone = 'default' | 'danger';

export interface ConfirmDialogConfig {
  title: string;
  message: string;
  confirmLabel?: string;
  cancelLabel?: string;
  tone?: ConfirmDialogTone;
}

export interface ConfirmDialogState extends Required<Pick<ConfirmDialogConfig, 'confirmLabel' | 'cancelLabel' | 'tone'>> {
  title: string;
  message: string;
}
