import { useEffect, useState } from "react";
import './OrderComponent.scss';
import { getListOrdersAsync } from "../../services/order-service/OrderService";
import type { OrderDto } from "../../models/order-service/OrderDto";
import OrderListComponent from "../../common/order-list/OrderListComponent";
import { Link } from "react-router-dom";
import OrderDetailsComponent from "./dialogs/details/OrderDetailsComponent";
import BoxEmpty from '../../assets/orders/box_empty.webp';

const OrderComponent = () => {
    const [selectedOrderId, setSelectedOrderId] = useState(-1);
    const [selectedOrder, setSelectedOrder] = useState<OrderDto>();
    const [orders, setOrders] = useState<{ orders: OrderDto[] }>();

    useEffect(() => {
        getListOrdersAsync().then((data) => {
            if (!data)
                return;

            setOrders(() => ({
                orders: data.sort((a, b) => b.id - a.id),
            }));               
        });
    }, []);

    useEffect(() => {
        if (selectedOrderId < 0 || orders === undefined)
            return;
        setSelectedOrder(orders.orders.find(x => x.id === selectedOrderId));
    }, [selectedOrderId]);

    return (
        <div className="default_container order_container">
            {orders && orders.orders.length > 0 ? (
                <div className="my_orders">
                    <h1 className="default_name_chapter my_orders__name_chapter">Мои заказы</h1>
                    <div className='default_horizontal_multiple_containers order_horizontal_multiple_containers'>
                        <OrderListComponent listOrders={orders.orders} onSelected={(id) => { setSelectedOrderId(id); }}></OrderListComponent>
                        <div className="order_details">
                            <OrderDetailsComponent order={selectedOrder} />
                        </div>
                    </div>
                </div>
            ) : (
                <div className="shopping_cart_empy">
                    <div className="shopping_cart_empy__img">
                        <img src={BoxEmpty} alt="box_empty" />
                    </div>
                    <h1 className="shopping_cart_empy__main_message_text">Товары пока не куплены.</h1>
                    <span className="shopping_cart_empy__advice_message_text">
                        Пора это исправить.
                    </span>
                    <Link to="/" className="shopping_cart_empy__link default-link-button">К покупкам</Link>
                </div>
            )}
        </div >
    );
}

export default OrderComponent;