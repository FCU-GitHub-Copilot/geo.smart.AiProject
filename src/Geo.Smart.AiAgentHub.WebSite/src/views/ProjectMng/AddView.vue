<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';

import { apiCreateProjectMng } from '@/api';
import usePageStore from '@/stores/page';
import useSnackbarStore from '@/stores/snackbar';
import useProjectModelTool from '@/composables/projectMng';
import PageLayout from '@/components/shared/PageLayout.vue';
import EditInfo from '@/components/projectMng/EditInfo.vue';
import type { ProjectEdit } from '@/types/api/projectMng';

const router = useRouter();

const goList = () => {
    router.push({ name: 'AiProjectMng' });
};

const { setIsLoading } = usePageStore();
const { errSnack, succSnack, closeSnack } = useSnackbarStore();
const { llmList, mcpServerList } = useProjectModelTool();
const editProject = ref<ProjectEdit>({});

const onAddProject = async () => {
    try {
        setIsLoading(true);
        const { data } = await apiCreateProjectMng(editProject.value);
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
            v-model="editProject"
            :llm-list="llmList"
            :mcp-server-list="mcpServerList"
            @on:submit="onAddProject"
            @on:back="goList"
        />
    </PageLayout>
</template>

<style scoped>

</style>
