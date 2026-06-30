export interface OrderItem {
  id: number;
  productId: number;
  productName?: string;
  quantity: number;
  price: number;
}

export interface OrderGiftSummary {
  giftId: number;
  giftName: string;
  quantity: number;
  giftRuleId: number;
}

export interface Order {
  id: number;
  customerId: number;
  subtotalAmount?: number;
  taxAmount?: number;
  shippingAmount?: number;
  totalAmount: number;
  createdAt: string;
  status: string;
  promotionCode?: string | null;
  orderItems: OrderItem[];
  assignedGifts?: OrderGiftSummary[];
}

export interface CreateOrderPayload {
  items: { productId: number; quantity: number }[];
  promotionCode?: string;
}
