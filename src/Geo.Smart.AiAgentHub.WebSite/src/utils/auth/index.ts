import {
    apiLogin,
    apiRefreshToken,
} from '@/api';
import usePageStore from '@/stores/page';
import useProfileStore from '@/stores/profile';
import useSnackbarStore from '@/stores/snackbar';
import useUserStore from '@/stores/user';
import type { LoginParams, RefreshTokenParams } from '@/types/api/auth';

/**
 * 登出
 */
export const logout = async () => {
    const { setToken, setRefreshToken } = useUserStore();
    const { setProfile } = useProfileStore();

    setToken(null);
    setRefreshToken(null);
    setProfile(null);
};

interface CustomError extends Error {
    code?: number;
    token?: string;
}

interface LoginFuncParams {
    params?: LoginParams;
    token?: string;
    succCallback?: () => void;
    errCallback?: (error: Error) => void;
}

/**
 * 登入
 * @param {object} params - 登入參數
 * @param {string} params.userName - 使用者名稱
 * @param {string} params.qoo - 使用者密碼
 * @param {string} params.captchaId - 驗證碼 ID
 * @param {string} params.captcha - 驗證碼
 * @param {string} token - token
 * @param {Function} succCallback - 成功 callback
 * @param {Function} errCallback - 失敗 callback
 * @returns {Promise<void>}
 */
export const login = async ({
    params = {
        userName: '',
        qoo: '',
        captchaId: '',
        captcha: '',
    },
    token,
    succCallback,
    errCallback,
}: LoginFuncParams) => {
    const { setToken, setRefreshToken } = useUserStore();
    const { setIsLoading } = usePageStore();
    const { errSnack } = useSnackbarStore();

    if (token) {
        setToken(token);
        if (succCallback) succCallback();
        return;
    }

    try {
        setIsLoading(true);
        const { data } = await apiLogin(params);
        if (!data.success) {
            /**
             * 0:尚未填寫驗證碼,
             * 1:帳號已鎖定,
             * 2:登入失敗,
             * 3:未通過二階段驗證,
             * 4:不允許登入,
             * 5:驗證碼錯誤,
             */
            let message = '';
            switch (data.data.status) {
                case 0:
                    message = '尚未填寫驗證碼';
                    break;
                case 1:
                    message = '帳號已鎖定';
                    break;
                case 2:
                    message = '登入失敗';
                    break;
                case 3:
                    message = '未通過二階段驗證';
                    break;
                case 4:
                    message = '不允許登入';
                    break;
                case 5:
                    message = '需強制修改密碼';
                    break;
                case 6:
                    message = '驗證碼不符';
                    break;
                default:
                    break;
            }
            const error: CustomError = new Error(message);
            error.code = data.data.status;
            error.token = data.data.token; // 用於強制修改密碼
            throw error;
        }
        setToken(data.data.accessToken);
        setRefreshToken(data.data.refreshToken);
        if (succCallback) succCallback();
    } catch (error) {
        if (error instanceof Error) {
            errSnack(error.message);
        } else {
            errSnack('登入失敗');
        }
        if (errCallback) errCallback(error as Error);
    } finally {
        setIsLoading(false);
    }
};

/**
 * 刷新 token
 */
export const refreshToken = async () => {
    const userStore = useUserStore();

    if (!userStore.token || !userStore.refreshToken) return false;

    const params: RefreshTokenParams = {
        accessToken: userStore.token,
        refreshToken: userStore.refreshToken,
        status: 0,
    };

    try {
        const { data } = await apiRefreshToken(params);
        if (data.success) {
            userStore.setToken(data.data.accessToken);
            userStore.setRefreshToken(data.data.refreshToken);
        }
        return data.success;
    } catch {
        return false;
    }
};

/**
 * 忘記密碼錯誤訊息
 * @returns {{errorMessage: (function(*): string)}}
 */
export const usePasswordErrorMessage = () => {
    const errorMessage = (code: number) => {
        let message = '';
        switch (code) {
            case 0:
                message = '密碼更新失敗';
                break;
            case 1:
                message = '新密碼未輸入';
                break;
            case 2:
                message = '新密碼與確認新密碼不相同';
                break;
            case 3:
                message = '超過有效時間';
                break;
            case 4:
                message = '帳號不存在';
                break;
            case 5:
                message = '前三次密碼相同';
                break;
            case 6:
                message = 'Email重複';
                break;
            case 7:
                message = 'Email必填';
                break;
            case 8:
                message = 'Email格式錯誤';
                break;
            case 9:
                message = '密碼必填';
                break;
            case 10:
                message = '密碼格式錯誤';
                break;
            case 11:
                message = '寄發驗證信失敗';
                break;
            case 12:
                message = '無驗證碼紀錄';
                break;
            case 13:
                message = '驗證碼檢核失敗';
                break;
            case 14:
                message = '註冊帳號失敗';
                break;
            case 15:
                message = '註冊驗證失敗';
                break;
            case 16:
                message = '忘記密碼失敗';
                break;
            case 17:
                message = ' 驗證碼不符';
                break;
            default:
                break;
        }

        return message;
    };

    return { errorMessage };
};
