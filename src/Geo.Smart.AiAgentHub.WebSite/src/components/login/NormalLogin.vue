<script setup>
import {
    computed,
    onMounted,
    ref,
} from 'vue';
import { useCaptcha, useVerify } from '@smart/vue-form';

import { apiGetCaptcha } from '@/api';
import usePageStore from '@/stores/page';
import useSnackbarStore from '@/stores/snackbar';
import { login } from '@/utils/auth';
import ChangeQooDialog from './ChangeQooDialog.vue';
import ForgotQooDialog from './ForgotQooDialog.vue';
import { mdiEye, mdiEyeOff } from '@/utils/icons';

defineProps({
    type: {
        type: String,
        default: '0',
    },
});

const { setIsLoading } = usePageStore();
const { errSnack } = useSnackbarStore();
const { captchaId, captchaUrl, refreshCaptcha } = useCaptcha({
    apiGetCaptcha,
    setIsLoading,
    errSnack,
});

const { common } = useVerify();

const info = ref({
    userName: '',
    qoo: '',
    captcha: '',
    captchaId: '',
});

const clearInfo = () => {
    info.value = {
        userName: '',
        qoo: '',
        captcha: '',
        captchaId: '',
    };
};

onMounted(() => {
    refreshCaptcha();
});

const isEditQoo = ref(false);
const toggleEditQoo = () => {
    isEditQoo.value = !isEditQoo.value;
};

const enforceId = ref('');

const formRef = ref(null);

const restForm = () => {
    formRef.value.reset();
};

const submit = async () => {
    const { valid } = await formRef.value.validate();
    if (!valid) return;
    info.value.captchaId = captchaId.value;
    login({
        params: info.value,
        errCallback: (error) => {
            const { code, token } = error;
            // 5: 需要強制修改密碼
            if (code === 5) {
                enforceId.value = token;
                toggleEditQoo();
            }
            clearInfo();
            refreshCaptcha();
        },
    });
};

const isForgotQoo = ref(false);
const toggleForgotQoo = () => {
    isForgotQoo.value = !isForgotQoo.value;
};

const infoType = {
    input: 0,
    captcha: 1,
};

const isShowPassword = ref(false);

const infoList = ref([
    {
        title: '帳號',
        key: 'userName',
        type: infoType.input,
        placeholder: '請輸入帳號',
    },
    {
        title: '密碼',
        key: 'qoo',
        type: infoType.input,
        inputType: computed(() => (isShowPassword.value ? 'text' : 'password')),
        placeholder: '請輸入密碼',
    },
    {
        title: '驗證碼',
        key: 'captcha',
        type: infoType.captcha,
        placeholder: '請輸入驗證碼',
    },
]);
</script>

<template>
    <v-form
        ref="formRef"
        @submit.prevent="submit"
    >
        <template
            v-for="item in infoList"
            :key="item.key"
        >
            <s-captcha-selector
                v-if="item.type === infoType.captcha"
                v-model="info[item.key]"
                :captcha-url="captchaUrl"
                required
                @on:refresh="refreshCaptcha"
            />
            <div
                v-else
                class="mb-4"
            >
                <s-text-field
                    v-model="info[item.key]"
                    :rules="[common.required]"
                    size="full"
                    :type="item.inputType"
                    :placeholder="item.placeholder"
                >
                    <template
                        v-if="item.key === 'qoo'"
                        #append-inner
                    >
                        <v-tooltip
                            :text="isShowPassword ? '不顯示密碼' : '顯示密碼'"
                            location="top"
                        >
                            <template #activator="{ props }">
                                <v-icon
                                    v-bind="props"
                                    :icon="isShowPassword ? mdiEyeOff : mdiEye"
                                    color="grey-darken-1"
                                    @click="isShowPassword = !isShowPassword"
                                />
                            </template>
                        </v-tooltip>
                    </template>
                </s-text-field>
            </div>
        </template>
        <v-btn
            block
            class="mt-4 mb-2"
            type="submit"
        >
            登入
        </v-btn>
        <v-btn
            v-show="type === '0'"
            variant="text"
            @click="toggleForgotQoo"
        >
            忘記密碼
        </v-btn>
        <v-btn
            v-show="type === '0'"
            variant="text"
            @click="toggleEditQoo"
        >
            強制修改密碼
        </v-btn>
    </v-form>
    <ForgotQooDialog v-model="isForgotQoo" />
    <ChangeQooDialog
        v-model="isEditQoo"
        :enforce-id="enforceId"
        @reset-login-form="restForm"
    />
</template>
