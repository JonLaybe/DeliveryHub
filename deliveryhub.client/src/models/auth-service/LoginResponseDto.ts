type AccessToken = string & { readonly __brand: 'AccessToken' };
type RefreshToken = string & { readonly __brand: 'RefreshToken' };

export interface LoginResponseDto {
    access_token: AccessToken;
    token_type: string;
    expires_in: number;
    refresh_token: RefreshToken;
}