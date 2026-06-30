import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SimulatePaymentPayload } from '../../models/payment';
import { ConfirmDialogService } from '../../services/confirm-dialog.service';

@Component({
  selector: 'ecom-payment-gateway',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './payment-gateway.component.html',
  styleUrl: './payment-gateway.component.scss'
})
export class PaymentGatewayComponent {
  private readonly confirmDialog = inject(ConfirmDialogService);

  @Input() amount = 0;
  @Input() busy = false;
  @Input() submitLabel = 'Pay now';
  @Output() pay = new EventEmitter<SimulatePaymentPayload>();

  cardholderName = '';
  cardNumber = '';
  expiryMonth = '';
  expiryYear = '';
  cvv = '';
  localError: string | null = null;

  async submit(): Promise<void> {
    this.localError = null;
    const payload: SimulatePaymentPayload = {
      cardholderName: this.cardholderName.trim(),
      cardNumber: this.cardNumber.trim(),
      expiryMonth: this.expiryMonth.trim(),
      expiryYear: this.expiryYear.trim(),
      cvv: this.cvv.trim()
    };

    if (!payload.cardholderName) {
      this.localError = 'Cardholder name is required.';
      return;
    }
    if (!payload.cardNumber.replace(/\D/g, '').length) {
      this.localError = 'Card number is required.';
      return;
    }
    if (!payload.expiryMonth || !payload.expiryYear) {
      this.localError = 'Expiry date is required.';
      return;
    }
    if (!payload.cvv) {
      this.localError = 'CVV is required.';
      return;
    }

    const confirmed = await this.confirmDialog.open({
      title: 'Confirm payment',
      message: `Pay ${this.amount.toFixed(2)} now? This will confirm your order.`,
      confirmLabel: 'Pay now'
    });
    if (!confirmed) {
      return;
    }

    this.pay.emit(payload);
  }
}
