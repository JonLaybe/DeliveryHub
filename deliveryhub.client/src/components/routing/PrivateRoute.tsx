import { Navigate, useLocation } from "react-router-dom";
import { isAuthentication } from "../../services/auth-service/AuthService";
import type { ReactNode } from "react";

const PrivateRoute = ({ children }: { children: ReactNode }) => {
    const location = useLocation();

    if (!isAuthentication())
        <Navigate to="/" replace />

    if (location.pathname === "/payment") {
        const state = location.state as { fromBasket?: boolean } | null;

        if (!state?.fromBasket) {
            return <Navigate to="/" replace />;
        }
    }

    return children;
}

export default PrivateRoute;