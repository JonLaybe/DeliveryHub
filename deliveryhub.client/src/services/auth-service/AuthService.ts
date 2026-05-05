import { AUTH_URL } from "../../constants/EndpointConstants";
import { AuthenticationError } from "../../errors/AuthenticationError";
import { api } from "../../http";
import type { LoginRequestDto } from "../../models/auth-service/LoginRequestDto";
import type { LoginResponseDto } from "../../models/auth-service/LoginResponseDto";

export async function loginAsync(loginRequest: LoginRequestDto) {
    const res = await api.post(`${AUTH_URL}/login`, loginRequest);

    if (!res.data)
        return;

    setResultTokens(res.data as LoginResponseDto);
}

export async function refreshAsync() {
    let refreshToken = getRefreshToken();

    if (!refreshToken)
        throw new AuthenticationError();

    const res = await api.post(`${AUTH_URL}/refresh`, { refreshToken });
    
    if (!res.data)
        throw new AuthenticationError();

    setResultTokens(res.data as LoginResponseDto);
}

export function getAccessToken(): string | null {
    return localStorage.getItem('access_token');
}

export function getRefreshToken(): string | null {
    const refresh_token = localStorage.getItem('refresh_token');

    localStorage.removeItem('refresh_token');
    
    return refresh_token;
}

// TODO сделать проверку по времени токена
export function isAuthentication(): boolean {
    return getAccessToken() == null ? false : true;
}

function setResultTokens(loginRespons:LoginResponseDto) {
    localStorage.setItem('access_token', loginRespons.access_token);
    localStorage.setItem('refresh_token', loginRespons.refresh_token);
}