<script setup lang="ts">
import type { MenuItem } from '@/types/ui/menu';
import { mdiMenuRight, mdiOpenInNew } from '@/utils/icons';
import { PropType } from 'vue';
import { useRoute, useRouter } from 'vue-router';

const props = defineProps({
    subMenuList: {
        type: Array as PropType<MenuItem[]>,
        default: () => [],
    },
    isRail: {
        type: Boolean,
        default: true,
    },
});
// 路由
const route = useRoute();
const router = useRouter();

/**
 * 是否為當前頁面
 * @param {String} routeName 該頁面route名稱
 */
const isAcitve = (routeName: string) => route.name === routeName;

/**
 * 選單點擊觸發事件
 * @param {*} menu 該頁面route menu 物件
 */
const clickHandler = (menu: MenuItem) => {
    if (menu.isRedirect && menu.redirectUri) {
        window.open(menu.redirectUri, '_blank');
    } else {
        router.push(menu.path);
    }
};
</script>
<template>
    <template
        v-for="(s, sIdx) in props.subMenuList"
        :key="'s' + sIdx"
    >
        <v-list-item
            v-if="!s.subMenus || !s.subMenus.length"
            :class="{ 'no-icon': !s.icon }"
            :title="s.title"
            :value="s.id"
            color="nav-list-active"
            :active="isAcitve(s.routeName)"
            @click.prevent="clickHandler(s)"
        >
            <template #prepend>
                <v-icon
                    v-if="s.icon"
                    :icon="s.icon"
                    class="mr-2"
                />
            </template>
            <template #title="{ title }">
                <span class="text-body-1">{{ title }}</span>
            </template>
            <template #append>
                <v-icon
                    v-if="s.isRedirect"
                    class="ml-2"
                    :icon="mdiOpenInNew"
                />
            </template>
        </v-list-item>
        <template v-else>
            <!--浮動選單樣式-->
            <v-menu
                v-if="isRail"
                location="end"
            >
                <template #activator="{ props: sProps }">
                    <v-list-item
                        :class="{ 'no-icon': !s.icon }"
                        :prepend-icon="s.icon"
                        :append-icon="s.subMenus ? mdiMenuRight : undefined"
                        :title="s.title"
                        :value="s.routeName"
                        color="white"
                        v-bind="sProps"
                    >
                        <template #title="{ title }">
                            <span class="text-body-1">{{ title }}</span>
                            <v-icon
                                v-if="s.isRedirect"
                                class="ml-2"
                                :icon="mdiOpenInNew"
                            />
                        </template>
                    </v-list-item>
                </template>
                <v-list
                    bg-color="grey-darken-1"
                    :role="null"
                >
                    <SidebarTree
                        :sub-menu-list="s.subMenus"
                        :is-rail="props.isRail"
                    />
                </v-list>
            </v-menu>
            <!--固定選單樣式-->
            <v-list-group
                v-else
                :value="s.routeName"
                class="menu-group"
            >
                <template #activator="{ props: sProps }">
                    <v-list-item
                        :class="{ 'no-icon': !s.icon }"
                        v-bind="sProps"
                        :title="s.title"
                        :value="s.routeName"
                        active-class="list-group-active"
                    >
                        <template #prepend>
                            <v-icon
                                v-if="s.icon"
                                :icon="s.icon"
                                class="mr-2"
                            />
                        </template>
                        <template #title="{ title }">
                            <span class="text-body-1">{{ title }}</span>
                        </template>
                    </v-list-item>
                </template>

                <SidebarTree :sub-menu-list="s.subMenus" />
            </v-list-group>
        </template>
    </template>
</template>

<style lang="sass">
.v-list-item__prepend > .v-icon ~ .v-list-item__spacer
    width: 0
.v-list-item--active > .v-list-item__overlay
    opacity: 0 !important
.menu-group .v-list-group__items .v-list-item
    padding-inline-start: calc(var(--indent-padding) - 8px) !important
.list-group-active
    background-color: white !important
    color: rgb(var(--v-theme-primary)) !important
</style>
