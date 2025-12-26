import defaultMenu from '@/router/menu';
import { type RoleTypeKey, roleType } from '@/types/models/role';
import type { MenuItem } from '@/types/ui/menu';
import { useStorage } from '@vueuse/core';
import * as jose from 'jose';
import { defineStore } from 'pinia';
import { computed, ref, watch } from 'vue';

// 系統名稱
const SYSTEM_NAME = import.meta.env.VITE_APP_TITLE;
const TOKEN_KEY = `${SYSTEM_NAME}_token`;
const REFRESH_TOKEN_KEY = `${SYSTEM_NAME}_refreshToken`;

const useUserStore = defineStore('user', () => {
    const storeToken = useStorage<string | null>(TOKEN_KEY, null);
    const storeRefreshToken = useStorage<string | null>(
        REFRESH_TOKEN_KEY,
        null,
    );
    const role = ref<RoleTypeKey | null>(null);

    /**
     * 設定角色
     * @param {string} value - 要設定的角色值
     */
    const setRole = (value: RoleTypeKey | null) => {
        role.value = value;
    };

    /**
     * 解碼 JWT Token
     * @param {string} jwtToken - 要解碼的 JWT Token
     * @returns {object} 解碼後的 JWT Token 資料
     */
    const jwtDecode = (jwtToken: string) => jose.decodeJwt(jwtToken);

    if (storeToken.value) {
        setRole(jwtDecode(storeToken.value).role as RoleTypeKey);
    }

    watch(storeToken, (value) => {
        if (value) {
            setRole(jwtDecode(value).role as RoleTypeKey);
        } else {
            setRole(null);
        }
    });

    /**
     * 判斷是否登入
     * @returns {boolean} 是否登入
     */
    const isLogin = computed(() => !!storeToken.value);

    /**
     * 設定 Token 的值
     * @param {string} value - 要設定的 Token 值
     */
    const setToken = (value: string | null) => {
        storeToken.value = value;
    };

    /**
     * 設定刷新 Token 的值
     * @param {string} value - 要設定的刷新 Token 值
     */
    const setRefreshToken = (value: string | null) => {
        storeRefreshToken.value = value;
    };

    /**
     * 判斷是否有權限
     * @param {Array<string>} roleKeys - 角色鍵值陣列
     * @returns {boolean} 是否有權限
     */
    const hasAccess = (roleKeys: RoleTypeKey[]) => {
        if (!roleKeys?.length) return true;
        const roleIds = roleKeys.map((key) => roleType[key]);
        return role.value
            ? roleIds.includes(roleType[role.value])
            : false;
    };

    /**
     * 根據角色設定選單項目
     * @param {object} menuItem - 選單項目
     * @returns {object|null} 設定後的選單項目或 null
     */
    const setMenuItemByRole = (menuItem: MenuItem): MenuItem | null => {
        if (!menuItem.subMenus?.length && !menuItem.roles) {
            return menuItem;
        }

        // 檢查是否有權限訪問
        const isAccessible = hasAccess(menuItem.roles || []);

        if (!menuItem.subMenus?.length) {
            return isAccessible ? menuItem : null;
        }

        const subMenus = menuItem.subMenus
            .map((secound) => setMenuItemByRole(secound))
            .filter((secound): secound is MenuItem => secound !== null);

        return isAccessible
            ? {
                ...menuItem,
                subMenus,
            }
            : null;
    };

    /**
     * 取得有權限的選單
     * @returns {Array<object>} 有權限的選單
     */
    const menu = computed(() => defaultMenu
        .map((first) => setMenuItemByRole(first))
        .filter((secound) => secound));

    /**
     * 根據登入角色導向首頁
     * @returns {string} 導向的首頁路徑
     */
    const homePath = computed(() => (role.value ? '/llmMng' : '/'));

    return {
        menu,
        token: storeToken,
        refreshToken: storeRefreshToken,
        isLogin,
        role,
        systemName: SYSTEM_NAME,
        setToken,
        setRefreshToken,
        setRole,
        jwtDecode,
        hasAccess,
        homePath,
    };
});

export default useUserStore;
