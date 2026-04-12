export interface ProductSearchQueryRequest {
    text: string;
    minPrice?: number;
    maxPrice?: number;
    attributes?: Record<string, string[]>;
}