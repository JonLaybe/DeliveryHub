import { createContext, useEffect, useState } from "react";
import { searchProductsAsync, suggestProductsAsync } from "../services/catalog-service/ProductService";
import { useDebounce } from "../hooks/useDebounce";
import type { OrderDto } from "../models/order-service/OrderDto";
import type { ProductDto } from "../models/catalog-service/ProductDto";

interface SearchContextType {
    products: { products: ProductDto[] } | undefined
    setProducts: (value: { products: ProductDto[] } | undefined) => void
    orders: { orders: OrderDto[] } | undefined
    setOrders: (value: { orders: OrderDto[] } | undefined) => void
    query: string
    suggestedProducts: string[]
    searchBoxChangeHandler: (event: React.ChangeEvent<HTMLInputElement>) => void,
    searchProductHandler: (event: React.MouseEvent<HTMLInputElement>) => void,
}

export const SearchContext = createContext<SearchContextType>({
    products: undefined,
    setProducts: () => {},
    orders: undefined,
    setOrders: () => {},
    query: '',
    suggestedProducts: [],
    searchBoxChangeHandler: () => {},
    searchProductHandler: () => {}
});

export const SearchProvider = (props:any) => {
    const { children } = props;
    const [orders, setOrders] = useState<{ orders: OrderDto[] }>();
    const [products, setProducts] = useState<{ products: ProductDto[] }>();
    const [query, setQuery] = useState<string>('');
    const [suggestedProducts, setSuggestedProducts] = useState<string[]>([]);

    const searchBoxChangeHandler = (event: React.ChangeEvent<HTMLInputElement>) => {
        setQuery(event.target.value);
    }

    const searchProductHandler = (event: React.MouseEvent<HTMLElement>) => {
        const query = event.currentTarget.textContent?.trim();
        setQuery(query);

        searchProductsAsync(query)
        .then(data => {
            setSuggestedProducts([]);
            setProducts({
                products: data,
            });
        });
    }

    const debouncedQuery = useDebounce(query, 400);

    useEffect(() => {
        if (debouncedQuery && debouncedQuery.length > 1) {
            suggestProductsAsync(debouncedQuery)
            .then(data => {
                if (data.length === 1 && data[0].toLowerCase() === debouncedQuery.toLowerCase()) {
                    return;
                }
                setSuggestedProducts(data);
            });
        }
        else {
            setSuggestedProducts([]);
        }
    }, [debouncedQuery]);

    return (
        <SearchContext.Provider value={{
            products,
            setProducts,
            suggestedProducts,
            orders,
            query,
            setOrders,
            searchBoxChangeHandler,
            searchProductHandler
        }}>
            {children}
        </SearchContext.Provider>
    );
}