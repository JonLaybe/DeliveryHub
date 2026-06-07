import axios from "axios";
import { BASE_URL, CATALOG_BASE_URL, AUTH_BASE_URL } from "../constants/EndpointConstants";
import { getAccessToken, refreshAsync } from "../services/auth-service/AuthService";
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

auth_api_authorized.interceptors.request.use((config) => {
    const access_token = getAccessToken();

    if (!access_token)
        throw new AuthenticationError();

    config.headers.Authorization = `Bearer ${access_token}`;
    return config;
});

auth_api_authorized.interceptors.response.use(
    (response) => response,
    async (error) => {
        const originalRequest = error.config;
        if (error.response?.status === 401 && error.config && !error.config._isRetry) {
            originalRequest._isRetry = true;
            try {
                await refreshAsync();
                return auth_api_authorized.request(originalRequest);
            } catch {
                console.log("Unauthorized");
            }
        }
        throw error;
    }
);

api_authorized.interceptors.request.use((config) => {
    const access_token = getAccessToken();

    if (!access_token)
        throw new AuthenticationError();

    config.headers.Authorization = `Bearer ${access_token}`;
    return config;
});

api_authorized.interceptors.response.use(
    (response) => response,
    async (error) => {
        const originalRequest = error.config;
        if (error.response?.status === 401 && error.config && !error.config._isRetry) {
            originalRequest._isRetry = true;
            try {
                await refreshAsync();
                return api_authorized.request(originalRequest);
            } catch {
                console.log("Unauthorized");
            }
        }
        throw error;
    }
);