<script setup lang="ts">
import {
    onMounted,
    ref,
    watch,
} from 'vue';
import { usePagination } from '@smart/vue-table';
import { useRouter } from 'vue-router';

import {
    apiQueryProjectMng,
    apiDeleteProjectMng,
    apiGetProjectLlms,
    apiGetProjectMcpServers,
    apiDownloadProjectSetting,
} from '@/api';
import usePageStore from '@/stores/page';
import useSnackbarStore from '@/stores/snackbar';
import useDialogStore from '@/stores/dialog';
import useTableSorting from '@/composables/table';
import { downloadFile } from '@/utils/common';
import PageLayout from '@/components/shared/PageLayout.vue';
import type { ProjectMngQuery } from '@/types/api/projectMng';
import type { McpServerQuery } from '@/types/api/mcpServerMng';
import type { LlmQuery } from '@/types/api/llmMng';

const router = useRouter();
const goAdd = () => {
    router.push({ name: 'AiProjectMngAdd' });
};

const goDetail = (projectId: string) => {
    router.push({ name: 'AiProjectMngDetail', params: { id: projectId } });
};

const { setIsLoading } = usePageStore();
const { errSnack, succSnack } = useSnackbarStore();

const { pagination } = usePagination();
const {
    sorting,
    sortingDesc,
    sortItems,
} = useTableSorting('name');
const keyword = ref('');
const isFirstLoad = ref(false);
const headerList = ref([
    {
        name: '#',
        key: 'index',
    },
    {
        name: '專案編號',
        key: 'projectId',
        slot: 'text',
        sortTable: true,
        sortingDesc: false,
    },
    {
        name: '名稱',
        key: 'name',
        slot: 'text',
        sortTable: true,
        sortingDesc: false,
    },
    {
        name: '已選取的 LLM',
        key: 'llmNames',
        class: 'w-300',
        slot: 'array',
        sortTable: true,
        sortingDesc: false,
    },
    {
        name: '已選取的 MCP Server',
        key: 'mcpServerNames',
        class: 'w-300',
        slot: 'array',
        sortTable: true,
        sortingDesc: false,
    },
    {
        name: ' ',
        key: 'projectId',
        class: 'w-300',
        slot: 'actions',
    },
]);
const list = ref<ProjectMngQuery[]>([]);
const llmList = ref<LlmQuery[]>([]);
const mcpServerList = ref<McpServerQuery[]>([]);

const getList = async () => {
    try {
        setIsLoading(true);
        const params = {
            keyword: keyword.value,
            currentPage: pagination.currentPage,
            pageSize: pagination.pageSize,
            sorting: sorting.value,
            sortingDesc: sortingDesc.value,
        };
        const { data } = await apiQueryProjectMng(params);
        if (!data.success) throw data.message;
        list.value = data.data.map((item: ProjectMngQuery, index: number) => ({
            ...item,
            index: (pagination.currentPage - 1) * pagination.pageSize + index + 1,
        }));
        pagination.total = data.total;
        pagination.totalPage = data.totalPages;
        if (isFirstLoad.value) isFirstLoad.value = false;
    } catch (error) {
        const message = typeof error === 'string' ? error : '取得列表失敗';
        errSnack(message);
    } finally {
        setIsLoading(false);
    }
};

const getFirstList = () => {
    pagination.currentPage = 1;
    isFirstLoad.value = true;
    getList();
};

const getSettingList = async () => {
    try {
        setIsLoading(true);
        const [llmRes, mcpRes] = await Promise.all([
            apiGetProjectLlms(),
            apiGetProjectMcpServers(),
        ]);
        if (!llmRes.data.success) throw llmRes.data.message;
        if (!mcpRes.data.success) throw mcpRes.data.message;
        llmList.value = llmRes.data.data;
        mcpServerList.value = mcpRes.data.data;
    } catch (error) {
        const message = typeof error === 'string' ? error : '取得設定列表失敗';
        errSnack(message);
    } finally {
        setIsLoading(false);
    }
};

onMounted(() => {
    Promise.all([
        getSettingList(),
        getFirstList(),
    ]);
});

watch(
    () => pagination.pageSize,
    () => {
        if (isFirstLoad.value) return;
        getFirstList();
    }
);

watch([
    sorting,
    sortingDesc,
    () => pagination.currentPage,
], () => {
    if (isFirstLoad.value) return;
    getList();
});

const { openDialog, toggleDialog } = useDialogStore();

const deleteLlm = async (mcpServerId: string) => {
    try {
        setIsLoading(true);
        const { data } = await apiDeleteProjectMng(mcpServerId);
        if (!data.success) throw data.message;
        succSnack('刪除成功');
        getList();
    } catch (error) {
        const message = typeof error === 'string' ? error : '刪除失敗';
        errSnack(message);
    } finally {
        setIsLoading(false);
    }

};

const deleteAlert = (mcpServerId: string) => {
    openDialog({
        title: '刪除MCP Server',
        content: '確定要刪除此MCP Server嗎？',
        isShowHeaderCancel: true,
        isShowFooterCancel: true,
        submitAction: () => {
            toggleDialog(false);
            deleteLlm(mcpServerId);
        },
    });
};

const downloadSetting = async (projectId: string) => {
    try {
        setIsLoading(true);
        const res = await apiDownloadProjectSetting(projectId);
        if (res.status !== 200) throw '下載失敗';
        const project = list.value.find(item => item.projectId === projectId);
        const filename = `${project?.name}設定檔`;
        downloadFile(res.data, filename);
        succSnack('下載成功');
    } catch (error) {
        const message = typeof error === 'string' ? error : '下載失敗';
        errSnack(message);
    } finally {
        setIsLoading(false);
    }
};

const formatStrList = (strList: string[]) => {
    if (!strList || strList.length === 0) return '';
    return strList.join('、');
};

</script>

<template>
    <PageLayout>
        <SKeywordSearch
            v-model="keyword"
            @on:submit="getFirstList"
        />
        <v-row>
            <v-col cols="12">
                <STableLayout v-model:pagination="pagination">
                    <template #btn>
                        <v-btn @click="goAdd">
                            新增
                        </v-btn>
                    </template>
                    <template #list>
                        <SListComponent
                            :items="list"
                            :headers="headerList"
                            @update:sort="sortItems"
                        >
                            <template #text="{ data }">
                                <p class="text-start">
                                    {{ data }}
                                </p>
                            </template>
                            <template #array="{ data }">
                                <p class="text-start">
                                    {{ formatStrList(data) }}
                                </p>
                            </template>
                            <template #actions="{ data }">
                                <v-table-btn @click="goDetail(data)">
                                    詳細
                                </v-table-btn>
                                <v-table-btn @click="downloadSetting(data)">
                                    下載設定檔
                                </v-table-btn>
                                <v-table-btn
                                    color="error"
                                    @click="deleteAlert(data)"
                                >
                                    刪除
                                </v-table-btn>
                            </template>
                        </SListComponent>
                    </template>
                </STableLayout>
            </v-col>
        </v-row>
    </PageLayout>
</template>

<style scoped>

</style>
