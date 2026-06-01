import './PaymentComponent.scss';
import { Controller, useForm } from 'react-hook-form';
import { ValidationConstants } from '../../constants/ValidationConstants';
import psMir from '../../assets/payments/ps-mir.svg';
import psMc from '../../assets/payments/ps-mc.svg';
import psAe from '../../assets/payments/ps-ae.svg';
import psUp from '../../assets/payments/ps-up.svg';
import psVisa from '../../assets/payments/ps-visa.svg';
import secureLock from '../../assets/secure_lock.svg';
import { useNavigate } from 'react-router-dom';

const PaymentComponent = () => {
    const navigate = useNavigate();

    const {
        control,
        register,
        reset,
        handleSubmit,
        formState: { isSubmitting, isSubmitSuccessful }
    } = useForm({
        defaultValues: {
            cartNumber: '',
            cartDate: '',
            cartPass: '',
        }
    });

    const formatCardNumber = (value: string) => {
        if (!ValidationConstants.CART_NUMBER.test(value)) {
            return value.slice(0, -1);
        }

        if ((value.length > 0 && value.length < 16) && value.length % 4 === 0 && value.charAt(value.length - 1) !== ' ') {
            return value + ' ';
        }

        return value;
    };

    const formatCartDate = (value: string) => {
        const cleanDigits = value.replace(/[^\d]/g, '');

        const limited = cleanDigits.substring(0, 4);

        if (limited.length > 2) {
            const month = limited.substring(0, 2);
            const year = limited.substring(2, 4);

            const validMonth = Number(month) > 12 ? '12' : month;
            const finalMonth = validMonth === '00' ? '01' : validMonth;

            return `${finalMonth}/${year}`;
        }

        return limited;
    };

    const formatCartPass = (value: string) => {
        if (!ValidationConstants.CART_PASS.test(value)) {
            return value.slice(0, -1);
        }

        return value;
    }

    const onCangeCartNumber = (onChange: any, e: any) => {
        const inputValue = e.target.value;
        const isDelete = (e.nativeEvent as InputEvent).inputType === 'deleteContentBackward';

        if ((e.nativeEvent as InputEvent).inputType === 'deleteContentForward')
            return;

        if (isDelete && inputValue.length > 0) {
            onChange(inputValue);
            return;
        }

        const formatted = formatCardNumber(inputValue);
        onChange(formatted);
    };

    const onSubmit = async () => {
        navigate('/grocery_basket', { state: { paymentSuccess: true } });
    };

    return (
        <div className='payment-wrapper'>
            <div className="payment-wrapper__context">
                <h1 className='default_name_chapter name_chapter'>Оплата</h1>
                <div className='default_container payment-container'>
                    <form onSubmit={handleSubmit(onSubmit)}>
                        <div className="payment-controllers">
                            <Controller
                                name="cartNumber"
                                control={control}
                                render={({ field: { onChange, value } }) => (
                                    <input
                                        value={value}
                                        className="default_input_filed_fill input-cart-number" type="text"
                                        placeholder='Номер карты'
                                        maxLength={16}
                                        onChange={(e) => onCangeCartNumber(onChange, e)}
                                    />
                                )}
                            />
                            <Controller
                                name="cartDate"
                                control={control}
                                render={({ field: { onChange, value } }) => (
                                    <input
                                        value={value}
                                        className="default_input_filed_fill" type="text"
                                        placeholder='ММ/ГГ'
                                        maxLength={5}
                                        onChange={(e) => onChange(formatCartDate(e.target.value))}
                                    />
                                )}
                            />
                            <Controller
                                name="cartPass"
                                control={control}
                                render={({ field: { onChange, value } }) => (
                                    <input
                                        value={value}
                                        className="default_input_filed_fill" type="password"
                                        placeholder='CVC/CVV'
                                        maxLength={5}
                                        onChange={(e) => onChange(formatCartPass(e.target.value))}
                                    />
                                )}
                            />
                            <div className="icons-payments">
                                <div className="icon-svg icon-payments">
                                    <img src={psMir} alt="profile_icon" />
                                </div>
                                <div className="icon-svg icon-payments">
                                    <img src={psVisa} alt="profile_icon" />
                                </div>
                                <div className="icon-svg icon-payments">
                                    <img src={psMc} alt="profile_icon" />
                                </div>
                                <div className="icon-svg icon-payments">
                                    <img src={psAe} alt="profile_icon" />
                                </div>
                                <div className="icon-svg icon-payments">
                                    <img src={psUp} alt="profile_icon" />
                                </div>
                            </div>
                        </div>
                        <div className="bottom_content">
                            <div className="information-secure-lock">
                                <div className="icon-svg icon-payments">
                                    <img src={secureLock} alt="profile_icon" />
                                </div>
                                <span className='default_text'>Ваши данные надёжно защищены</span>
                            </div>
                        </div>
                        <input
                            className='default-button'
                            type="submit"
                            disabled={isSubmitting}
                            value="Оплатить"
                        />
                    </form>
                </div>
            </div>
        </div>
    )
}

export default PaymentComponent;