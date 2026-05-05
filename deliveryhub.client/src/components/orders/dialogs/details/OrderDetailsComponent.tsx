import type { OrderDto } from '../../../../models/order-service/OrderDto';
import './OrderDetailsComponent.scss';

const OrderDetailsComponent = ({order}: {order: OrderDto}) => {

    return (
        <div className="default_container order_details_container">
            <span>{order.address}</span>
            <span>{order.createdDate.toString()}</span>
            <span>{order.deliveryDate.toString()}</span>
            <span>{order.status}</span>
        </div>
    );
}

export default OrderDetailsComponent;