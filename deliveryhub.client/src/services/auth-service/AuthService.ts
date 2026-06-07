import { AUTH_URL, AUTH_PROFILE_URL } from "../../constants/EndpointConstants";
import { AuthenticationError } from "../../errors/AuthenticationError";
import { api, api_authorized, auth_api_authorized  } from "../../http";
import type { LoginRequestDto } from "../../models/auth-service/LoginRequestDto";
import type { LoginResponseDto } from "../../models/auth-service/LoginResponseDto";
import type { RegisterRequestDto } from "../../models/auth-service/RegisterRequestDto";
import type { RefreshTokenRequestDto } from "../../models/auth-service/RefreshTokenRequestDto";
import type { UserDto } from "../../models/auth-service/UserDto";

const TOKEN_STORAGE_KEY = "token";
const AUTH_CHANGED_EVENT = "auth:changed";
let _cachedUser: UserDto | null = null;
let _cacheTimestamp: number = 0;
const CACHE_TTL = 5 * 60 * 1000; // 5 минут

export async function loginAsync(loginRequest: LoginRequestDto) {
    const res = await api.post(`${AUTH_URL}/login`, loginRequest);

    if (!res.data)
        return;

    setResultTokens(res.data as LoginResponseDto);
}

export async function refreshAsync() {
    const refreshToken = getRefreshToken();

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
    const refreshToken = getRefreshToken();

    try {
        if (refreshToken) {
            const body: RefreshTokenRequestDto = { refreshToken };
            await api.post(`${AUTH_URL}/logout`, body);
        }
    } finally {
        clearTokens();
        _cachedUser = null;
        _cacheTimestamp = 0;
    }
}

export async function getCurrentUser(forceRefresh: boolean = false): Promise<UserDto> {
    if (!forceRefresh && _cachedUser && (Date.now() - _cacheTimestamp) < CACHE_TTL) {
        return _cachedUser;
    }
    
    try {
        const response = await auth_api_authorized.get<UserDto>(AUTH_PROFILE_URL);
        _cachedUser = response.data;
        _cacheTimestamp = Date.now();
        return response.data;
    } catch (error) {
        if (_cachedUser) {
            console.warn("Using cached user data due to API error", error);
            return _cachedUser;
        }
        throw error;
    }
}

export function getTokens(): LoginResponseDto | null {
    const raw = localStorage.getItem(TOKEN_STORAGE_KEY);
    if (!raw) return null;

    try {
        return JSON.parse(raw) as LoginResponseDto;
    } catch {
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

export function isAuthentication(): boolean {
    return getAccessToken() !== null;
}

export function clearTokens() {
    localStorage.removeItem(TOKEN_STORAGE_KEY);
    notifyAuthChanged();
}

export function onAuthChanged(handler: () => void) {
    window.addEventListener(AUTH_CHANGED_EVENT, handler);
    return () => window.removeEventListener(AUTH_CHANGED_EVENT, handler);
}

function setResultTokens(loginResponse: LoginResponseDto) {
    localStorage.setItem(TOKEN_STORAGE_KEY, JSON.stringify(loginResponse));
    _cachedUser = null;
    _cacheTimestamp = 0;
    notifyAuthChanged();
}

function notifyAuthChanged() {
    window.dispatchEvent(new Event(AUTH_CHANGED_EVENT));
}