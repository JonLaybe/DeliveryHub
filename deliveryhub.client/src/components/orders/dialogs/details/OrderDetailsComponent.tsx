import type { OrderDto } from '../../../../models/order-service/OrderDto';
import { getFormattedDate } from '../../../../pipe/GeneralPipe';
import './OrderDetailsComponent.scss';

const OrderDetailsComponent = ({ order }: { order: OrderDto | undefined }) => {
    return (
        <>
            {
                order !== undefined ? (
                    <div className="default_container order_details_container">
                        <div className="order_details__info">
                            <span className="name">
                                <span className="clue_message">Адрес доставки: </span>
                                {order.address}</span>
                            <span className="date-_create">
                                <span className="clue_message">Время создания заказа: </span>
                                {getFormattedDate(order.createdDate)}</span>
                            <span className="date-_create">
                                <span className="clue_message">Время доставки: </span>
                                {getFormattedDate(order.deliveryDate)}</span>
                            <span className="name">
                                <span className="clue_message">Статус: </span>
                                {order.status}</span>
                        </div>
                    </div>
                ) : ""
            }
        </>
    );
}

export default OrderDetailsComponent;