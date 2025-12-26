<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRouter, useRoute } from 'vue-router';

import { apiDetailLlmMng, apiUpdateLlmMng } from '@/api';
import usePageStore from '@/stores/page';
import useSnackbarStore from '@/stores/snackbar';
import PageLayout from '@/components/shared/PageLayout.vue';
import EditInfo from '@/components/llmMng/EditInfo.vue';
import type { LlmEdit } from '@/types/api/llmMng';

const router = useRouter();
const route = useRoute();
const llmId = computed(() => route.params.id as string);

const goList = () => {
    router.push({ name: 'LlmMng' });
};

const { setIsLoading } = usePageStore();
const { errSnack, succSnack, closeSnack } = useSnackbarStore();
const editModel = ref<LlmEdit>({});

const geInfo = async () => {
    try {
        setIsLoading(true);
        const { data } = await apiDetailLlmMng(llmId.value);
        if (!data.success) throw data.message;
        editModel.value = data.data;
    } catch (error) {
        const message = typeof error === 'string' ? error : '取得資料失敗';
        errSnack(message);
    } finally {
        setIsLoading(false);
    }
};

onMounted(() => {
    geInfo();
});

const onUpdateLlm = async () => {
    try {
        setIsLoading(true);
        const { data } = await apiUpdateLlmMng(editModel.value);
        if (!data.success) throw data.message;
        succSnack('更新成功');
        if (editModel.value.llmId) return;
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
            @on:submit="onUpdateLlm"
            @on:back="goList"
        />
    </PageLayout>
</template>

<style scoped>

</style>
