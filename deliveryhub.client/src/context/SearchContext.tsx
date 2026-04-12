import { createContext, useEffect, useState } from "react";
import { searchProductsAsync, suggestProductsAsync } from "../services/catalog-service/ProductService";
import { useDebounce } from "../hooks/useDebounce";
import type { ProductDto } from "../models/catalog-service/ProductDto";
import type { ProductSearchQueryRequest } from "../models/catalog-service/ProductSearchQueryRequest";

interface SearchContextType {
    products: { products: ProductDto[] } | undefined
    setProducts: (value: { products: ProductDto[] } | undefined) => void
    query: string,
    showFiltersBlock: boolean,
    showFilters: (value: boolean) => void,
    filtersIsShown: boolean,
    filtersCount: number,
    setFiltersCount: (value: number) => void,
    suggestedProducts: string[],
    filterAttributes?: Record<string, string[]>,
    searchBoxChangeHandler: (event: React.ChangeEvent<HTMLInputElement>) => void,
    searchProductHandler: (event: React.MouseEvent<HTMLInputElement>) => void,
    searchBoxKeyDownHandler?: (event: React.KeyboardEvent<HTMLInputElement>) => void,
    serachProductsAndSetResults: (request: ProductSearchQueryRequest) => void,
    clearFilters: () => void
}

export const SearchContext = createContext<SearchContextType>({
    products: undefined,
    setProducts: () => {},
    query: '',
    showFiltersBlock: true,
    showFilters: () => {},
    filtersIsShown: false,
    filtersCount: 0,
    setFiltersCount: () => {},
    suggestedProducts: [],
    filterAttributes: undefined,
    searchBoxChangeHandler: () => {},
    searchProductHandler: () => {},
    serachProductsAndSetResults: () => {},
    clearFilters: () => {}
});

export const SearchProvider = (props:any) => {
    const { children } = props;
    const [products, setProducts] = useState<{ products: ProductDto[] }>();
    const [query, setQuery] = useState<string>('');
    const [showFiltersBlock, setShowFiltersBlock] = useState<boolean>(true);
    const [filtersIsShown, setFiltersIsShown] = useState<boolean>(false);
    const [filtersCount, setFiltersCount] = useState(0);
    const [suggestedProducts, setSuggestedProducts] = useState<string[]>([]);
    const [filterAttributes, setFilterAttributes] = useState<Record<string, string[]> | undefined>();

    const showFilters = (value: boolean) => {
        setFiltersIsShown(value);
    }

    const clearFilters = () => {
        setFilterAttributes(undefined);
        // setShowFiltersBlock(false);
    }

    const searchBoxChangeHandler = (event: React.ChangeEvent<HTMLInputElement>) => {
        setQuery(event.target.value);
    };

    const serachProductsAndSetResults = (request: ProductSearchQueryRequest) => {
        if (request.text.length === 0) {
        }

        searchProductsAsync(request)
        .then(data => {
            setSuggestedProducts([]);
            setProducts({
                products: data.products,
            });
            setFilterAttributes(data.attributes);

            if (data.products.length > 0) {
                // setShowFiltersBlock(true);
            }
        });
    };


    const searchBoxKeyDownHandler = (event: React.KeyboardEvent<HTMLInputElement>) => {
        if (event.key === 'Enter') {
            serachProductsAndSetResults({ text: query });
        }
    };


    const searchProductHandler = (event: React.MouseEvent<HTMLElement>) => {
        const query = event.currentTarget.textContent?.trim();
        setQuery(query);

        serachProductsAndSetResults({ text: query ?? '' });
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
            query,
            showFiltersBlock,
            showFilters,
            filtersIsShown,
            filterAttributes,
            filtersCount,
            setFiltersCount,
            clearFilters,
            searchBoxChangeHandler,
            searchProductHandler,
            searchBoxKeyDownHandler,
            serachProductsAndSetResults
        }}>
            {children}
        </SearchContext.Provider>
    );
}