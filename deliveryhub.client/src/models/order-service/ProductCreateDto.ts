import type { UUIDTypes } from "uuid";

export interface ProductCreateDto {
    articleNumber: UUIDTypes;
    quantity: number;
    price: number;
}