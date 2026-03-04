import { Routes, Route, Navigate } from "react-router-dom";
import ProductComponent from "../products/ProductComponent";
import OrderComponent from "../orders/OrderComponent";

export default function RoutingComponent() {
    return (
        <Routes>
            <Route path="/" element={<ProductComponent />} />
            <Route path="/orders" element={<OrderComponent />} />
            <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
    );
}