import useDialogStore from '@/stores/dialog';
import useUserStore from '@/stores/user';
import axios from 'axios';
import qs from 'qs';

const isDev = import.meta.env.VITE_ENV === 'development';
const baseURL = isDev ? '' : import.meta.env.VITE_APP_API;

const request = axios.create({
    baseURL,
    // .net core 陣列參數不需要[]
    paramsSerializer: {
        serialize: (params) => qs.stringify(params, { indices: false }),
    },
});

request.interceptors.request.use(
    (config) => {
        if (
            !isDev
            && typeof config.url === 'string'
            && /^\/Api/.test(config.url)
        ) {
            config.url = config.url.replace(/^\/Api/, '');
        }
        const userStore = useUserStore();
        if (userStore.token) {
            config.headers.Authorization = `Bearer ${userStore.token}`;
        }
        return config;
    },
    (err) => Promise.reject(err),
);

request.interceptors.response.use(
    (response) => {
        if (
            response.status === 200
            || response.statusText.toLowerCase() === 'ok'
        ) {
            return response;
        }

        // 兼容 blob 下載出錯 json 提示
        if (
            response.request.responseType === 'blob'
            && response.data instanceof Blob
            && response.data.type
            && response.data.type.toLowerCase().includes('json')
        ) {
            return new Promise((resolve, reject) => {
                const reader = new FileReader();
                reader.onload = () => {
                    if (typeof reader.result === 'string') {
                        response.data = JSON.parse(reader.result);
                    } else {
                        // Handle unexpected result type
                        response.data = {};
                    }
                    resolve(Promise.reject(response.data.msg));
                };

                reader.onerror = () => {
                    reject(response.data.msg);
                };
                reader.readAsText(response.data);
            });
        }

        return response;
    },
    (err) => {
        const handle401 = () => {
            const userStore = useUserStore();
            const { setToken, setRefreshToken, homePath } = userStore;
            const { openDialog, toggleDialog } = useDialogStore();
            openDialog({
                title: '登入逾時',
                content: '請重新登入',
                submitAction: () => {
                    toggleDialog(false);
                    setToken(null);
                    setRefreshToken(null);
                    const redirectUri = window.location.pathname || homePath;
                    window.location.href = `/?redirectUri=${redirectUri}`;
                },
            });


            err.message = '登入逾時';
        };

        const responseStatus = err.response?.status;
        const responseData = err.response?.data || {};
        switch (responseStatus) {
            case 401:
                if (window.location.pathname !== '/') handle401();
                break;
            case 403:
                window.location.href = '/403';
                break;
            case 500:
            // window.location.href = '/500';
                break;
            default:
                err.message = responseData.title || responseData.message || err.message;
                break;
        }
        return Promise.reject(err);
    },
);

export default request;
