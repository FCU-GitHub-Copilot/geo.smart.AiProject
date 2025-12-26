<script setup>
import { ref, watch } from 'vue';

import { useVerify } from '@smart/vue-form';
import usePageStore from '@/stores/page';
import useSnackbarStore from '@/stores/snackbar';
import PasswordInput from './PasswordInput.vue';

const props = defineProps({
    enforceId: {
        type: String,
        default: '',
    },
});

const emits = defineEmits(['resetLoginForm']);

const { setIsLoading } = usePageStore();
const { errSnack, succSnack } = useSnackbarStore();

const model = defineModel({ type: Boolean, default: false });

const info = ref({
    enforceId: '',
    newQoo: '',
    confirmQoo: '',
});

const clearInfo = () => {
    info.value = {
        enforceId: '',
        newQoo: '',
        confirmQoo: '',
    };
};

const checkQoo = (v) => v === info.value.newQoo || '必須與新密碼相同';

watch(model, (val) => {
    if (!val) clearInfo();
});

const formRef = ref(null);

const changeQoo = async () => {
    try {
        setIsLoading(true);
        info.value.enforceId = props.enforceId;
        // const { data } = await apiEnforceResetQoo(info.value);
        // if (!data.success) throw data.message;
        succSnack('密碼修改成功，請於下次登入時使用新密碼');
        emits('resetLoginForm');
        model.value = false;
    } catch (error) {
        errSnack(error);
    } finally {
        setIsLoading(false);
    }
};

const submit = async () => {
    const { valid } = await formRef.value.validate();
    if (valid) changeQoo();
};

const { common } = useVerify();

const infoList = [
    {
        title: '新密碼',
        key: 'newQoo',
        rules: [common.required],
    },
    {
        title: '確認密碼',
        key: 'confirmQoo',
        rules: [checkQoo, common.required],
    },
];

const alertList = [
    '密碼原則：',
    '1.密碼長度 8 個字元以上，包含英文大小寫、數字以及特殊字元。',
    '2.不可以與前3次使用過的密碼相同。',
];
</script>

<template>
    <s-dialog
        v-model="model"
        title="需強制修改密碼"
    >
        <template #content>
            <v-form ref="formRef">
                <v-container>
                    <v-row
                        v-for="item in infoList"
                        :key="item.key"
                    >
                        <s-form-title
                            is-required
                            is-row
                        >
                            {{
                                item.title
                            }}
                        </s-form-title>
                        <v-table-col>
                            <PasswordInput
                                v-model="info[item.key]"
                                :rules="item.rules"
                            />
                        </v-table-col>
                    </v-row>
                    <v-row>
                        <v-col>
                            <p
                                v-for="(alert, index) in alertList"
                                :key="index"
                                class="text-error"
                            >
                                {{ alert }}
                            </p>
                        </v-col>
                    </v-row>
                </v-container>
            </v-form>
        </template>
        <template #footerBtn>
            <v-dialog-btn @click="submit">
                確定
            </v-dialog-btn>
        </template>
    </s-dialog>
</template>
