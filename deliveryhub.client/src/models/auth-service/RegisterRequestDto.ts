import type { Password } from "./LoginRequestDto";

export interface RegisterRequestDto {
    email: string;
    password: Password;
}