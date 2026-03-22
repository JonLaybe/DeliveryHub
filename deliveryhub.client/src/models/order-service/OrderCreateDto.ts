import type { ProductCreateDto } from "./ProductCreateDto";

export interface OrderCreateDto {
    address: string;
    deliveryDate: Date;
    products: ProductCreateDto[];
}