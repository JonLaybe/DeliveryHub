import { useEffect, useState, type FC } from "react";
import './ProductCardComponent.scss';
import type { ProductDto } from "../../models/catalog-service/ProductDto";
import { CATALOGSERVICE_PRODUCT_IMAGE_URL } from "../../constants/EndpointConstants";
import { CATALOG_BASE_URL } from "../../constants/EndpointConstants";
import { Link } from "react-router-dom";
import { addGroceryBasket, decreaseGroceryBasket, getItemGroceryBasket, isProductInGroceryBasket } from "../../services/grocery-basket/GroceryBasketService";
import { formattedPrice } from "../../pipe/GeneralPipe";
import ConterComponent from "../counter/ConterComponent";
import { LINK_PRODUCTS } from "../../constants/ValueTypeConstans";

interface ProductProps {
    product: ProductDto;
}

const ProductCardComponent: FC<ProductProps> = (props) => {
    const { product } = props;
    const [groceryBasketItem, setGroceryBasketItem] = useState(getItemGroceryBasket(product.id));
    const [counter, setCounter] = useState<number>(0);

    const imgThumb = product.images?.filter(x => x.type === 1)[0].url ?? '';
    const imgMain = product.images?.filter(x => x.type === 0)[0].url ?? '';

    const imageUrl = `${CATALOG_BASE_URL}${imgMain}`;

    useEffect(() => {
        if (groceryBasketItem)
            setCounter(groceryBasketItem.quantity);
    }, [groceryBasketItem])

    const handelIncreaseQuantity = () => {
        setCounter(counter + 1);
        addGroceryBasket(product);
    }

    const handelDecreaseQuantity = () => {
        if (decreaseGroceryBasket(product.id))
            setCounter(counter - 1);
    }

    return (
        <div className="product_card">
            <Link to={`${LINK_PRODUCTS}/${product.id}`}>
                <div className="product_card__preview">
                    <img src={imageUrl} alt={product.name} />
                </div>
            </Link>
            <span className="product_card__price">{formattedPrice(product.price)}</span>
            <span className="product_card__name">{product.name}</span>
            {
                counter !== 0 ? (
                    <div className="product_card__actions_quantity">
                        <ConterComponent counter={counter}
                            onClickMinus={() => handelDecreaseQuantity()}
                            onClickPlus={() => handelIncreaseQuantity()} />
                    </div>
                ) : (
                    <button className="default-button" onClick={() => handelIncreaseQuantity()}>В корзину</button>
                )
            }
        </div>
    )
}

export default ProductCardComponent;