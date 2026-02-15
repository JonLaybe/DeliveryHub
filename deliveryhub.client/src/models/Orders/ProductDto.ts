import type { UUIDTypes } from "uuid";
import type { BaseEntity } from "../BaseEntity";

export interface ProductDto extends BaseEntity {
    articleNumber: UUIDTypes,
    quantity: Number,
    name: string,
    price: Number,
    photoPreviewUrl: string,
}