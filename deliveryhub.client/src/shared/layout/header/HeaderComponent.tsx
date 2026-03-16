import type { FC } from "react";
import './HeaderComponent.scss';
import SearchBoxComponent from "../../../common/search-box/SearchBoxComponent";
import { Link } from "react-router";

const HeaderComponent: FC = () => {
    return (
        <>
            <header className="header">
                <h1 className="header__name_product">
                    <Link to="/">DeliveryHub</Link>
                </h1>
                <div className="header__search_bar">
                    <SearchBoxComponent placeholder={'Найти на DeliveryHub'}></SearchBoxComponent>
                </div>
                <div className="header__action_icons">
                    <div className="header__icon">
                        <Link to="/orders" className="rest_default_link">
                            <svg viewBox="0 0 49 56" version="1.1">
                                <g id="Page-1" stroke="none" strokeWidth="1" fill="none" fillRule="evenodd">
                                    <g id="Basket" fill="white">
                                        <path d="M48.2,55.5 L0.5,55.5 L0.5,13.5 L48.2,13.5 L48.2,55.5 Z M3.5,52.5 L45.2,52.5 L45.2,16.5 L3.5,16.5 L3.5,52.5 Z" id="Shape" />
                                        <path d="M38,25.7 L35,25.7 L35,11.3 C35,7 31.5,3.5 27.2,3.5 L21.6,3.5 C17.3,3.5 13.8,7 13.8,11.3 L13.8,25.7 L10.8,25.7 L10.8,11.3 C10.8,5.3 15.6,0.5 21.6,0.5 L27.2,0.5 C33.2,0.5 38,5.3 38,11.3 L38,25.7 Z" id="Shape" />
                                    </g>
                                </g>
                            </svg>
                        </Link>
                    </div>
                </div>
            </header>
        </>
    );
}

export default HeaderComponent;