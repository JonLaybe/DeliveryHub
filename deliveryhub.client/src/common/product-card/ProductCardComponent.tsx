import type { FC } from "react";
import './ProductCardComponent.scss';
import type { ProductDto } from "../../models/catalog-service/ProductDto";
import { CATALOGSERVICE_PRODUCT_IMAGE_URL } from "../../constants/EndpointConstants";

interface ProductProps {
    product: ProductDto;
}

const ProductCardComponent: FC<ProductProps> = (props) => {
    const prefix = `http://localhost:5000/${CATALOGSERVICE_PRODUCT_IMAGE_URL}`;

    const { product } = props;

    const imageUrl = product.images ? `${prefix}/${product.images[0]?.productId}` : '';

    return (
        <div className="product_card">
            <div className="product_card__preview">
                <img src={imageUrl} alt={product.name} />
            </div>
            <span className="product_card__price">{product.price.toString()}</span>
            <span className="product_card__name">{product.name}</span>
            <button className="default-button">В корзину</button>
        </div>
    )
}

export default ProductCardComponent;