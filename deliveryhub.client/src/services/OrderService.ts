import { ORDER_URL } from "../constants/EndpointConstants";
import api from "../http";
import type { OrderDto } from "../models/Orders/OrderDto";

export async function getListOrdersAsync(): Promise<OrderDto[]> {
    const res = await api.get(`${ORDER_URL}/getorders`);

    return res.data;
}