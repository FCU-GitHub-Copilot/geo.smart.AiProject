export interface User {
    id: string;
    userName: string;
    email?: string;
    roles: string[];
    isActive: boolean;
    createdDate: string;
    lastLoginDate?: string;
}

export interface AuthToken {
    accessToken: string;
    refreshToken: string;
    expiresIn: number;
    tokenType: string;
}

export interface LoginResponse {
    user: User;
    token: AuthToken;
}
