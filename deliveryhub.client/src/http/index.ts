import axios from "axios";
import { BASE_URL, CATALOG_BASE_URL } from "../constants/EndpointConstants";
import { getAccessToken } from "../services/auth-service/AuthService";
import { AuthenticationError } from "../errors/AuthenticationError";

export const api = axios.create({
    baseURL: BASE_URL,
});

export const api_authorized = axios.create({
    baseURL: BASE_URL,
    withCredentials: true,
});

export const catalog_api = axios.create({
    baseURL: CATALOG_BASE_URL,
});

api_authorized.interceptors.request.use((config) => {
    const access_token = getAccessToken();

    if (!access_token)
        throw new AuthenticationError();

    config.headers.Authorization = `Bearer ${access_token}`;
    return config;
});
