import type { ProductDto } from "../catalog-service/ProductDto";

export interface GroceryBasketItem {
    quantity: number;
    price: number;
    product: ProductDto;
}