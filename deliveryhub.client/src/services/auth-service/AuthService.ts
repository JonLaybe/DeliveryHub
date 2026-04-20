import { AUTH_URL } from "../../constants/EndpointConstants";
import { api } from "../../http";
import type { LoginRequestDto } from "../../models/auth-service/LoginRequestDto";
import type { LoginResponseDto } from "../../models/auth-service/LoginResponseDto";

export async function loginAsync(loginRequest: LoginRequestDto) {
    const res = await api.post(`${AUTH_URL}/login`, loginRequest);

    console.log(res);

    if (!res.data)
        return;

    setResultTokens(res.data as LoginResponseDto);
}

export function getAccessToken(): string | null {
    const rl = localStorage.getItem('token');
    const access_token = rl ? JSON.parse(rl) as LoginResponseDto : null;

    if (access_token)
        return access_token.access_token;

    return null;
}

// TODO сделать проверку по времени токена
export function isAuthentication(): boolean {
    return getAccessToken() == null ? false : true;
}

function setResultTokens(loginRespons:LoginResponseDto) {
    localStorage.setItem('token', JSON.stringify(loginRespons));
}