import type { FC } from "react";
import './OrderListComponent.scss';
import type { OrderDto } from "../../models/order-service/OrderDto";

interface OrderListProps {
    listOrders: OrderDto[];
}

const OrderListComponent: FC<OrderListProps> = (props) => {
    const { listOrders } = props;

    return (
        <div className="order_list_container">
            {
                listOrders.map(ord => (
                    <div className="card_order" key={ord.id}>
                        <div className="card_order__info">
                            <span className="identity_order text_hover">{ord.orderNumber}</span>
                        </div>
                        {
                            ord.products.map(prd => (
                                <div className="card_product" key={prd.id}>
                                    <div className="card_product__preview">
                                        <img src={prd.photoPreviewUrl} alt="preview" />
                                    </div>
                                    <div className="card_product__main_info">
                                        <span className="price">
                                            <span className="clue_message">Цена: </span>
                                            {prd.price.toString()} ₽</span>
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