import { Routes, Route, Navigate, useLocation } from "react-router-dom";
import ProductsComponent from "../products/ProductsComponent";
import OrderComponent from "../orders/OrderComponent";
import ProductPageComponent from "../products/ProductPageComponent";
import GroceryBasketComponent from "../grocery_basket/GroceryBasketComponent";
import ProfileComponent from "../auth/profile/ProfileComponent";
import PrivateRoute from "./PrivateRoute";

export default function RoutingComponent() {
    const location = useLocation();
    return (
        <Routes>
            <Route path="/" element={<ProductsComponent key={location.key} />} />
            <Route path="/product/:id" element={<ProductPageComponent />} />
            <Route path="/grocery_basket" element={<GroceryBasketComponent />} />
            <Route path="/orders" element={
                <PrivateRoute>
                    <OrderComponent />
                </PrivateRoute>
            } />
            <Route path="/profile" element={
                <PrivateRoute>
                    <ProfileComponent />
                </PrivateRoute>
            } />
            <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
    );
}