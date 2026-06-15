import type { ProductCreateDto } from "./ProductCreateDto";

export interface OrderCreateDto {
    address: string;
    deliveryDate: Date;
    discount?: number | null;
    discountUsageId?: number | null;
    products: ProductCreateDto[];
}