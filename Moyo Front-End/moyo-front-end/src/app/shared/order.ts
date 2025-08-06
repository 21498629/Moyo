import { OrderItem } from "./order-item";  

export interface Order {
    id: number;
    orderNumber: string;
    createdAt: Date;
    orderItems: OrderItem[];
}