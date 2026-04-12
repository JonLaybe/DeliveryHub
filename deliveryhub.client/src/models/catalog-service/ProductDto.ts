import type { UUIDTypes } from "uuid";
import type { BaseEntityIdentityGuid } from "../BaseEntityIdentityGuid";
import type { ProductImageDto } from "./ProductImageDto";

export interface ProductDto extends BaseEntityIdentityGuid {
    name: string;
    description: string;
    price: number;
    availableQty: number;
    categoryId: UUIDTypes;
    images: ProductImageDto[] | undefined;
    attributes: Record<string, string>;
}

export interface ProductSearchResultDto {
    products: ProductDto[];
    attributes: Record<string, string[]>;
}