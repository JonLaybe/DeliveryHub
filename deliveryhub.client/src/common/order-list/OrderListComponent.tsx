import { useEffect, useState, type FC } from "react";
import './OrderListComponent.scss';
import type { OrderDto } from "../../models/order-service/OrderDto";
import { formattedPrice } from "../../pipe/GeneralPipe";
import { getListProductsByIdsAsync } from "../../services/catalog-service/ProductService";
import { flatMapOrderAndProduct, type OrderForList } from "../../pipe/OrderPipe";
import { CATALOG_BASE_URL } from "../../constants/EndpointConstants";
import PanelbarComponent from "../panelbar/PanelbarComponent";
import { Link } from "react-router-dom";
import { LINK_PRODUCTS } from "../../constants/ValueTypeConstans";

interface OrderListProps {
    listOrders: OrderDto[];
    onSelected: (id: number) => void;
}

const OrderListComponent: FC<OrderListProps> = (props) => {
    const { listOrders } = props;
    const [orderProducts, setOrderProducts] = useState<{ orderProducts: OrderForList[] }>({ orderProducts: [] });
    const [selectedOrderId, setSelectedOrderId] = useState(0);

    useEffect(() => {
        if (listOrders.length > 0)
            handleCheckboxChange(listOrders[0].id);

        let productIds = listOrders.flatMap(ord => {
            return ord.products.map(prd => prd.articleNumber);
        });

        if (!productIds)
            return;

        getListProductsByIdsAsync(productIds).then((data) => {
            if (!data)
                return;

            var result = flatMapOrderAndProduct(listOrders, data);

            setOrderProducts(() => ({
                orderProducts: result,
            }));
        });
    }, []);

    const handleCheckboxChange = (orderId: number) => {
        setSelectedOrderId(orderId);
        props.onSelected(orderId);
    };

    return (
        <div className="order_list_container">
            {
                orderProducts.orderProducts.map((ord, index) => (
                    <div className="card_order" key={ord.id}>
                        <PanelbarComponent
                            id={ord.id}
                            title={`Номер заказа: ${ord.id.toString()}`}
                            panelIsOpen={index === 0}
                            click={(id) => handleCheckboxChange(id)}>
                            <div className="card_product__checkbox">
                                <input
                                    type="checkbox"
                                    checked={selectedOrderId === ord.id}
                                    onChange={() => handleCheckboxChange(ord.id)}
                                />
                            </div>
                            <div className="card_products">
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
                                                <Link to={`${LINK_PRODUCTS}/${prd.articleNumber}`}
                                                    className="default-link-button order_clue_message_button_link">
                                                    Страница товара
                                                </Link>
                                            </div>
                                        </div>
                                    ))
                                }
                            </div>
                        </PanelbarComponent>
                    </div>
                ))
            }
        </div>
    );
}

export default OrderListComponent;