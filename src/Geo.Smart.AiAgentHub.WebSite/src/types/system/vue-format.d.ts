// Vue 3 相關型別擴展
declare module '*.vue' {
    import type { DefineComponent } from 'vue';
    const component: DefineComponent<{}, {}, any>;
    export default component;
}

// Vue Format 相關型別
export interface VueFormatOptions {
    currency?: string;
    locale?: string;
    precision?: number;
}
