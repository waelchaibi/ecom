export interface OrderItem {
  id: number;
  productId: number;
  quantity: number;
  price: number;
}

export interface Order {
  id: number;
  customerId: number;
  totalAmount: number;
  createdAt: string;
  status: string;
  orderItems: OrderItem[];
}

export interface CreateOrderPayload {
  customerId: number;
  items: { productId: number; quantity: number }[];
}
