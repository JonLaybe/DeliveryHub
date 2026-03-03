import type { FC } from "react";
import './ProductComponent.scss';
import ProductCardComponent from "../../common/product-card/ProductCardComponent";

const ProductComponent: FC = () => {
    return (
        <div className="container">
            <div className="list_products">
                <ProductCardComponent></ProductCardComponent>
                <ProductCardComponent></ProductCardComponent>
                <ProductCardComponent></ProductCardComponent>
                <ProductCardComponent></ProductCardComponent>
                <ProductCardComponent></ProductCardComponent>
            </div>
        </div>
    );
}

export default ProductComponent;