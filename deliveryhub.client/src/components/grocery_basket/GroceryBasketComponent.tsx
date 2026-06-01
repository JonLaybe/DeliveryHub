import { useEffect, useRef, useState, type FC } from 'react';
import './GroceryBasketComponent.scss';
import { getGroceryBasket, refreshGroceryBasket, resetGroceryBasket } from '../../services/grocery-basket/GroceryBasketService';
import { CATALOG_BASE_URL } from '../../constants/EndpointConstants';
import type { UUIDTypes } from 'uuid';
import { formattedPrice } from '../../pipe/GeneralPipe';
import { createOrderAsync } from '../../services/order-service/OrderService';
import type { OrderCreateDto } from '../../models/order-service/OrderCreateDto';
import { mapGroceryBasketItemsToProduct } from '../../pipe/GroceryBasketPipe';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { isAuthentication } from '../../services/auth-service/AuthService';
import CounterComponent from '../../common/counter/CounterComponent';

const GroceryBasketComponent: FC = () => {
    const [groceryBasket, setGroceryBasket] = useState(getGroceryBasket());
    const [totalPrice, setTotalPrice] = useState(0);
    const location = useLocation();
    const navigate = useNavigate();
    const isProcessed = useRef(false);

    useEffect(() => {
        let result = 0;

        groceryBasket.map(prod => {
            result += prod.price;
        });

        setTotalPrice(result);
    }, []);

    useEffect(() => {
        if (location.state?.paymentSuccess && !isProcessed.current) {
            isProcessed.current = true;
            navigate(location.pathname, { replace: true, state: {} });
            sentGroceryBasket();
        }
    }, [location.state, navigate, groceryBasket]);

    const decreaseQuantity = (productId: UUIDTypes) => {
        let product = groceryBasket.find(item => item.product.id === productId);

        if (!product) return;

        let refGroceryBasket;

        if (product.quantity === 1) {
            setTotalPrice(totalPrice - product.product.price);
            refGroceryBasket = groceryBasket.filter(item => item.product.id !== productId);
        }
        else if (product.quantity > 1) {
            refGroceryBasket = groceryBasket.map(item => {
                if (item.product.id === productId && item.quantity > 1) {
                    setTotalPrice(totalPrice - item.product.price);
                    return { ...item, quantity: item.quantity - 1, price: item.product.price * (item.quantity - 1) };
                }
                return item;
            });
        }

        if (refGroceryBasket) {
            refreshGroceryBasket(refGroceryBasket);
            setGroceryBasket(refGroceryBasket);
        }
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

    const sentPayment = () => {
        if (!checkSentGroceryBasket())
            return;

        navigate('/payment', { state: { fromBasket: true } });
    }

    const sentGroceryBasket = () => {
        if (!checkSentGroceryBasket())
            return;

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

    const checkSentGroceryBasket = (): boolean => {
        if (groceryBasket.length > 0 && isAuthentication())
            return true;

        return false;
    }

    return (
        <div className='root_grocery_basket'>
            <h1 className='default_name_chapter name_chapter'>Корзина</h1>
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
                                        <CounterComponent counter={gb_item.quantity}
                                            onClickMinus={() => decreaseQuantity(gb_item.product.id)}
                                            onClickPlus={() => increaseQuantity(gb_item.product.id)} />
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
                            <button className='default-button' onClick={() => sentPayment()} disabled={!checkSentGroceryBasket()}>Оформить заказ</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
}

export default GroceryBasketComponent;