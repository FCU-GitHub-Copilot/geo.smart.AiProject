<script setup lang="ts">
import { computed, ref, toRefs } from 'vue';
import { useRoute } from 'vue-router';

import usePageStore from '@/stores/page';
import useUserStore from '@/stores/user';
import type { MenuItem } from '@/types/ui/menu';
import SidebarTree from './SidebarTree.vue';

// 頁面狀態
const pageStore = usePageStore();
// 個人資訊
const userStore = useUserStore();
// 路由
const route = useRoute();

// 選單
const { menu } = toRefs(userStore);

// 選單開合控制
const menuOpen = ref<string[]>([]);

/**
 * 設定要開合的子選單
 * @param {Array} submenu 子選單
 * @param {Array} parentRouteNames 父選單的Route名稱
 */
const setDefaultMenuOpen = (
    submenu: MenuItem[],
    parentRouteNames: string[],
): boolean => submenu?.some((s) => {
    if (s.routeName === route.name) {
        menuOpen.value = [...parentRouteNames];
        return true;
    }
    if (s.subMenus && s.subMenus.length > 0) {
        return setDefaultMenuOpen(s.subMenus, [
            ...parentRouteNames,
            s.routeName,
        ]);
    }
    return false;
});

// 設定預設要開合的選單
setDefaultMenuOpen(
    (menu.value || []).filter((item): item is MenuItem => item !== null),
    [],
);

const navProps = computed(() => ({
    rail: pageStore.rail,
    prop: pageStore.isMobile ? 'temporary' : 'permanent',
}));
</script>

<template>
    <v-navigation-drawer
        v-model="pageStore.drawer"
        v-bind="navProps"
    >
        <v-divider />

        <v-list
            v-model:opened="menuOpen"
            density="compact"
            nav
        >
            <SidebarTree
                :sub-menu-list="
                    menu?.filter((item): item is MenuItem => item !== null)
                "
                :is-rail="pageStore.rail"
            />
        </v-list>
    </v-navigation-drawer>
</template>
<style lang="sass">
.v-list--nav
    & > .v-list-item > .v-list-item__content > .v-list-item-title
        font-size: 1rem
    & > .v-list-group > .v-list-item > .v-list-item__content > .v-list-item-title
        font-size: 1rem
</style>
