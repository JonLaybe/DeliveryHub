import { ORDER_URL } from "../../constants/EndpointConstants";
import { api_authorized } from "../../http";
import type { OrderCreateDto } from "../../models/order-service/OrderCreateDto";
import type { OrderDto } from "../../models/order-service/OrderDto";

// const prefix = 'https://localhost:7225/';

export async function getListOrdersAsync(): Promise<OrderDto[]> {
    const res = await api_authorized.get(`${ORDER_URL}/getorders`);

    return res.data;
}

export async function createOrderAsync(order: OrderCreateDto): Promise<OrderDto> {
    const res = await api_authorized.post(`${ORDER_URL}/create`, order);

    return res.data;
}