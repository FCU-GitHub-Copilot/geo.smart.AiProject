import type { RoleTypeKey } from '@/types/models/role';

/**
 * 選單項目
 */
export interface MenuItem {
    /** 選單ID */
    id: string;
    /** 標題 */
    title: string;
    /** 路由名稱 */
    routeName: string;
    /** icon */
    icon?: string;
    /** 組件 */
    component?: (() => Promise<unknown>) | null;
    /** 路徑 */
    path: string;
    /** 角色鍵值陣列 */
    roles?: RoleTypeKey[];
    /** 子選單 */
    subMenus?: MenuItem[];
    /** 是否跳過註冊路由 */
    skipRegist?: boolean;
    /** 是否直接導向 */
    isRedirect?: boolean;
    /** 連結導向 */
    redirectUri?: string;
    /** 麵包屑頁面名稱是否透過query的name取代路由的name */
    isReplaceByQuery?: boolean;
}

/**
 * 頁首選單項目
 */
export type HeaderMenuItem = {
    /** 標題 */
    title: string;
    /** icon */
    icon?: string;
} & (
    | {
        /** 路由名稱 */
        routeName: string;
        /** Click事件 */
        onClick?: never;
    }
    | {
        /** 路由名稱 */
        routeName?: never;
        /** Click事件 */
        onClick: () => void;
    }
);
