<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';

import { apiDetailProjectMng, apiUpdateProjectMng } from '@/api';
import usePageStore from '@/stores/page';
import useSnackbarStore from '@/stores/snackbar';
import useProjectModelTool from '@/composables/projectMng';
import PageLayout from '@/components/shared/PageLayout.vue';
import EditInfo from '@/components/projectMng/EditInfo.vue';
import ChatComponent from '@/components/chat/ChatComponent.vue';
import type { ProjectEdit } from '@/types/api/projectMng';
import type { McpServerQuery } from '@/types/api/mcpServerMng';
import type { LlmQuery } from '@/types/api/llmMng';
import type { FloatingMode } from '@smart/vue-chat';

const router = useRouter();
const route = useRoute();
const projectId = computed(() => route.params.id as string);

const goList = () => {
    router.push({ name: 'AiProjectMng' });
};

const { setIsLoading } = usePageStore();
const { errSnack, succSnack, closeSnack } = useSnackbarStore();
const { llmList, mcpServerList } = useProjectModelTool();
const editProject = ref<ProjectEdit>({});

const geInfo = async () => {
    try {
        setIsLoading(true);
        const { data } = await apiDetailProjectMng(projectId.value);
        if (!data.success) throw data.message;
        editProject.value = {
            ...data.data,
            llmIds: data.data.llmInfos.map((llm: LlmQuery) => llm.llmId),
            mcpServerIds: data.data.mcpServers.map((mcp: McpServerQuery) => mcp.mcpServerId),
        };
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

const onUpdateProject = async () => {
    try {
        setIsLoading(true);
        const { data } = await apiUpdateProjectMng(editProject.value);
        if (!data.success) throw data.message;
        succSnack('更新成功');
        if (editProject.value.projectId) return;
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

// 浮動模式
const floatingMode = ref<FloatingMode>('right');
const isShowChat = ref(false);

const toggleChat = () => {
    isShowChat.value = !isShowChat.value;
};

</script>

<template>
    <PageLayout>
        <EditInfo
            v-model="editProject"
            :llm-list="llmList"
            :mcp-server-list="mcpServerList"
            @on:submit="onUpdateProject"
            @on:back="goList"
            @open:chat="toggleChat"
        />
        <v-row>
            <v-col class="pa-0">
                <ChatComponent
                    v-if="isShowChat"
                    v-model:float="floatingMode"
                    is-project-mng
                    :project-id="projectId"
                    :chat-title="editProject.name"
                    @close-floating="toggleChat"
                />
            </v-col>
        </v-row>
    </PageLayout>
</template>

<style scoped>

</style>
