import { themes, type ThemesKey } from '@/themes';
import { useStorage } from '@vueuse/core';
import { defineStore } from 'pinia';
import { computed, ref } from 'vue';
import { useRouter } from 'vue-router';
import { useDisplay } from 'vuetify';

const SYSTEM_NAME = import.meta.env.VITE_APP_TITLE;
const THEME_KEY = `${SYSTEM_NAME}_theme`;
const IS_DARK_KEY = `${SYSTEM_NAME}_isDark`;

const usePageStore = defineStore('page', () => {
    /**
     * 主題類型，直接使用 themes 的 key，統一管理
     */
    const themeType = Object.keys(themes).reduce<Record<string, string>>(
        (acc, key) => {
            acc[key] = key;
            return acc;
        },
        {},
    );

    const theme = useStorage(THEME_KEY, themeType.defaultLight); // 預設主題為 defaultLight
    const prefersDarkScheme = window.matchMedia('(prefers-color-scheme: dark)'); // 用於判斷系統是否為暗色模式
    const isDark = useStorage(IS_DARK_KEY, prefersDarkScheme.matches); // 用於控制主題是否為暗色模式

    const drawer = ref<boolean | null>(null); // 用於控制側邊欄開合
    const { mobile, sm } = useDisplay();
    const isMobile = computed(() => mobile.value);
    const isSm = computed(() => sm.value);
    const rail = ref(false); // 用於控制側邊欄是否為縮小模式

    const isLoading = ref(false); // 用於控制是否加載中

    /**
     * 設定主題
     * @param {string} newTheme - 新的主題
     */
    const setTheme = (newTheme: ThemesKey) => {
        theme.value = newTheme;
    };

    /**
     * 根據 isDark 設定主題
     * @param {string} themeName - 主題名稱
     */
    const setThemeFromIsDark = (themeName: ThemesKey): void => {
        const updatedTheme = isDark.value
            ? themeName.replace('Light', 'Dark')
            : themeName.replace('Dark', 'Light');
        setTheme(updatedTheme as ThemesKey);
    };

    /**
     * 切換暗色模式
     */
    const toggleDarkMode = () => {
        isDark.value = !isDark.value;
        setThemeFromIsDark(theme.value as ThemesKey);
    };

    /**
     * 設定側邊欄開合狀態
     * @param {boolean} newDrawer - 新的側邊欄開合狀態
     */
    const setDrawer = (newDrawer: boolean | null) => {
        drawer.value = newDrawer;
    };

    /**
     * 設定側邊欄是否為縮小模式
     * @param {boolean} newRail - 新的側邊欄縮小模式狀態
     */
    const setRail = (newRail: boolean) => {
        rail.value = newRail;
    };

    /**
     * 設定是否加載中
     * @param {boolean} data - 加載狀態
     */
    const setIsLoading = (data: boolean) => {
        isLoading.value = data;
    };

    const router = useRouter();

    /**
     * 導航到指定頁面
     * @param {string} pageName - 頁面名稱
     */
    const goPage = (pageName: string) => {
        router.push({ name: pageName });
    };

    return {
        themeType,
        theme,
        isDark,
        toggleDarkMode,
        setThemeFromIsDark,
        drawer,
        rail,
        isLoading,
        isMobile,
        isSm,
        setTheme,
        setDrawer,
        setRail,
        setIsLoading,
        goPage,
    };
});

export default usePageStore;
