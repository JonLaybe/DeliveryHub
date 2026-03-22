import { useEffect, useState, type FC } from 'react';
import './GroceryBasketComponent.scss';
import { getGroceryBasket, refreshGroceryBasket, resetGroceryBasket } from '../../services/grocery-basket/GroceryBasketService';
import { CATALOG_BASE_URL } from '../../constants/EndpointConstants';
import minus_actions from '../../assets/minus_actions.svg';
import pluse_actions from '../../assets/pluse_actions.svg';
import type { UUIDTypes } from 'uuid';
import { formattedPrice } from '../../pipe/GeneralPipe';
import { createOrderAsync } from '../../services/order-service/OrderService';
import type { OrderCreateDto } from '../../models/order-service/OrderCreateDto';
import { mapGroceryBasketItemsToProduct } from '../../pipe/GroceryBasket/GroceryBasketPipe';
import { Link, useNavigate } from 'react-router-dom';

const GroceryBasketComponent: FC = () => {
    const [groceryBasket, setGroceryBasket] = useState(getGroceryBasket());
    const [totalPrice, setTotalPrice] = useState(0);
    const navigate = useNavigate();

    useEffect(() => {
        let result = 0;

        groceryBasket.map(prod => {
            result += prod.price;
        });

        setTotalPrice(result);
    }, []);

    const decreaseQuantity = (productId: UUIDTypes) => {
        let refGroceryBasket = groceryBasket.map(item => {
            if (item.product.id === productId && item.quantity > 1) {
                setTotalPrice(totalPrice - item.product.price);
                return { ...item, quantity: item.quantity - 1, price: item.product.price * (item.quantity - 1) };
            }
            return item;
        });

        refreshGroceryBasket(refGroceryBasket);
        setGroceryBasket(refGroceryBasket);
    };

    const increaseQuantity = (productId: UUIDTypes) => {
        let refGroceryBasket = groceryBasket.map(item => {
            if (item.product.id === productId) {
                setTotalPrice(totalPrice + item.product.price);
                return { ...item, quantity: item.quantity + 1, price: item.product.price * (item.quantity + 1) };
            }
            return item;
        });

        refreshGroceryBasket(refGroceryBasket);
        setGroceryBasket(refGroceryBasket);
    };

    const sentGroceryBasket = () => {
        let order: OrderCreateDto = {
            address: "г. Москва, ул. Пупкина, д. 5, кв. 31",
            deliveryDate: new Date((new Date).getTime() + 1),
            products: mapGroceryBasketItemsToProduct(groceryBasket),
        }

        createOrderAsync(order).then(data => {
            if (!data)
                return;

            resetGroceryBasket();
            navigate('/orders');
        });
    }

    return (
        <div>
            <h1 className='default_name_chapter name_chapter'>Магазин</h1>
            <div className='default_horizontal_multiple_containers grocery_basket_horizontal_multiple_containers'>
                <div className="default_container grocery_basket_container">
                    <div className="grocery_basket_items">
                        {groceryBasket && groceryBasket.length > 0 ? (
                            groceryBasket.map(gb_item => (
                                <div className='grocery_basket_card' key={gb_item.product.id.toString()}>
                                    <div className="grocery_basket_card__info">
                                        <div className="card_preview">
                                            <img src={gb_item.product.images && gb_item.product.images.length > 0 ? `${CATALOG_BASE_URL}/${gb_item.product.images[0].url}` : undefined} alt={gb_item.product.name} />
                                        </div>
                                        <div className="card_item_info">
                                            <span className='default_text'>{gb_item.product.name}</span>
                                            <span className='default_text description'>{gb_item.product.description}</span>
                                        </div>
                                    </div>
                                    <div className="grocery_basket_card__actions_quantity">
                                        <button className='default-button grocery_basket_actions' onClick={() => decreaseQuantity(gb_item.product.id)}>
                                            <img src={minus_actions} alt="minus" />
                                        </button>
                                        <span className='default_text'>{gb_item.quantity}</span>
                                        <button className='default-button grocery_basket_actions' onClick={() => increaseQuantity(gb_item.product.id)}>
                                            <img src={pluse_actions} alt="plus" />
                                        </button>
                                    </div>
                                    <div className="grocery_basket_card__price">
                                        <span className='default_text'>{formattedPrice(gb_item.price)}</span>
                                    </div>
                                </div>
                            ))
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
                    </div>
                </div>
                <div className='default_container result_grocery_basket_container'>
                    <div className="form_registration_new_order">
                        <div className="total_price">
                            <h1 className='default_name_chapter name_chapter'>Итого:</h1>
                            <span className='default_text total_price'>{formattedPrice(totalPrice)}</span>
                        </div>
                        <div className="registration_new_order">
                            <button className='default-button' onClick={() => sentGroceryBasket()}>Оформить заказ</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
}

export default GroceryBasketComponent;