import type { FC } from "react";
import './ProductCardComponent.scss';

const ProductCardComponent: FC = () => {
    return (
        <div className="product_card">
            <div className="product_card__preview">
                <img src="https://ir-5.ozone.ru/s3/multimedia-1-8/wc300/8070558200.jpg" alt="" />
            </div>
            <span className="product_card__price">2 329</span>
            <span className="product_card__name">Name product</span>
        </div>
    )
}

export default ProductCardComponent;