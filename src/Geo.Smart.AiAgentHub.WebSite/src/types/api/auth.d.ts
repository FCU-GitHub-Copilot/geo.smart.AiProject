export interface LoginParams {
    userName: string;
    qoo: string;
    captchaId: string;
    captcha: string;
}

export interface RefreshTokenParams {
    accessToken: string;
    refreshToken: string;
    status: 0;
}

export interface ForgetPasswordParams {
    email: string;
    captcha: string;
    captchaId: string;
}
