import { AUTH_URL, AUTH_PROFILE_URL } from "../../constants/EndpointConstants";
import { AuthenticationError } from "../../errors/AuthenticationError";
import { api, auth_api_authorized } from "../../http";
import type { LoginRequestDto } from "../../models/auth-service/LoginRequestDto";
import type { LoginResponseDto } from "../../models/auth-service/LoginResponseDto";
import type { RegisterRequestDto } from "../../models/auth-service/RegisterRequestDto";
import type { RefreshTokenRequestDto } from "../../models/auth-service/RefreshTokenRequestDto";
import type { UserDto } from "../../models/auth-service/UserDto";
import type { UpdateUserDto } from "../../models/auth-service/UserDto";

const TOKEN_STORAGE_KEY = "token";
const AUTH_CHANGED_EVENT = "auth:changed";

let _cachedUser: UserDto | null = null;
let _cacheTimestamp: number = 0;

const CACHE_TTL = 5 * 60 * 1000; // 5 минут

export async function loginAsync(loginRequest: LoginRequestDto) {
    const res = await api.post(`${AUTH_URL}/login`, loginRequest);

    if (!res.data) {
        return;
    }

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

    if (!res.data) {
        return;
    }

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

    if (!raw) {
        return null;
    }

    try {
        return JSON.parse(raw) as LoginResponseDto;
    } catch {
        localStorage.removeItem(TOKEN_STORAGE_KEY);
        return null;
    }
}

export function getAccessToken(): string | null {
    const tokens = getTokens();
    return tokens?.access_token ?? null;
}

export function getRefreshToken(): string | null {
    const tokens = getTokens();
    return tokens?.refresh_token ?? null;
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

export async function updateProfileAsync(profileData: UpdateUserDto): Promise<UserDto> {
    const response = await auth_api_authorized.put<UserDto>(AUTH_PROFILE_URL, profileData);

    _cachedUser = response.data;
    _cacheTimestamp = Date.now();

    notifyAuthChanged();

    return response.data;
}

export function getLoginErrorMessage(error: unknown): string {
    const status = getErrorStatus(error);

    if (status === 400 || status === 401) {
        return "Неверная электронная почта или пароль.";
    }

    if (!navigator.onLine) {
        return "Нет подключения к интернету. Проверьте соединение и попробуйте снова.";
    }

    return "Не удалось выполнить вход. Попробуйте ещё раз.";
}

export function getRegisterErrorMessage(error: unknown): string {
    const status = getErrorStatus(error);
    const payload = getErrorPayload(error);
    const text = payloadToText(payload);

    if (status === 409 || isDuplicateEmailError(text)) {
        return "Эта электронная почта уже используется. Попробуйте войти или укажите другую почту.";
    }

    if (isPasswordValidationError(text)) {
        return "Пароль недостаточно надежный. Используйте минимум 8 символов, заглавную и строчную букву, цифру и специальный символ.";
    }

    if (status === 400) {
        return text || "Проверьте правильность введенных данных.";
    }

    if (!navigator.onLine) {
        return "Нет подключения к интернету. Проверьте соединение и попробуйте снова.";
    }

    return "Не удалось зарегистрироваться. Попробуйте ещё раз.";
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

function getErrorStatus(error: unknown): number | undefined {
    if (typeof error !== "object" || error === null) {
        return undefined;
    }

    const axiosError = error as {
        response?: {
            status?: number;
        };
    };

    return axiosError.response?.status;
}

function getErrorPayload(error: unknown): unknown {
    if (typeof error !== "object" || error === null) {
        return error;
    }

    const axiosError = error as {
        response?: {
            data?: unknown;
        };
        data?: unknown;
    };

    return axiosError.response?.data ?? axiosError.data ?? error;
}

function payloadToText(payload: unknown): string {
    if (!payload) {
        return "";
    }

    if (typeof payload === "string") {
        return payload;
    }

    if (Array.isArray(payload)) {
        return payload.map(payloadToText).join(" ");
    }

    if (typeof payload !== "object") {
        return String(payload);
    }

    const errorPayload = payload as {
        message?: string;
        detail?: string;
        title?: string;
        errors?: unknown;
        code?: string;
        description?: string;
    };

    if (errorPayload.description) {
        return errorPayload.description;
    }

    if (errorPayload.message) {
        return errorPayload.message;
    }

    if (errorPayload.detail) {
        return errorPayload.detail;
    }

    if (errorPayload.title) {
        return errorPayload.title;
    }

    if (errorPayload.code) {
        return errorPayload.code;
    }

    if (Array.isArray(errorPayload.errors)) {
        return errorPayload.errors.map(payloadToText).join(" ");
    }

    if (typeof errorPayload.errors === "object" && errorPayload.errors !== null) {
        return Object.values(errorPayload.errors)
            .flat()
            .map(payloadToText)
            .join(" ");
    }

    return JSON.stringify(payload);
}

function isDuplicateEmailError(text: string): boolean {
    const normalized = text.toLowerCase();

    return (
        normalized.includes("duplicate") ||
        normalized.includes("already") ||
        normalized.includes("already exists") ||
        normalized.includes("email") && normalized.includes("taken") ||
        normalized.includes("email") && normalized.includes("exist") ||
        normalized.includes("username") && normalized.includes("taken") ||
        normalized.includes("duplicateemail") ||
        normalized.includes("duplicateusername") ||
        normalized.includes("почта") && normalized.includes("использ") ||
        normalized.includes("почта") && normalized.includes("существ")
    );
}

function isPasswordValidationError(text: string): boolean {
    const normalized = text.toLowerCase();

    return (
        normalized.includes("password") ||
        normalized.includes("пароль") ||
        normalized.includes("passwordtooshort") ||
        normalized.includes("passwordrequiresdigit") ||
        normalized.includes("passwordrequiresuppercase") ||
        normalized.includes("passwordrequireslower") ||
        normalized.includes("passwordrequiresnonalphanumeric")
    );
}