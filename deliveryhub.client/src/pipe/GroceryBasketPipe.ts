import type { ProductDto } from "../models/catalog-service/ProductDto";
import type { GroceryBasketItem } from "../models/grocery-basket/GroceryBasket";
import type { ProductCreateDto } from "../models/order-service/ProductCreateDto";

export function mapProductToGroceryBasketItem(product: ProductDto, quantity = 1): GroceryBasketItem {
    return {
        quantity: quantity,
        price: product.price,
        product: product,
    };
}

export function mapGroceryBasketItemsToProduct(basket: GroceryBasketItem[]): ProductCreateDto[] {
    if (!basket)
        return [];

    return basket.map(gb => ({
        articleNumber: gb.product.id,
        quantity: gb.quantity,
        price: gb.price,
        discount: gb.product.discount
    }) as ProductCreateDto);
}