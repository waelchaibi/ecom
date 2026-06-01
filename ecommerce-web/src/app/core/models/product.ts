export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  stockQuantity: number;
  categoryId?: number | null;
  categoryName?: string | null;
  imageUrl?: string | null;
}

export interface CreateProductPayload {
  name: string;
  description: string;
  price: number;
  stockQuantity: number;
  categoryId?: number | null;
  imageUrl?: string | null;
}
