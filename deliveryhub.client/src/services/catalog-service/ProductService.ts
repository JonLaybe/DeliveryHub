import type { UUIDTypes } from "uuid";
import { CATALOGSERVICE_PRODUCT_URL } from "../../constants/EndpointConstants";
import { catalog_api } from "../../http";
import type { ProductDto } from "../../models/catalog-service/ProductDto";


export async function getListProductsAsync(): Promise<ProductDto[]> {
    const res = await catalog_api.get(`${CATALOGSERVICE_PRODUCT_URL}`);

    return res.data;
}

export async function getListProductsByIdsAsync(productIds: UUIDTypes[]): Promise<ProductDto[]> {
    const params = new URLSearchParams();
    productIds.forEach(id => params.append('idList', id.toString()));

    const res = await catalog_api.get(`${CATALOGSERVICE_PRODUCT_URL}/list?${params.toString()}`);

    return res.data;
}

export async function getProductByIdAsync(id: UUIDTypes): Promise<ProductDto> {
    const res = await catalog_api.get(`${CATALOGSERVICE_PRODUCT_URL}/${id}`);
    return res.data;
}

export async function searchProductsAsync(text: string): Promise<ProductDto[]> {
    const res = await catalog_api.get(`${CATALOGSERVICE_PRODUCT_URL}/search?text=${text}`);
    return res.data;
}

export async function suggestProductsAsync(text: string): Promise<string[]> {
    const res = await catalog_api.get(`${CATALOGSERVICE_PRODUCT_URL}/suggest?query=${text}`);
    return res.data;
}