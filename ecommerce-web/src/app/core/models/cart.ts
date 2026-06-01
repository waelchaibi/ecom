export interface CartLine {
  productId: number;
  productName: string;
  imageUrl?: string | null;
  unitPrice: number;
  quantity: number;
  lineTotal: number;
  stockQuantity: number;
}

export interface Cart {
  items: CartLine[];
  subtotal: number;
}

export interface CartCheckoutPayload {
  taxAmount: number;
  shippingAmount: number;
  promotionCode?: string;
}
