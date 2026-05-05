import { useEffect, useState } from "react";
import './OrderComponent.scss';
import { getListOrdersAsync } from "../../services/order-service/OrderService";
import type { OrderDto } from "../../models/order-service/OrderDto";
import OrderListComponent from "../../common/order-list/OrderListComponent";
import { Link } from "react-router-dom";
import OrderDetailsComponent from "./dialogs/details/OrderDetailsComponent";

const OrderComponent = () => {
    const [orders, setOrders] = useState<{ orders: OrderDto[] }>();

    useEffect(() => {
        getListOrdersAsync().then((data) => {
            if (!data)
                return;

            setOrders(() => ({
                orders: data,
            }));
        });
    }, []);

    return (
        <div className="default_container order_container">
            {orders && orders.orders.length > 0 ? (
                <div className="my_orders">
                    <h1 className="default_name_chapter my_orders__name_chapter">Мои заказы</h1>
                    <div className="order_grid">
                        <OrderListComponent listOrders={orders.orders}></OrderListComponent>
                        <OrderDetailsComponent order={orders.orders[0]} />
                    </div>
                </div>
            ) : (
                <div className="shopping_cart_empy">
                    <div className="shopping_cart_empy__img">
                        <img src="https://nsk-static-cdn-03.geobasket.ru/vol2/site/i/v3/empty/cart.webp" alt="" />
                    </div>
                    <h1 className="shopping_cart_empy__main_message_text">В корзине пока пусто</h1>
                    <span className="shopping_cart_empy__advice_message_text">
                        Загляните на главную — собрали там товары, которые могут вам понравиться
                    </span>
                    <Link to="/" className="shopping_cart_empy__link default-link-button">
                        Перейти на главную
                    </Link>
                </div>
            )}
        </div >
    );
}

export default OrderComponent;