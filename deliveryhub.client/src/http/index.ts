import axios from "axios";
import { toast } from "react-hot-toast";
import { BASE_URL, CATALOG_BASE_URL, AUTH_BASE_URL } from "../constants/EndpointConstants";
import { clearTokens, getAccessToken, refreshAsync } from "../services/auth-service/AuthService";
import { AuthenticationError } from "../errors/AuthenticationError";

export const api = axios.create({
    baseURL: BASE_URL,
});

export const api_authorized = axios.create({
    baseURL: BASE_URL,
});

export const auth_api_authorized = axios.create({
    baseURL: AUTH_BASE_URL,
});

export const catalog_api = axios.create({
    baseURL: CATALOG_BASE_URL,
});

let isSessionExpiredToastShown = false;

function notifySessionExpiredOnce() {
    if (isSessionExpiredToastShown) {
        return;
    }

    isSessionExpiredToastShown = true;

    toast.error("Сессия истекла. Пожалуйста, войдите снова.");

    setTimeout(() => {
        isSessionExpiredToastShown = false;
    }, 3000);
}

function handleRefreshFailed(error: unknown): never {
    clearTokens();
    notifySessionExpiredOnce();
    throw error;
}

auth_api_authorized.interceptors.request.use((config) => {
    const access_token = getAccessToken();

    if (!access_token) {
        throw new AuthenticationError();
    }

    config.headers.Authorization = `Bearer ${access_token}`;
    return config;
});

auth_api_authorized.interceptors.response.use(
    (response) => response,
    async (error) => {
        const originalRequest = error.config;

        if (error.response?.status === 401 && originalRequest && !originalRequest._isRetry) {
            originalRequest._isRetry = true;

            try {
                await refreshAsync();
                return auth_api_authorized.request(originalRequest);
            } catch (refreshError) {
                handleRefreshFailed(refreshError);
            }
        }

        throw error;
    }
);

api_authorized.interceptors.request.use((config) => {
    const access_token = getAccessToken();

    if (!access_token) {
        throw new AuthenticationError();
    }

    config.headers.Authorization = `Bearer ${access_token}`;
    return config;
});

api_authorized.interceptors.response.use(
    (response) => response,
    async (error) => {
        const originalRequest = error.config;

        if (error.response?.status === 401 && originalRequest && !originalRequest._isRetry) {
            originalRequest._isRetry = true;

            try {
                await refreshAsync();
                return api_authorized.request(originalRequest);
            } catch (refreshError) {
                handleRefreshFailed(refreshError);
            }
        }

        throw error;
    }
);