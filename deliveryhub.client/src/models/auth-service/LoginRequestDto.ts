export type Password = string & { readonly __brand: unique symbol };

export interface LoginRequestDto {
    email: string;
    password: Password;
}