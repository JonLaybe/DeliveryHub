import type { LoginResponseDto } from "../../models/auth-service/LoginResponseDto";

const TOKEN_STORAGE_KEY = "token";

export function setTokens(tokens: LoginResponseDto): void {
    localStorage.setItem(TOKEN_STORAGE_KEY, JSON.stringify(tokens));
}

export function getTokens(): LoginResponseDto | null {
    const raw = localStorage.getItem(TOKEN_STORAGE_KEY);
    if (!raw) return null;

    try {
        return JSON.parse(raw) as LoginResponseDto;
    } catch {
        // если вдруг в localStorage лежит мусор
        localStorage.removeItem(TOKEN_STORAGE_KEY);
        return null;
    }
}

export function getAccessToken(): string | null {
    return getTokens()?.access_token ?? null;
}

export function getRefreshToken(): string | null {
    return getTokens()?.refresh_token ?? null;
}

export function clearTokens(): void {
    localStorage.removeItem(TOKEN_STORAGE_KEY);
}