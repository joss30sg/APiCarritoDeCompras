export interface Product {
  id: string;
  name: string;
  description: string;
  price: number;
  quantity: number;
  imageUrl?: string;
}

export interface CartItem {
  product: Product;
  quantity: number;
}
