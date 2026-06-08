import { useEffect, useContext, type FC, useState } from "react";
import './ProductsComponent.scss';
import ProductCardComponent from "../../common/product-card/ProductCardComponent";
import { SearchContext } from "../../context/SearchContext";
import FiltersControlComponent from "../filters/FiltersControlComponent";
import { useNavigate } from "react-router-dom";
import chat_icon from "../../assets/chat.svg";
import { isAuthentication } from "../../services/auth-service/AuthService";

const ProductsComponent: FC = () => {
    const {
        products,
        searchBoxChangeHandler,
        clearFilters,
        setFiltersCount,
        serachProductsAndSetResults,
    } = useContext(SearchContext);

    const [isAuthenticated, setIsAuthenticated] = useState(isAuthentication());
    const navigate = useNavigate();

    useEffect(() => {
        clearFilters();
        searchBoxChangeHandler({ target: { value: '' } } as React.ChangeEvent<HTMLInputElement>);

        serachProductsAndSetResults({
            text: '',
        });

        setFiltersCount(0);
    }, []);

    useEffect(() => {
        const handleAuthChange = () => {
            setIsAuthenticated(isAuthentication());
        };
        
        window.addEventListener('auth:changed', handleAuthChange);
        
        return () => {
            window.removeEventListener('auth:changed', handleAuthChange);
        };
    }, []);

    return (
        <>
            <FiltersControlComponent />

            <div className="container" >
                <div className="list_products">
                    {
                        products?.products.map(prd => (
                            <ProductCardComponent product={prd} key={prd.id.toString()} />
                        ))
                    }
                </div>
            </div>
            
            {isAuthenticated && (
                <div className="chat-fab" onClick={() => navigate("/chat")}>
                    <img src={chat_icon} alt="chat" />
                </div>
            )}
        </>
    );
};

export default ProductsComponent;