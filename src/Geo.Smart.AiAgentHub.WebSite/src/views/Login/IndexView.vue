<script setup lang="ts">
import { onMounted, ref, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useDisplay } from 'vuetify';

import { login } from '@/utils/auth/index';
import useDialogStore from '@/stores/dialog';
import useUserStore from '@/stores/user';
import NormalLogin from '@/components/login/NormalLogin.vue';
import FooterShared from '@/components/shared/FooterShared.vue';

// 路由
const router = useRouter();
const route = useRoute();
const { mobile } = useDisplay();

const userStore = useUserStore();

// 回首頁
const goHome = () => {
    const redirectUri = Array.isArray(route.query.redirectUri)
        ? route.query.redirectUri[0]
        : route.query.redirectUri || '/llmMng';
    router.push(redirectUri as string);
};

const loginType = {
    normal: '0',
};

const loginList = [
    {
        title: 'AD帳號登入',
        btnTitle: 'AD帳號登入',
        btnSubTitle: '',
        value: loginType.normal,
    },
];

const { dialog, toggleDialog } = useDialogStore();

const currentType = ref(loginType.normal);

const handleLogin = (token: string | null) => {
    if (token) login({ token, succCallback: goHome });
};

onMounted(async () => {
    if (dialog.open) toggleDialog(false);
    handleLogin(userStore.token);
});

watch(() => userStore.token, handleLogin);
</script>

<template>
    <v-container class="align-center fill-height">
        <v-row class="justify-center">
            <v-col
                cols="12"
                sm="11"
                md="7"
            >
                <v-card class="py-4">
                    <v-card-title class="text-h4 font-weight-bold">
                        登入 Login
                    </v-card-title>
                    <v-container>
                        <v-row>
                            <v-col
                                cols="12"
                                sm="4"
                                class="d-flex flex-column login-sidebar"
                            >
                                <s-selector
                                    v-if="mobile"
                                    v-model="currentType"
                                    :items="loginList"
                                    item-title="btnTitle"
                                    size="full"
                                />
                                <template v-else>
                                    <v-hover
                                        v-for="(btn, index) in loginList"
                                        :key="index"
                                    >
                                        <template
                                            #default="{
                                                isHovering,
                                                props: hProps,
                                            }"
                                        >
                                            <v-btn
                                                block
                                                v-bind="hProps"
                                                :active="
                                                    currentType === btn.value
                                                "
                                                :color="
                                                    isHovering ||
                                                        currentType === btn.value
                                                        ? 'primary'
                                                        : 'surface'
                                                "
                                                class="text-h6"
                                                @click="currentType = btn.value"
                                            >
                                                <div class="d-flex flex-column">
                                                    {{ btn.btnTitle }}
                                                    <div
                                                        v-if="btn.btnSubTitle"
                                                        v-html="btn.btnSubTitle"
                                                    />
                                                </div>
                                            </v-btn>
                                        </template>
                                    </v-hover>
                                </template>
                            </v-col>
                            <v-col>
                                <NormalLogin :type="currentType" />
                            </v-col>
                        </v-row>
                    </v-container>
                </v-card>
            </v-col>
        </v-row>
    </v-container>
    <FooterShared />
</template>

<style lang="sass">
.login-sidebar
    align-self: auto !important
    .v-btn.v-btn--active
        position: relative
        > .v-btn__overlay
            opacity: 0 !important
        &:before
            content: ''
            position: absolute
            right: -1.6em
            top: calc(50% - .8em)
            opacity: 1
            border: .8em solid transparent
            border-left-color: rgb(var(--v-theme-primary))
</style>
