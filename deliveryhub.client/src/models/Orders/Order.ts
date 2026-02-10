import type { UUIDTypes } from "uuid";
import type { BaseEntity } from "../BaseEntity";
import type { OrderStatusType } from "../../enums/orders/OrderStatus";

export interface Order extends BaseEntity {
    orderNumber : UUIDTypes;
    status: OrderStatusType;
    address: string;
    createdDate: Date;
    deliveryDate: Date;
}