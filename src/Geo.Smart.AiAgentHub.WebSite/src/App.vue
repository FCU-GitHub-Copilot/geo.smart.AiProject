<script setup lang="ts">
import { FullScreenLoading as SFullLoading } from '@smart/vue-loading';
import { SnackBar as SSnackBar } from '@smart/vue-snackbar';
import { watch } from 'vue';
import { RouterView, useRoute, useRouter } from 'vue-router';

import useDialogStore from '@/stores/dialog';
import usePageStore from '@/stores/page';
import useSnackbarStore from '@/stores/snackbar';
import useUserStore from '@/stores/user';

const pageStore = usePageStore();
const { dialog } = useDialogStore();
const snackbarStore = useSnackbarStore();
const userStore = useUserStore();

const route = useRoute();
const router = useRouter();

watch(
    () => userStore.isLogin,
    (val) => {
        // 未登入時，導向登入頁
        if (!val && route.name !== 'Login') {
            const pathQuery = window.location.search || '';
            const redirectUri = `${window.location.pathname}${pathQuery}` || userStore.homePath;

            router.push({
                name: 'Login',
                query: { redirectUri },
                replace: true,
            });
        }
    },
);
</script>
<template>
    <v-app :theme="pageStore.theme">
        <!--系統頁首-->
        <RouterView name="Header" />

        <!--系統選單-->
        <RouterView name="Sidebar" />

        <v-main
            color="background"
            class="position-relative pt-print-0 pl-print-0"
        >
            <RouterView v-slot="{ Component }">
                <component :is="Component" />
            </RouterView>
        </v-main>

        <!--系統頁尾-->
        <RouterView name="Footer" />
    </v-app>
    <!--全螢幕載入中-->
    <SFullLoading
        :theme="pageStore.theme"
        :show="pageStore.isLoading"
    />
    <!--彈跳視窗-->
    <s-dialog
        v-model="dialog.open"
        :theme="pageStore.theme"
        :title="dialog.title"
        :content="dialog.content"
        :persistent="dialog.persistent"
        :is-show-header-cancel="dialog.isShowHeaderCancel"
        :is-show-footer-cancel="dialog.isShowFooterCancel"
    >
        <template #footerBtn>
            <v-dialog-btn @click="dialog.submitDialog">
                確定
            </v-dialog-btn>
        </template>
    </s-dialog>
    <!--訊息提示-->
    <SSnackBar
        v-model="snackbarStore.snackbarModel"
        v-bind="snackbarStore.snackbarOptions"
    />
</template>
<style lang="sass" scoped>
@media print
    .pt-print-0
        padding-top: 0 !important
    .pl-print-0
        padding-left: 0 !important
</style>
