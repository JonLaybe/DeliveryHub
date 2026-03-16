import { ORDER_URL } from "../../constants/EndpointConstants";
import { order_api } from "../../http";
import type { OrderDto } from "../../models/order-service/OrderDto";

export async function getListOrdersAsync(): Promise<OrderDto[]> {
    const res = await order_api.get(`${ORDER_URL}/getorders`);

    return res.data;
}