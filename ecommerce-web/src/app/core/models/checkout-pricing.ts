export interface CheckoutPricing {
  subtotal: number;
  taxAmount: number;
  shippingAmount: number;
  totalAmount: number;
  taxRate: number;
  freeShippingApplied: boolean;
}
