import { Routes, Route, Navigate, useLocation } from "react-router-dom";
import ProductsComponent from "../products/ProductsComponent";
import OrderComponent from "../orders/OrderComponent";
import ProductPageComponent from "../products/ProductPageComponent";
import GroceryBasketComponent from "../grocery_basket/GroceryBasketComponent";
import ProfileComponent from "../auth/profile/ProfileComponent";
import PrivateRoute from "./PrivateRoute";
import ChatComponent from "../chat/ChatComponent";
import PaymentComponent from "../payments/PaymentComponent";
import ProfileEditComponent from "../auth/profile/ProfileEditComponent";

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
            <Route path="/payment" element={
                <PrivateRoute>
                    <PaymentComponent />
                </PrivateRoute>
            } />
            <Route path="/profile" element={
                <PrivateRoute>
                    <ProfileComponent />
                </PrivateRoute>
            } />
			<Route path="/profile/edit" element={
                <PrivateRoute>
                    <ProfileEditComponent  />
                </PrivateRoute>
            } />
			<Route path="/chat/:conversationId" element={
				<PrivateRoute>
					<ChatComponent />
				</PrivateRoute>
			} />
			<Route path="/chat" element={
				<PrivateRoute>
					<ChatComponent />
				</PrivateRoute>
			} />
            <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
    );
}