<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';

import { apiCreateMcpServer } from '@/api';
import usePageStore from '@/stores/page';
import useSnackbarStore from '@/stores/snackbar';
import PageLayout from '@/components/shared/PageLayout.vue';
import EditInfo from '@/components/mcpServerMng/EditInfo.vue';
import type { McpServer } from '@/types/api/mcpServerMng';

const router = useRouter();

const goList = () => {
    router.push({ name: 'McpServerMng' });
};

const { setIsLoading } = usePageStore();
const { errSnack, succSnack, closeSnack } = useSnackbarStore();
const editMcpServer = ref<McpServer>({});

const onAddMcpServer = async () => {
    try {
        setIsLoading(true);
        const { data } = await apiCreateMcpServer(editMcpServer.value);
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
            v-model="editMcpServer"
            @on:submit="onAddMcpServer"
            @on:back="goList"
        />
    </PageLayout>
</template>

<style scoped>

</style>
