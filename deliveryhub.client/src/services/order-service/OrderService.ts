import { ORDER_URL } from "../../constants/EndpointConstants";
import { api } from "../../http";
import type { OrderDto } from "../../models/order-service/OrderDto";

const prefix = 'https://localhost:7225/';

export async function getListOrdersAsync(): Promise<OrderDto[]> {
    const res = await api.get(`${prefix}${ORDER_URL}/getorders`);

    return res.data;
}