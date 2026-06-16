import { useEffect, useState } from 'react';
import type { OrderDto } from '../../../../models/order-service/OrderDto';
import { formattedPrice, getFormattedDate } from '../../../../pipe/GeneralPipe';
import './OrderDetailsComponent.scss';
import { deliveryState } from '../../../../constants/ValueTypeConstans';

const OrderDetailsComponent = ({ order }: { order: OrderDto | undefined }) => {
    const [totalPrice, setTotalPrice] = useState(0);

    useEffect(() => {
        console.log(order);

        if (!order)
            return;

        let result = 0;

        order.products.map(prd => {
            result += prd.quantity * prd.price;
        });

        setTotalPrice(result);
    }, [order]);

    return (
        <>
            {
                order !== undefined ? (
                    <div className="default_container order_details_container">
                        <div className="order_details__info">
                            <span className="delivery_number">
                                <span className="clue_message">Номер заказа: </span>
                                {order.id}</span>
                            <span className="name">
                                <span className="clue_message">Адрес доставки: </span>
                                {order.address}</span>
                            <span className="created_date">
                                <span className="clue_message">Время создания заказа: </span>
                                {getFormattedDate(order.createdDate)}</span>
                            <span className="delivery_date">
                                <span className="clue_message">Время доставки: </span>
                                {getFormattedDate(order.deliveryDate)}</span>
                            <span className="name">
                                <span className="clue_message">Статус: </span>
                                {deliveryState.find(x => order.status === x.code)?.value}</span>
                            <div className="total_price">
                                <h1 className='default_name_chapter name_chapter'>Цена:</h1>
                                <span className='default_text amount_price'>{formattedPrice(totalPrice)}</span>
                            </div>
                            {order.discount!=null &&
                            <>
                            <div className="total_price">
                                <h1 className='default_name_chapter name_chapter'>Скидка:</h1>
                                <span className='default_text amount_price'>-{formattedPrice(order.discount)}</span>
                            </div>
                            <div className="total_price">
                                <h1 className='default_name_chapter name_chapter'>Итого:</h1>
                                <span className='default_text amount_price'>{formattedPrice(totalPrice-order.discount)}</span>
                            </div>
                            </>}
                        </div>
                    </div>
                ) : ""
            }
        </>
    );
}

export default OrderDetailsComponent;