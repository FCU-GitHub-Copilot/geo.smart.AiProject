/// <reference types="vite/client" />

interface ImportMetaEnv {
    readonly VITE_APP_TITLE: string;
    readonly VITE_APP_PATH: string;
    readonly VITE_APP_API: string;
    readonly NODE_ENV: string;
}

// 為全域 nonce 變數提供型別宣告
declare const __VUETIFY_NONCE__: string;

declare module '*.vue' {
    import type { DefineComponent } from 'vue';

    const component: DefineComponent<
        Record<string, unknown>,
        Record<string, unknown>,
        unknown
    >;
    export default component;
}

declare module '@smart/vue-format';
