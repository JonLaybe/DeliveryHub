import type { UUIDTypes } from "uuid";
export interface ApplyModel {
    code: string;
    orderId?: UUIDTypes;
    userId?: number;
    orderAmount: number;
}