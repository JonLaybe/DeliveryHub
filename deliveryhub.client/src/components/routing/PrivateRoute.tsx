import { Navigate } from "react-router-dom";
import { isAuthentication } from "../../services/auth-service/AuthService";
import type { ReactNode } from "react";

const PrivateRoute = ({ children }: { children: ReactNode }) => {
    return isAuthentication() ? children : <Navigate to="/" replace />;
}

export default PrivateRoute;