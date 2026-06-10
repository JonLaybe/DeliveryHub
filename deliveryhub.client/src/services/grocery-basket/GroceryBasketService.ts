import type { UUIDTypes } from "uuid";
import type { ProductDto } from "../../models/catalog-service/ProductDto";
import type { GroceryBasketItem } from "../../models/grocery-basket/GroceryBasket";
import { mapProductToGroceryBasketItem } from "../../pipe/GroceryBasketPipe";
import type { Payment } from "../../models/grocery-basket/Payment";
import { toast } from "react-hot-toast";

const nameGroceryBasket = "groceryBasketProducts";
const namePaymentData = "paymentData";

export function getGroceryBasket(): GroceryBasketItem[] {
    let productsJson = localStorage.getItem(nameGroceryBasket);

    if (!productsJson)
        return [];

    return JSON.parse(productsJson);
}

export function isProductInGroceryBasket(productId: UUIDTypes): boolean {
    return getGroceryBasket().some(x => x.product.id === productId);
}

export function getItemGroceryBasketCount() {
    return getGroceryBasket().reduce((total, item) => total + item.quantity, 0);
}

export function getItemGroceryBasket(productId: UUIDTypes): GroceryBasketItem | undefined {
    return getGroceryBasket().find(x => x.product.id === productId);
}

export function addGroceryBasket(product: ProductDto): void {
    if (product.availableQty <= 0)
        return;

    const productsJson = localStorage.getItem(nameGroceryBasket);
    let groceryBasket: GroceryBasketItem[] = [];

    if (productsJson) {
        let prod = JSON.parse(productsJson);
        groceryBasket = Array.isArray(prod) ? JSON.parse(productsJson) : [];
    }

    if (groceryBasket.length === 0) {
        groceryBasket.push(mapProductToGroceryBasketItem(product));
        localStorage.setItem(nameGroceryBasket, JSON.stringify(groceryBasket));
        window.dispatchEvent(new Event('basketStorageChanged'));
        toast.success(`Товар '${product.name}' добавлен в корзину`);

        return;
    }

    const indexSame = groceryBasket.findIndex(item => item.product.id === product.id)

    if (indexSame !== -1) {
        groceryBasket[indexSame].quantity++;
        groceryBasket[indexSame].price = groceryBasket[indexSame].product.price * groceryBasket[indexSame].quantity;
    }
    else {
        groceryBasket.push(mapProductToGroceryBasketItem(product));
        toast.success(`Товар '${product.name}' добавлен в корзину`);
    }

    localStorage.setItem(nameGroceryBasket, JSON.stringify(groceryBasket));
    window.dispatchEvent(new Event('basketStorageChanged'));
}

export function decreaseGroceryBasket(productId: UUIDTypes): boolean {
    let groceryBasket = getGroceryBasket();

    let basketItem = groceryBasket.find(item => item.product.id === productId);

    if (!basketItem) return false;

    let refGroceryBasket;

    if (basketItem.quantity === 1) {
        refGroceryBasket = groceryBasket.filter(item => item.product.id !== productId);
        toast.success(`Товар '${basketItem.product.name}' удален из корзины`);
    }
    else if (basketItem.quantity > 1) {
        refGroceryBasket = groceryBasket.map(item => {
            if (item.product.id === productId && item.quantity > 1) {
                return { ...item, quantity: item.quantity - 1, price: item.product.price * (item.quantity - 1) };
            }
            return item;
        });
    }

    if (refGroceryBasket) {
        refreshGroceryBasket(refGroceryBasket);
        return true;
    }

    return false;
}

export function refreshGroceryBasket(groceryBasketItem: GroceryBasketItem[]): void {
    localStorage.setItem(nameGroceryBasket, JSON.stringify(groceryBasketItem));
    window.dispatchEvent(new Event('basketStorageChanged'));
}

export function resetGroceryBasket(): void {
    localStorage.removeItem(nameGroceryBasket);
    window.dispatchEvent(new Event('basketStorageChanged'));
}

export function popPaymentData(): Payment | undefined {
    let paymentItem = localStorage.getItem(namePaymentData);

    if (!paymentItem)
        return undefined;

    let paymentData = JSON.parse(paymentItem);
    localStorage.removeItem(namePaymentData);

    return paymentData;
}

export function setPaymentData(payment: Payment) {
    localStorage.setItem(namePaymentData, JSON.stringify(payment));
}