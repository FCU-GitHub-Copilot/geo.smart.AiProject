<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRouter, useRoute } from 'vue-router';

import {
    apiDetailMcpServer,
    apiUpdateMcpServer,
} from '@/api';
import usePageStore from '@/stores/page';
import useSnackbarStore from '@/stores/snackbar';
import PageLayout from '@/components/shared/PageLayout.vue';
import EditInfo from '@/components/mcpServerMng/EditInfo.vue';
import type { McpServer } from '@/types/api/mcpServerMng';

const router = useRouter();
const route = useRoute();
const mcpServerId = computed(() => route.params.id as string);

const goList = () => {
    router.push({ name: 'McpServerMng' });
};

const { setIsLoading } = usePageStore();
const { errSnack, succSnack, closeSnack } = useSnackbarStore();
const editMcpServer = ref<McpServer>({});

const geInfo = async () => {
    try {
        setIsLoading(true);
        const { data } = await apiDetailMcpServer(mcpServerId.value);
        if (!data.success) throw data.message;
        editMcpServer.value = data.data;
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

const onUpdateMcpServer = async () => {
    try {
        setIsLoading(true);
        const { data } = await apiUpdateMcpServer(editMcpServer.value);
        if (!data.success) throw data.message;
        succSnack('更新成功');
        if (editMcpServer.value.mcpServerId) return;
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
            @on:submit="onUpdateMcpServer"
            @on:back="goList"
        />
    </PageLayout>
</template>

<style scoped>

</style>
