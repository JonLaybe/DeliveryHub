import type { ProductDto } from "../../models/catalog-service/ProductDto";
import type { GroceryBasketItem } from "../../models/grocery-basket/GroceryBasket";
import { mapProductToGroceryBasketItem } from "../../pipe/GroceryBasketPipe";

const nameGroceryBasket = "groceryBasketProducts";

export function getGroceryBasket(): GroceryBasketItem[] {
    let productsJson = localStorage.getItem(nameGroceryBasket);

    if (!productsJson)
        return [];

    return JSON.parse(productsJson);
}

export function addGroceryBasket(product: ProductDto): void {
    const productsJson = localStorage.getItem(nameGroceryBasket);
    let groceryBasket: GroceryBasketItem[] = [];

    if (productsJson) {
        let prod = JSON.parse(productsJson);
        groceryBasket = Array.isArray(prod) ? JSON.parse(productsJson) : [];
    }

    if (groceryBasket.length === 0) {
        groceryBasket.push(mapProductToGroceryBasketItem(product));
        localStorage.setItem(nameGroceryBasket, JSON.stringify(groceryBasket));

        return;
    }

    const indexSame = groceryBasket.findIndex(item => item.product.id === product.id)

    if (indexSame !== -1) {
        groceryBasket[indexSame].quantity++;
        groceryBasket[indexSame].price = groceryBasket[indexSame].product.price * groceryBasket[indexSame].quantity;
    }
    else {
        groceryBasket.push(mapProductToGroceryBasketItem(product));
    }

    localStorage.setItem(nameGroceryBasket, JSON.stringify(groceryBasket));
}

export function refreshGroceryBasket(groceryBasketItem: GroceryBasketItem[]): void {
    localStorage.setItem(nameGroceryBasket, JSON.stringify(groceryBasketItem));
}

export function resetGroceryBasket(): void {
    localStorage.removeItem(nameGroceryBasket);
}