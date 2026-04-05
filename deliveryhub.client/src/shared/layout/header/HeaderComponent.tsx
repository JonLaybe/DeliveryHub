import { useState, type FC } from "react";
import './HeaderComponent.scss';
import SearchBoxComponent from "../../../common/search-box/SearchBoxComponent";
import { Link } from "react-router";
import icon_order from '../../../assets/icon_order.svg';
import box_grocery_basket from '../../../assets/box_grocery_basket.svg';
import profile_icon from '../../../assets/profile_icon.svg';
import AuthModelComponent from "../../../components/auth/dialogs/AuthModelComponent";

const HeaderComponent: FC = () => {
    const [isOpenModelLogin, setIsOpenModelLogin] = useState(false);

    return (
        <header className="header">
            <h1 className="header__name_product">
                <Link className="rest_default_link" to="/">DeliveryHub</Link>
            </h1>
            <div className="header__search_bar">
                <SearchBoxComponent placeholder={'Найти на DeliveryHub'}></SearchBoxComponent>
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
                <button className="header__icon" onClick={() => setIsOpenModelLogin(true)}>
                    <img src={profile_icon} alt="profile" />
                </button>
                <AuthModelComponent value={isOpenModelLogin} onChange={(newValue) => setIsOpenModelLogin(newValue)}/>
            </div>
        </header>
    );
}

export default HeaderComponent;