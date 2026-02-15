import { useEffect, useState, type FC } from "react";
import './OrderComponent.scss';
import { getListOrdersAsync } from "../../services/OrderService";
import type { OrderDto } from "../../models/Orders/OrderDto";
import OrderListComponent from "../../common/order-list/OrderListComponent";

const OrderComponent: FC = () => {
    const [orders, setOrders] = useState<{ orders: OrderDto[] }>();

    useEffect(() => {
        getListOrdersAsync().then((data) => {
            setOrders(() => ({
                orders: data,
            }));
        });
    }, []);

    return (
        <div className="order_container">
            {!orders ? (
                <div className="shopping_cart_empy">
                    <div className="shopping_cart_empy__img">
                        <img src="https://nsk-static-cdn-03.geobasket.ru/vol2/site/i/v3/empty/cart.webp" alt="" />
                    </div>
                    <h1 className="shopping_cart_empy__main_message_text">В корзине пока пусто</h1>
                    <span className="shopping_cart_empy__advice_message_text">
                        Загляните на главную — собрали там товары, которые могут вам понравиться
                    </span>
                    <button className="shopping_cart_empy__route_root default-button">Перейти на главную</button>
                </div>
            ) : (
                <div className="my_orders">
                    <h1 className="my_orders__name_chapter">Мои заказы</h1>
                    <OrderListComponent listOrders={orders.orders}></OrderListComponent>
                </div>
            )
            }
        </div >
    );
}

export default OrderComponent;