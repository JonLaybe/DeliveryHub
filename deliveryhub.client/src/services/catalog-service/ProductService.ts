import { CATALOGSERVICE_PRODUCT_URL } from "../../constants/EndpointConstants";
import api from "../../http";
import type { ProductDto } from "../../models/catalog-service/ProductDto";

const prefix = 'http://localhost:5000'

export async function getListProductsAsync(): Promise<ProductDto[]> {
    const res = await api.get(`${prefix}/${CATALOGSERVICE_PRODUCT_URL}`);

    return res.data;
}