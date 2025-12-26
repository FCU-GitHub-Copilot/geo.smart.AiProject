<script setup>
import { ref, watch } from 'vue';

import { apiGetCaptcha } from '@/api';
import usePageStore from '@/stores/page';
import useSnackbarStore from '@/stores/snackbar';
// import { usePasswordErrorMessage } from '@/utils/auth';
import { useCaptcha, useVerify } from '@smart/vue-form';

const { setIsLoading } = usePageStore();
const { errSnack } = useSnackbarStore();
const { captchaId, captchaUrl, refreshCaptcha } = useCaptcha({
    apiGetCaptcha,
    setIsLoading,
    errSnack,
});
const { common } = useVerify();

const model = defineModel({ type: Boolean, default: false });

const info = ref({
    email: '',
    captcha: '',
    captchaId,
});

const clearInfo = () => {
    info.value = {
        email: '',
        captcha: '',
        captchaId: '',
    };
};

watch(model, (val) => {
    if (val) {
        refreshCaptcha();
        return;
    }
    clearInfo();
});

const formRef = ref(null);

// const { errorMessage } = usePasswordErrorMessage();

// const changeQoo = async () => {
//     try {
//         setIsLoading(true);
//         info.value.captchaId = captchaId.value;
//         const { data } = await apiForgetPassword(info.value);
//         if (!data.success) {
//             const message = errorMessage(data.errorCode);
//             throw message;
//         }
//         succSnack('已將密碼重新設定說明寄到您註冊的信箱，請前往收信');
//         model.value = false;
//     } catch (error) {
//         errSnack(error);
//         refreshCaptcha();
//         clearInfo();
//     } finally {
//         setIsLoading(false);
//     }
// };

const submit = async () => {
    const { valid } = await formRef.value.validate();
    if (!valid) return;
    changeQoo();
};

const infoList = [
    {
        title: '帳號',
        key: 'email',
        required: true,
        rules: [],
        placeholder: '請輸入帳號',
    },
    {
        title: '驗證碼',
        key: 'captcha',
        required: true,
        isCaptcha: true,
    },
];
</script>

<template>
    <s-dialog
        v-model="model"
        title="忘記密碼"
    >
        <template #content>
            <v-form ref="formRef">
                <v-container>
                    <v-row
                        v-for="(item, index) in infoList"
                        :key="index"
                    >
                        <s-form-title
                            :is-required="item.required"
                            is-row
                        >
                            {{
                                item.title
                            }}
                        </s-form-title>
                        <v-table-col>
                            <s-captcha-selector
                                v-if="item.isCaptcha"
                                v-model="info[item.key]"
                                :captcha-url="captchaUrl"
                                required
                                @on:refresh="refreshCaptcha"
                            />
                            <s-text-field
                                v-else
                                v-model="info[item.key]"
                                :rules="
                                    item.required
                                        ? [common.required, ...item.rules]
                                        : []
                                "
                                :placeholder="item.placeholder"
                                :type="item.inputType"
                            />
                        </v-table-col>
                    </v-row>
                </v-container>
            </v-form>
        </template>
        <template #footerBtn>
            <v-dialog-btn @click="submit">
                寄出驗證信
            </v-dialog-btn>
        </template>
    </s-dialog>
</template>
