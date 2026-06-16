import { useEffect, useState, type FC } from "react";
import './HeaderComponent.scss';
import SearchBoxComponent from "../../../common/search-box/SearchBoxComponent";
import { Link } from "react-router";
import icon_order from '../../../assets/icon_order.svg';
import box_grocery_basket from '../../../assets/box_grocery_basket.svg';
import profile_icon from '../../../assets/profile_icon.svg';
import AuthModelComponent from "../../../components/auth/dialogs/AuthModelComponent";
import {
    getCurrentUser,
    isAuthentication,
    onAuthChanged
} from "../../../services/auth-service/AuthService";
import type { UserDto } from "../../../models/auth-service/UserDto";
import { getItemGroceryBasketCount } from "../../../services/grocery-basket/GroceryBasketService";

const HeaderComponent: FC = () => {
    const [isOpenModelLogin, setIsOpenModelLogin] = useState(false);
    const [isAuthed, setIsAuthed] = useState(isAuthentication());
    const [currentUser, setCurrentUser] = useState<UserDto | null>(null);
    const [basketCount, setBasketCount] = useState(getItemGroceryBasketCount());

    useEffect(() => {
        const syncBasket = () => {
            setBasketCount(getItemGroceryBasketCount());
        };

        window.addEventListener('basketStorageChanged', syncBasket);

        const syncAuth = async () => {
            const authenticated = isAuthentication();
            setIsAuthed(authenticated);

            if (!authenticated) {
                setCurrentUser(null);
                return;
            }

            try {
                const user = await getCurrentUser(true);
                setCurrentUser(user);
            } catch (error) {
                console.error("Не удалось загрузить текущего пользователя", error);
                setCurrentUser(null);
            }
        };

        syncAuth();

        const unsubscribeAuth = onAuthChanged(syncAuth);

        return () => {
            window.removeEventListener('basketStorageChanged', syncBasket);
            unsubscribeAuth();
        };
    }, []);

    const userName = currentUser?.firstName || currentUser?.email;

    return (
        <header className="header">
            <h1 className="header__name_product">
                <Link className="rest_default_link" to="/">DeliveryHub</Link>
            </h1>

            <div className="header__search_bar">
                <SearchBoxComponent placeholder={'Найти на DeliveryHub'} />
            </div>

            <div className="header__action_icons">
                <Link to="/orders" className="rest_default_link">
                    <div className="header__icon">
                        <img src={icon_order} alt="order" />
                    </div>
                </Link>

                <Link to="/grocery_basket" className="rest_default_link">
                    <div className="header__icon">
                        <img src={box_grocery_basket} alt="grocery_basket" />
                    </div>
                </Link>

                {basketCount > 0 && <span className="basket-count">{basketCount}</span>}

                {
                    isAuthed ?
                        <Link to="/profile" className="rest_default_link header__profile_link">
                            <div className="header__user">
                                <div className="header__icon">
                                    <img src={profile_icon} alt="profile_icon" />
                                </div>

                                {userName && (
                                    <span className="header__user_name">
                                        {userName}
                                    </span>
                                )}
                            </div>
                        </Link> :
                        <button className="header__icon" onClick={() => setIsOpenModelLogin(true)}>
                            <img src={profile_icon} alt="profile" />
                        </button>
                }

                <AuthModelComponent
                    value={isOpenModelLogin}
                    onChange={(newValue) => setIsOpenModelLogin(newValue)}
                />
            </div>
        </header>
    );
}

export default HeaderComponent;