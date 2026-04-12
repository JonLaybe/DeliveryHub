import { useEffect, useContext, type FC } from "react";
import './ProductsComponent.scss';
import ProductCardComponent from "../../common/product-card/ProductCardComponent";
import { SearchContext } from "../../context/SearchContext";

const ProductsComponent: FC = () => {
    const { 
        products, 
        searchBoxChangeHandler,
        showFiltersBlock,
        showFilters,
        clearFilters,
        filtersCount,
        setFiltersCount,
        serachProductsAndSetResults
    } = useContext(SearchContext);

    useEffect(() => {
        clearFilters();
        searchBoxChangeHandler({ target: { value: '' } } as React.ChangeEvent<HTMLInputElement>)

        serachProductsAndSetResults({
            text: '',
        });

        setFiltersCount(0);
    }, []);

    return (
        <>
            {showFiltersBlock && 
            <div className="filters-block">
                <button onClick={() => showFilters(true)} className="filter-button">Фильтры</button>
                {filtersCount > 0 && <span className="filters-count">{filtersCount}</span>}
            </div>}

            <div className="container" >
                <div className="list_products">
                    {
                        products?.products.map(prd => (
                            <ProductCardComponent product={prd} key={prd.id.toString()}/>
                        ))
                    }
                </div>
            </div>
        </>
    );
}

export default ProductsComponent;