import { useEffect, useContext, type FC } from "react";
import './ProductComponent.scss';
import ProductCardComponent from "../../common/product-card/ProductCardComponent";
import { getListProductsAsync } from "../../services/catalog-service/ProductService";
import { SearchContext } from "../../context/SearchContext";

const ProductComponent: FC = () => {
    const { products, setProducts} = useContext(SearchContext);

    useEffect(() => {
        getListProductsAsync().then(data => {
            if (!data)
                return;

            setProducts({
                products: data,
            });
        })
    }, []);

    return (
        <div className="container" >
            <div className="list_products">
                {
                    products?.products.map(prd => (
                        <ProductCardComponent product={prd} key={prd.id.toString()}/>
                    ))
                }
            </div>
        </div>
    );
}

export default ProductComponent;