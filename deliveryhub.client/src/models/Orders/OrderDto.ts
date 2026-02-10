import type { UUIDTypes } from "uuid";
import type { BaseEntity } from "../BaseEntity";
import type { OrderStatusType } from "../../enums/orders/OrderStatus";

export interface OrderDto extends BaseEntity {
    orderNumber : UUIDTypes;
    status: OrderStatusType;
    address: string;
    createdDate: Date;
    deliveryDate: Date;
}