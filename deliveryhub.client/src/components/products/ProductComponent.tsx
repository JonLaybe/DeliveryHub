import { useEffect, useState, type FC } from "react";
import './ProductComponent.scss';
import ProductCardComponent from "../../common/product-card/ProductCardComponent";
import type { ProductDto } from "../../models/catalog-service/ProductDto";
import { getListProductsAsync } from "../../services/catalog-service/ProductService";

const ProductComponent: FC = () => {
    const [products, setProducts] = useState<{ products: ProductDto[] }>();

    useEffect(() => {
        getListProductsAsync().then(data => {
            if (!data)
                return;

            console.log(data);

            setProducts(() => ({
                products: data,
            }));
        })
    }, []);

    return (
        < div className="container" >
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