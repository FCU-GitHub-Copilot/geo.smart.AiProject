import HeaderLogin from '@/components/login/HeaderLogin.vue';
import HeaderShared from '@/components/shared/HeaderShared.vue';
import RightSidebarShared from '@/components/shared/RightSidebarShared.vue';
import SidebarShared from '@/components/shared/SidebarShared.vue';
import defaultMenu from '@/router/menu';
import useUserStore from '@/stores/user';
import type { RoleTypeKey } from '@/types/models/role';
import type { MenuItem } from '@/types/ui/menu';
import { createRouter, createWebHistory } from 'vue-router';

// eslint-disable-next-line @typescript-eslint/no-explicit-any
interface RouteComponent extends Record<string, any> {
    default: () => Promise<unknown> | null;
    Header?: typeof HeaderShared | typeof HeaderLogin;
    Sidebar?: typeof SidebarShared;
    RightSidebar?: typeof RightSidebarShared;
}

interface RouteMeta {
    title: string;
    roles?: RoleTypeKey[];
    msg?: string;
    [key: string]: unknown;
    [key: symbol]: unknown;
}

interface RouteItem {
    path: string;
    name: string;
    components: RouteComponent;
    meta?: RouteMeta;
    children?: RouteItem[];
}

const routes: RouteItem[] = [];

/**
 * 生成路由
 * @param {Array} menu - 系統選單
 */
const generateRoutes = (menu: MenuItem[]) => menu.forEach((item: MenuItem) => {
    if (!item.skipRegist) {
        const route = {
            path: item.path,
            name: item.routeName,
            components: {
                default: item.component || (() => null),
                Header: HeaderShared,
                Sidebar: SidebarShared,
                RightSidebar: RightSidebarShared,
            },
            meta: {
                title: item.title,
                roles: item.roles,
                isReplaceByQuery: item.isReplaceByQuery,
            },
        };

        routes.push(route);
    }

    // 如果有 subMenus，遞迴生成路由
    if (item.subMenus && item.subMenus.length > 0) {
        generateRoutes(item.subMenus);
    }
});

generateRoutes(defaultMenu);

const router = createRouter({
    history: createWebHistory(import.meta.env.VITE_APP_PATH),
    routes: [
        ...routes,
        {
            path: '/',
            name: 'Login',
            components: {
                default: () => import('@/views/Login/IndexView.vue'),
                Header: HeaderLogin,
            },
            meta: {
                title: '登入',
                roles: [],
            },
        },
        {
            path: '/llmMng/add',
            name: 'LlmMngAdd',
            components: {
                default: () => import('@/views/LlmMng/AddView.vue'),
                Header: HeaderShared,
                Sidebar: SidebarShared,
                RightSidebar: RightSidebarShared,
            },
            meta: {
                title: '新增',
                roles: [],
            },
        },
        {
            path: '/llmMng/:id',
            name: 'LlmMngDetail',
            components: {
                default: () => import('@/views/LlmMng/DetailView.vue'),
                Header: HeaderShared,
                Sidebar: SidebarShared,
                RightSidebar: RightSidebarShared,
            },
            meta: {
                title: '詳細',
                roles: [],
            },
        },
        {
            path: '/mcpServerMng/add',
            name: 'McpServerMngAdd',
            components: {
                default: () => import('@/views/McpServerMng/AddView.vue'),
                Header: HeaderShared,
                Sidebar: SidebarShared,
                RightSidebar: RightSidebarShared,
            },
            meta: {
                title: '新增',
                roles: [],
            },
        },
        {
            path: '/mcpServerMng/:id',
            name: 'McpServerMngDetail',
            components: {
                default: () => import('@/views/McpServerMng/DetailView.vue'),
                Header: HeaderShared,
                Sidebar: SidebarShared,
                RightSidebar: RightSidebarShared,
            },
            meta: {
                title: '詳細',
                roles: [],
            },
        },
        {
            path: '/AiProjectMng/add',
            name: 'AiProjectMngAdd',
            components: {
                default: () => import('@/views/ProjectMng/AddView.vue'),
                Header: HeaderShared,
                Sidebar: SidebarShared,
                RightSidebar: RightSidebarShared,
            },
            meta: {
                title: '新增',
                roles: [],
            },
        },
        {
            path: '/AiProjectMng/:id',
            name: 'AiProjectMngDetail',
            components: {
                default: () => import('@/views/ProjectMng/DetailView.vue'),
                Header: HeaderShared,
                Sidebar: SidebarShared,
                RightSidebar: RightSidebarShared,
            },
            meta: {
                title: '詳細',
                roles: [],
            },
        },
        {
            path: '/403',
            name: 'Page403',
            components: {
                default: () => import('@/views/ErrorPageView.vue'),
            },
            meta: {
                title: '403',
                msg: '抱歉，你沒有這個頁面的權限。',
            },
        },
        {
            path: '/:pathMatch(.*)*',
            name: 'Page404',
            components: {
                default: () => import('@/views/ErrorPageView.vue'),
            },
            meta: {
                title: '404',
                msg: '抱歉，你要找的頁面不存在。',
            },
        },
    ],
    scrollBehavior(to, _, savedPosition) {
        // 如果有保存的位置（例如：瀏覽器後退）
        if (savedPosition) {
            return savedPosition;
        }

        // 如果有 hash，滾動到對應元素
        if (to.hash) {
            return { el: to.hash };
        }

        // 預設滾動到頂部
        return { top: 0 };
    },
});

router.beforeEach((to, _, next) => {
    // 使用者資料
    const userStore = useUserStore();

    if (to.name === 'Login' || to.name === 'ChangeQoo') {
        next();
        return;
    }

    // * 未登入或無角色權限
    const { isLogin, role } = userStore;
    if (!isLogin || !role) {
        next({ name: 'Login', query: { redirectUri: to.path }, replace: true });
        return;
    }

    // * 有登入確認是否有權限
    if (!userStore.hasAccess(to.meta.roles as RoleTypeKey[])) {
        if (to.name === 'Home') {
            next({ path: userStore.homePath });
            return;
        }
        next({ name: 'Page403' });
        return;
    }

    next();
});

router.beforeResolve((_, __, next) => {
    next();
});

router.afterEach(() => {});

export default router;
