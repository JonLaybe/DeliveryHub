import { createContext, useEffect, useState } from "react";
import { getCategoriesAsync, searchProductsAsync, suggestProductsAsync } from "../services/catalog-service/ProductService";
import { useDebounce } from "../hooks/useDebounce";
import type { ProductDto } from "../models/catalog-service/ProductDto";
import type { ProductSearchQueryRequest } from "../models/catalog-service/ProductSearchQueryRequest";
import type { CategoryDto } from "../models/catalog-service/CategoryDto";

interface SearchContextType {
    products: { products: ProductDto[] } | undefined
    setProducts: (value: { products: ProductDto[] } | undefined) => void
    query: string,
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
    clearFilters: () => void,
    filterCategories: CategoryDto[],
}

export const SearchContext = createContext<SearchContextType>({
    products: undefined,
    setProducts: () => {},
    query: '',
    showFilters: () => {},
    filtersIsShown: false,
    filtersCount: 0,
    setFiltersCount: () => {},
    suggestedProducts: [],
    filterAttributes: undefined,
    searchBoxChangeHandler: () => {},
    searchProductHandler: () => {},
    serachProductsAndSetResults: () => {},
    clearFilters: () => {},
    filterCategories: [],
});

export const SearchProvider = (props:any) => {
    const { children } = props;
    const [products, setProducts] = useState<{ products: ProductDto[] }>();
    const [query, setQuery] = useState<string>('');
    const [allCategories, setAllCategories] = useState<CategoryDto[]>([]);
    const [filterCategories, setFilterCategories] = useState<CategoryDto[]>([]);
    const [filtersIsShown, setFiltersIsShown] = useState<boolean>(false);
    const [filtersCount, setFiltersCount] = useState(0);
    const [suggestedProducts, setSuggestedProducts] = useState<string[]>([]);
    const [filterAttributes, setFilterAttributes] = useState<Record<string, string[]> | undefined>();

    const showFilters = (value: boolean) => {
        setFiltersIsShown(value);
    }

    useEffect(() => {
        getCategoriesAsync().then(data => 
            { setAllCategories(data); setFilterCategories(data); });
    }, []);

    const clearFilters = () => {
        setFilterAttributes(undefined);
    }

    const searchBoxChangeHandler = (event: React.ChangeEvent<HTMLInputElement>) => {
        setQuery(event.target.value);
    };

    const serachProductsAndSetResults = (request: ProductSearchQueryRequest) => {

        searchProductsAsync(request)
        .then(data => {
            setSuggestedProducts([]);
            setProducts({
                products: data.products,
            });
            setFilterAttributes(data.attributes);

            if (filtersCount > 0) {
                setFilterCategories(allCategories.filter(c => data.products.some(p => p.categoryId === c.id)));
            }
            else {
                if (allCategories.length > 0)
                    setFilterCategories(allCategories);
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
            showFilters,
            filtersIsShown,
            filterAttributes,
            filtersCount,
            setFiltersCount,
            clearFilters,
            searchBoxChangeHandler,
            searchProductHandler,
            searchBoxKeyDownHandler,
            serachProductsAndSetResults,
            filterCategories,
        }}>
            {children}
        </SearchContext.Provider>
    );
}