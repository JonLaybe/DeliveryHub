import type { UUIDTypes } from "uuid";

export interface ProductSearchQueryRequest {
    text?: string;
    minPrice?: number;
    maxPrice?: number;
    attributes?: Record<string, string[]>;
    sort?: string;
    categoryId?: UUIDTypes;
}