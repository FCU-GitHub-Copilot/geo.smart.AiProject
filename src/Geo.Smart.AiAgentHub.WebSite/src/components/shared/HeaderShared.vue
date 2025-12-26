<script setup lang="ts">
import {
    onBeforeUnmount,
    onMounted,
    ref,
    computed,
} from 'vue';

import HeaderBarTitle from '@/components/shared/HeaderBarTitle.vue';
import useInactivityTimer from '@/stores/inactivityTimer';
import usePageStore from '@/stores/page';
import useUserStore from '@/stores/user';
import useProfileStore from '@/stores/profile';
import type { HeaderMenuItem } from '@/types/ui/menu';
import { logout } from '@/utils/auth';
import {
    mdiAccount,
    mdiChevronDown,
} from '@/utils/icons';

const pageStore = usePageStore();
const userStore = useUserStore();
const profileStore = useProfileStore();
const inactivityTimerStore = useInactivityTimer();
const userName = computed(() => profileStore.profile?.userName);

// 其他圖示按鈕列表
const otherMenuItems = ref<HeaderMenuItem[]>([]);

// 個人帳號 按鈕列表
const accountMenuItems = ref<HeaderMenuItem[]>([
    {
        title: '登出',
        onClick: () => {
            logout();
            pageStore.goPage('Login');
        },
    },
]);

// 按鈕事件
const handleMenuClick = (item: HeaderMenuItem) => {
    if (!item.onClick) {
        pageStore.goPage(item.routeName);
        return;
    }
    item.onClick();
};

onMounted(() => {
    inactivityTimerStore.initializeTimer(true);
    profileStore.getProfile();
});

onBeforeUnmount(() => {
    inactivityTimerStore.destoryTimer();
});

const isDev = import.meta.env.VITE_ENV === 'development';
</script>
<template>
    <v-app-bar>
        <v-app-bar-nav-icon
            v-if="userStore.isLogin"
            @click.stop="
                pageStore.setDrawer(!pageStore.drawer);
                pageStore.setRail(false);
            "
        />

        <v-app-bar-title>
            <HeaderBarTitle @title:click="pageStore.goPage('LlmMng')" />
        </v-app-bar-title>

        <div v-if="isDev">
            <!-- 開發模式下顯示倒數計時 -->
            {{ inactivityTimerStore.minutes }}:{{
                inactivityTimerStore.seconds
            }}
        </div>

        <template v-if="userStore.isLogin">
            <!--其他圖示按鈕列表-->
            <v-tooltip
                v-for="item in otherMenuItems"
                :key="item.title"
                :text="item.title"
                location="bottom"
            >
                <template #activator="{ props }">
                    <v-btn
                        v-bind="props"
                        :icon="item.icon"
                        @click="handleMenuClick(item)"
                    />
                </template>
            </v-tooltip>
            <!-- 登入後顯示帳號管理 -->
            <v-menu>
                <template #activator="{ props }">
                    <v-btn
                        v-if="pageStore.isSm || pageStore.isMobile"
                        v-bind="props"
                        variant="outlined"
                        :icon="mdiAccount"
                        class="mr-4"
                        title="帳號管理"
                    />
                    <div
                        v-else
                        class="d-flex flex-column pa-2 mr-4"
                    >
                        <v-btn
                            v-bind="props"
                            :append-icon="mdiChevronDown"
                            :prepend-icon="mdiAccount"
                            variant="outlined"
                        >
                            {{ userName || '使用者' }}
                        </v-btn>
                    </div>
                </template>
                <v-list bg-color="grey-darken-1">
                    <v-list-item
                        v-for="item in accountMenuItems"
                        :key="item.title"
                        @click="handleMenuClick(item)"
                    >
                        <v-list-item-title>
                            <v-icon
                                v-if="item.icon"
                                :icon="item.icon"
                                class="mr-2"
                            />
                            {{ item.title }}
                        </v-list-item-title>
                    </v-list-item>
                </v-list>
            </v-menu>
        </template>
    </v-app-bar>
</template>
