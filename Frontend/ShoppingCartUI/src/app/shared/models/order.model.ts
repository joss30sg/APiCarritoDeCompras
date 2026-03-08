export interface OrderItem {
  id: string;
  name: string;
  price: number;
  quantity: number;
  imageUrl: string;
  subtotal: number;
}

export interface Order {
  id: string;
  orderNumber: string;
  userId: string;
  items: OrderItem[];
  subtotal: number;
  tax: number;
  total: number;
  status: 'pending' | 'completed' | 'cancelled';
  createdAt: Date;
  orderDate: string;
}
