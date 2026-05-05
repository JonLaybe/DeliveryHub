import { AUTH_URL } from "../../constants/EndpointConstants";
import { AuthenticationError } from "../../errors/AuthenticationError";
import { api } from "../../http";
import type { LoginRequestDto } from "../../models/auth-service/LoginRequestDto";
import type { LoginResponseDto } from "../../models/auth-service/LoginResponseDto";
import type { RegisterRequestDto } from "../../models/auth-service/RegisterRequestDto";
import type { RefreshTokenRequestDto } from "../../models/auth-service/RefreshTokenRequestDto";

const TOKEN_STORAGE_KEY = "token";


export async function loginAsync(loginRequest: LoginRequestDto) {
    const res = await api.post(`${AUTH_URL}/login`, loginRequest);

    if (!res.data)
        return;

    setResultTokens(res.data as LoginResponseDto);
}

export async function refreshAsync() {
    const refreshToken = getRefreshToken();

    // если refresh-token отсутствует — это именно НЕ-аутентифицированный юзер
    if (!refreshToken) {
        throw new AuthenticationError();
    }

    try {
        const body: RefreshTokenRequestDto = { refreshToken };
        const res = await api.post(`${AUTH_URL}/refresh`, body);

        if (!res.data) {
            clearTokens();
            throw new AuthenticationError();
        }

        setResultTokens(res.data as LoginResponseDto);
    } catch (e) {
        // если refresh упал (401/500/сеть) — считаем, что сессии больше нет
        clearTokens();
        throw new AuthenticationError(e);
    }
}

export async function registerAsync(registerRequest: RegisterRequestDto) {
    const res = await api.post(`${AUTH_URL}/register`, registerRequest);

    if (!res.data) return;

    setResultTokens(res.data as LoginResponseDto);
}

export async function logoutAsync() {
    // logout для JWT: отзываем refresh_token, access сам протухнет
    const refreshToken = getRefreshToken();

    try {
        if (refreshToken) {
            const body: RefreshTokenRequestDto = { refreshToken };
            await api.post(`${AUTH_URL}/logout`, body);
        }
    } finally {
        clearTokens();
    }
}

export function getTokens(): LoginResponseDto | null {
    const raw = localStorage.getItem(TOKEN_STORAGE_KEY);
    if (!raw) return null;

    try {
        return JSON.parse(raw) as LoginResponseDto;
    } catch {
        // если кто-то случайно руками или кодом положил мусор — нужно почистить
        localStorage.removeItem(TOKEN_STORAGE_KEY);
        return null;
    }
}

export function getAccessToken(): string | null {
    const t = getTokens();
    return t?.access_token ?? null;
}

export function getRefreshToken(): string | null {
    const t = getTokens();
    return t?.refresh_token ?? null;
}

// TODO сделать проверку по времени токена
export function isAuthentication(): boolean {
    return getAccessToken() == null ? false : true;
}

export function clearTokens() {
    localStorage.removeItem(TOKEN_STORAGE_KEY);
}

function setResultTokens(loginResponse: LoginResponseDto) {
    localStorage.setItem(TOKEN_STORAGE_KEY, JSON.stringify(loginResponse));
}
