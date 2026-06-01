import { useEffect, useContext, type FC, useState } from "react";
import './ProductsComponent.scss';
import ProductCardComponent from "../../common/product-card/ProductCardComponent";
import { SearchContext } from "../../context/SearchContext";
import FiltersControlComponent from "../filters/FiltersControlComponent";
import { getGroceryBasket } from "../../services/grocery-basket/GroceryBasketService";
import { mapProductToGroceryBasketItem } from "../../pipe/GroceryBasketPipe";

const ProductsComponent: FC = () => {
    const {
        products,
        searchBoxChangeHandler,
        clearFilters,
        setFiltersCount,
        serachProductsAndSetResults,
    } = useContext(SearchContext);

    const [groceryBasket, setGroceryBasket] = useState(getGroceryBasket());

    useEffect(() => {
        clearFilters();
        searchBoxChangeHandler({ target: { value: '' } } as React.ChangeEvent<HTMLInputElement>);

        serachProductsAndSetResults({
            text: '',
        });

        setFiltersCount(0);
    }, []);

    return (
        <>
            <FiltersControlComponent />

            <div className="container" >
                <div className="list_products">
                    {
                        products?.products.map(prd => (
                            <ProductCardComponent product={prd} key={prd.id.toString()} />
                        ))
                    }
                </div>
            </div>
        </>
    );
};

export default ProductsComponent;