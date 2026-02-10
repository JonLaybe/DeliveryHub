import type { FC } from "react";
import { useGetOrdersQuery } from "../../services/OrderService";
import './OrderComponent.scss';

const OrderComponent: FC = () => {
    // const { data } = useGetOrderQuery(1);
    const { data, isLoading } = useGetOrdersQuery();

    console.log(data);

    return (
        <div className="container">
            {isLoading && !data ? (
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
                <>
                    <h1>Заказы:</h1>
                    {data?.map((el, i) => (
                        <div className="card" key={i}>
                            <span>№ {el.orderNumber}</span>
                            <span>Status: {el.status}</span>
                        </div>
                    ))}
                </>
            )}
        </div>
    );
}

export default OrderComponent;