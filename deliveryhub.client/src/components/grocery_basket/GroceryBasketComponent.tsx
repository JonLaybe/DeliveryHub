import { useEffect, useRef, useState, type FC } from 'react';
import './GroceryBasketComponent.scss';
import { getGroceryBasket, popPaymentData, refreshGroceryBasket, resetGroceryBasket, setPaymentData } from '../../services/grocery-basket/GroceryBasketService';
import { CATALOG_BASE_URL } from '../../constants/EndpointConstants';
import type { UUIDTypes } from 'uuid';
import { formattedPrice, getFormattedDateYMD } from '../../pipe/GeneralPipe';
import { createOrderAsync } from '../../services/order-service/OrderService';
import type { OrderCreateDto } from '../../models/order-service/OrderCreateDto';
import { mapGroceryBasketItemsToProduct } from '../../pipe/GroceryBasketPipe';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { isAuthentication } from '../../services/auth-service/AuthService';
import CounterComponent from '../../common/counter/CounterComponent';
import { Controller, useForm } from 'react-hook-form';
import PromoCodePanel, { type PromoApplyResult } from '../discounts/PromoCodePanel';
import type { ApplyModel } from "../../models/discount-service/ApplyModel";
import type { ApplyResponseModel } from "../../models/discount-service/ApplyResponseModel";
import { ApplyAsync } from '../../services/discount-service/DiscountService';

const GroceryBasketComponent: FC = () => {
    const [groceryBasket, setGroceryBasket] = useState(getGroceryBasket());
    const [totalPrice, setTotalPrice] = useState(0);
    const location = useLocation();
    const navigate = useNavigate();
    const isProcessed = useRef(false);
    const [discountAmount, setDiscountAmount] = useState(0);
    const [openPromoId, setOpenPromoId] = useState(false);
    const [discountUsagesId, setDiscountUsagesId] = useState(0);

    const {
        control,
        register,
        reset,
        handleSubmit,
        formState: { isValid, isSubmitting, isSubmitSuccessful }
    } = useForm({
        mode: 'onChange',
        defaultValues: {
            deliveryAddress: '',
            deliveryDate: ''
        }
    });

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

    const applyPromo = async (code: string): Promise<PromoApplyResult> => {
        try {
            const applyModel: ApplyModel = {
                code: code,
                orderAmount: totalPrice,
            };

            const response: ApplyResponseModel = await ApplyAsync(applyModel);

            if (response.success) {
                setDiscountAmount(response.appliedAmount ?? 0);
                setDiscountUsagesId(response.discountUsageId ?? 0);
                setOpenPromoId(false);
            } else {
                setDiscountAmount(0);
            }

            return {
                success: response.success,
                message: response.message,
                code: response.code,
                appliedAmount: response.appliedAmount,
                discountType: response.discountType,
                discountUsageId: response.discountUsageId
            };
        } catch (error) {
            return {
                success: false,
                message: 'Ошибка связи с сервером',
                code: undefined,
                appliedAmount: 0,
                discountType: undefined,
                discountUsageId: undefined,
            };
        }
    };

    const decreaseQuantity = (productId: UUIDTypes) => {
        setDiscountAmount(0);
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
        setDiscountAmount(0);
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

    const sentPayment = (data: any) => {
        if (!checkSentGroceryBasket())
            return;
        data.discount = discountAmount;
        data.discountUsageId = discountUsagesId;
        setPaymentData(data);
        navigate('/payment', { state: { fromBasket: true } });
    };

    const sentGroceryBasket = () => {
        if (!checkSentGroceryBasket())
            return;

        let paymentData = popPaymentData();

        if (!paymentData)
            return;

        let order: OrderCreateDto = {
            address: paymentData.deliveryAddress,
            deliveryDate: new Date(paymentData.deliveryDate),
            discount: paymentData.discount,
            discountUsageId: paymentData.discountUsageId,
            products: mapGroceryBasketItemsToProduct(groceryBasket),
        }

        createOrderAsync(order).then(data => {
            if (!data)
                return;

            resetGroceryBasket();
            navigate('/orders');
        });
    };

    const checkSentGroceryBasket = (): boolean => {
        if (groceryBasket.length > 0 && isAuthentication())
            return true;

        return false;
    };

    const minDeliveryDate = (): Date => {
        const moscowString = new Date().toLocaleDateString('en-US', { timeZone: 'Europe/Moscow' });
        const minDate = new Date(moscowString);
        minDate.setDate(minDate.getDate() + 3);

        return minDate;
    };

    const maxDeliveryDate = (): Date => {
        const moscowString = new Date().toLocaleDateString('en-US', { timeZone: 'Europe/Moscow' });
        const maxDate = new Date(moscowString);
        maxDate.setMonth(maxDate.getMonth() + 1);

        return maxDate;
    };

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
                        <div className="form_registration_new_order">
                            <div className="total_price">
                                <h1 className='default_name_chapter name_chapter'>Цена:</h1>
                                <span className='default_text total_price'>{formattedPrice(totalPrice)}</span>
                            </div>
                            {discountAmount > 0 && (<>
                                <div className="total_price">
                                    <h1 className='default_name_chapter name_chapter'>Скидка:</h1>
                                    <span className='default_text total_price'>-{formattedPrice(discountAmount)}</span>
                                </div>
                                <div className="total_price">
                                    <h1 className='default_name_chapter name_chapter'>Итого:</h1>
                                    <span className='default_text total_price'>{formattedPrice(totalPrice - discountAmount)}</span>
                                </div>
                            </>
                            )}
                            {discountAmount == 0 &&
                                <div className="registration_new_order">
                                    <PromoCodePanel value={openPromoId} onApply={(code) => applyPromo(code)} />
                                </div>
                            }
                        </div>
                        <div className="registration_new_order">
                            <form onSubmit={handleSubmit(sentPayment)}>
                                <div className="new_order_controllers">
                                    <div className="new_order_controllers__field">
                                        <label className='clue_message'>Адрес доставки:</label>
                                        <Controller name='deliveryAddress'
                                            control={control}
                                            rules={{ required: true }}
                                            render={({ field }) => (
                                                <input {...field} type="text"
                                                    placeholder='г. Москва, ул. Тверская'
                                                    maxLength={50}
                                                    className="default_input_filed_fill" />
                                            )}
                                        />
                                    </div>
                                    <div className="new_order_controllers__field">
                                        <label className='clue_message'>Время доставки:</label>
                                        <Controller name='deliveryDate'
                                            control={control}
                                            rules={{ required: true }}
                                            render={({ field }) => (
                                                <input {...field} type='date'
                                                    min={getFormattedDateYMD(minDeliveryDate())}
                                                    max={getFormattedDateYMD(maxDeliveryDate())}
                                                />
                                            )}
                                        />
                                    </div>
                                </div>
                                <input className='default-button'
                                    type="submit" disabled={!checkSentGroceryBasket() || !isValid || isSubmitSuccessful} value="Оформить заказ" />
                            </form>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
}

export default GroceryBasketComponent;