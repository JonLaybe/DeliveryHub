export interface UserDto {
    id: string;
    email: string;
    firstName: string;
    lastName: string;
    photoUrl: string;
    birthDate: string;
    phoneNumber: string;
    country: string;
    city: string;
    roles: string[];
}

export interface UpdateUserDto {
    firstName?: string | null;
    lastName?: string | null;
    photoUrl?: string | null;
    birthDate?: string | null;
    phoneNumber?: string | null;
    country?: string | null;
    city?: string | null;
}