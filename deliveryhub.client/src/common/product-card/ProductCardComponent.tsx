import type { FC } from "react";
import './ProductCardComponent.scss';
import type { ProductDto } from "../../models/catalog-service/ProductDto";
import { CATALOGSERVICE_PRODUCT_IMAGE_URL } from "../../constants/EndpointConstants";
import { CATALOG_BASE_URL } from "../../constants/EndpointConstants";
import { Link } from "react-router-dom";
import { addGroceryBasket } from "../../services/grocery-basket/GroceryBasketService";
import { formattedPrice } from "../../pipe/GeneralPipe";

interface ProductProps {
    product: ProductDto;
}

const ProductCardComponent: FC<ProductProps> = (props) => {
    const { product } = props;

    const imgThumb = product.images?.filter(x => x.type === 1)[0].url ?? '';
    const imgMain = product.images?.filter(x => x.type === 0)[0].url ?? '';

    const imageUrl = `${CATALOG_BASE_URL}${imgMain}`;

    return (
        <div className="product_card">
            <Link to={`/product/${product.id}`}>
                <div className="product_card__preview">
                    <img src={imageUrl} alt={product.name} />
                </div>
            </Link>
            <span className="product_card__price">{formattedPrice(product.price)}</span>
            <span className="product_card__name">{product.name}</span>
            <button className="default-button" onClick={() => addGroceryBasket(product)}>В корзину</button>
        </div>
    )
}

export default ProductCardComponent;