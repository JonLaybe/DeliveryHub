import type { BaseEntity } from "../BaseEntity";
import type { OrderStatusType } from "../../enums/orders/OrderStatus";
import type { ProductDto } from "./ProductDto";

export interface OrderDto extends BaseEntity {
    status: OrderStatusType;
    address: string;
    createdDate: Date;
    deliveryDate: Date;
    products: ProductDto[];
}