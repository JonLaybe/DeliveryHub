import { CATALOGSERVICE_PRODUCT_URL } from "../../constants/EndpointConstants";
import { catalog_api } from "../../http";
import type { ProductDto } from "../../models/catalog-service/ProductDto";


export async function getListProductsAsync(): Promise<ProductDto[]> {
    const res = await catalog_api.get(`${CATALOGSERVICE_PRODUCT_URL}`);

    return res.data;
}