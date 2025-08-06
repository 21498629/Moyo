export interface Product {
  id: number;
  name: string;
  description: string;
  price: string;
  image: string;
  quantity: number;
  isActive: boolean;
  categoryId: number;
  venderId: number;
}