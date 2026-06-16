import type { UUIDTypes } from "uuid";
import type { OrderStatusType } from "../enums/orders/OrderStatus";
import type { ProductDto } from "../models/catalog-service/ProductDto";
import type { ProductImageDto } from "../models/catalog-service/ProductImageDto";
import type { OrderDto } from "../models/order-service/OrderDto";

export interface OrderForList {
    id: number,
    status: OrderStatusType;
    address: string;
    createdDate: Date;
    deliveryDate: Date;
    discount?: number | null;
    products: ProductForOrderList[];
}

export interface ProductForOrderList {
    id: number;
    articleNumber: UUIDTypes;
    price: number;
    quantity: number;
    name: string;
    description: string;
    image: ProductImageDto | undefined;
}

export function flatMapOrderAndProduct(orders: OrderDto[], products: ProductDto[]): OrderForList[] {
    let ordPrds = orders.map(ord => {
        let order = {
            id: ord.id,
            status: ord.status,
            address: ord.address,
            createdDate: ord.createdDate,
            deliveryDate: ord.deliveryDate,
            discount: ord.discount,
            products: [],
        } as OrderForList;

        ord.products.map(prd => {
            let fullProduct = products.find(p => p.id === prd.articleNumber);

            order.products.push({
                id: prd.id,
                articleNumber: prd.articleNumber,
                price: prd.price,
                quantity: prd.quantity,
                name: fullProduct && fullProduct.name ? fullProduct.name : '',
                description: fullProduct && fullProduct.description ? fullProduct.description : '',
                image: fullProduct?.images && fullProduct.images.length > 0 ? fullProduct.images[0] : undefined,
            })
        });

        return order;
    })

    return ordPrds;
}