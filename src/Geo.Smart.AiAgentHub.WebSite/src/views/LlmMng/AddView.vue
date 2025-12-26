<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';

import { apiCreateLlmMng } from '@/api';
import usePageStore from '@/stores/page';
import useSnackbarStore from '@/stores/snackbar';
import PageLayout from '@/components/shared/PageLayout.vue';
import EditInfo from '@/components/llmMng/EditInfo.vue';
import type { LlmEdit } from '@/types/api/llmMng';

const router = useRouter();

const goList = () => {
    router.push({ name: 'LlmMng' });
};

const { setIsLoading } = usePageStore();
const { errSnack, succSnack, closeSnack } = useSnackbarStore();
const editModel = ref<LlmEdit>({});

const onAddLlm = async () => {
    try {
        setIsLoading(true);
        const { data } = await apiCreateLlmMng(editModel.value);
        if (!data.success) throw data.message;
        succSnack('新增成功');
        setTimeout(() => {
            closeSnack();
            goList();
        }, 500);
    } catch (error) {
        const message = typeof error === 'string' ? error : '儲存失敗';
        errSnack(message);
    } finally {
        setIsLoading(false);
    }
};

</script>

<template>
    <PageLayout>
        <EditInfo
            v-model="editModel"
            @on:submit="onAddLlm"
            @on:back="goList"
        />
    </PageLayout>
</template>

<style scoped>

</style>
