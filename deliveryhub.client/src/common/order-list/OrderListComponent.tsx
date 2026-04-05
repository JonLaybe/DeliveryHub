import { useEffect, useState, type FC } from "react";
import './OrderListComponent.scss';
import type { OrderDto } from "../../models/order-service/OrderDto";
import { formattedPrice } from "../../pipe/GeneralPipe";
import { getListProductsByIdsAsync } from "../../services/catalog-service/ProductService";
import { flatMapOrderAndProduct, type OrderForList } from "../../pipe/OrderPipe";
import { CATALOG_BASE_URL } from "../../constants/EndpointConstants";

interface OrderListProps {
    listOrders: OrderDto[];
}

const OrderListComponent: FC<OrderListProps> = (props) => {
    const { listOrders } = props;
    const [orderProducts, setOrderProducts] = useState<{ orderProducts: OrderForList[] }>({ orderProducts: [] });

    useEffect(() => {
        let productIds = listOrders.flatMap(ord => {
            return ord.products.map(prd => prd.articleNumber);
        });

        if (!productIds)
            return;

        getListProductsByIdsAsync(productIds).then((data) => {
            if (!data)
                return;

            var result = flatMapOrderAndProduct(listOrders, data);

            console.log(result);

            setOrderProducts(() => ({
                orderProducts: result,
            }));
        });
    }, []);

    return (
        <div className="order_list_container">
            {
                orderProducts.orderProducts.map(ord => (
                    <div className="card_order" key={ord.id}>
                        <div className="card_order__info">
                            <span className="identity_order text_hover">{ord.id}</span>
                        </div>
                        {
                            ord.products.map(prd => (
                                <div className="card_product" key={prd.id}>
                                    <div className="card_product__preview">
                                        <img src={prd.image ? `${CATALOG_BASE_URL}${prd.image.url}` : ''} alt="preview" />
                                    </div>
                                    <div className="card_product__main_info">
                                        <span className="price">
                                            <span className="clue_message">Цена: </span>
                                            {formattedPrice(prd.price)}</span>
                                        <span className="name">
                                            <span className="clue_message">Название: </span>
                                            {prd.name}</span>
                                        <span className="quantity">
                                            <span className="clue_message">Количество: </span>
                                            {prd.quantity.toString()}</span>
                                        <span className="article_number text_hover clue_message">
                                            {prd.articleNumber}</span>
                                    </div>
                                </div>
                            ))
                        }
                    </div>
                ))
            }
        </div>
    );
}

export default OrderListComponent;