import axios from "axios";
import { CATALOG_BASE_URL, ORDER_BASE_URL } from "../constants/EndpointConstants";

export const order_api = axios.create({
    baseURL: ORDER_BASE_URL,
});

export const catalog_api = axios.create({
    baseURL: CATALOG_BASE_URL,
});
