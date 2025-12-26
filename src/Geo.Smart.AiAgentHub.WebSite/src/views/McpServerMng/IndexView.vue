<script setup lang="ts">
import {
    onMounted,
    ref,
    watch,
} from 'vue';
import { usePagination } from '@smart/vue-table';
import { useRouter } from 'vue-router';

import {
    apiQueryMcpServer,
    apiDeleteMcpServer,
} from '@/api';
import usePageStore from '@/stores/page';
import useSnackbarStore from '@/stores/snackbar';
import useCommonStore from '@/stores/common';
import useDialogStore from '@/stores/dialog';
import useTableSorting from '@/composables/table';
import PageLayout from '@/components/shared/PageLayout.vue';
import type { McpServerQuery } from '@/types/api/mcpServerMng';

const router = useRouter();
const goAdd = () => {
    router.push({ name: 'McpServerMngAdd' });
};

const goDetail = (llmId: string) => {
    router.push({ name: 'McpServerMngDetail', params: { id: llmId } });
};

const { setIsLoading } = usePageStore();
const { errSnack, succSnack } = useSnackbarStore();
const { getMcpServerTypeName } = useCommonStore();

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
        name: 'MCP 編號',
        key: 'mcpServerId',
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
        name: 'MCP 類型',
        key: 'mcpServerType',
        slot: 'type',
        sortTable: true,
        sortingDesc: false,
    },
    {
        name: '服務位置​',
        key: 'sseUrl',
        slot: 'text',
        sortTable: true,
        sortingDesc: false,
    },
    {
        name: ' ',
        key: 'mcpServerId',
        slot: 'actions',
    },
]);
const list = ref<McpServerQuery[]>([]);

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
        const { data } = await apiQueryMcpServer(params);
        if (!data.success) throw data.message;
        list.value = data.data.map((item: McpServerQuery, index: number) => ({
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

onMounted(() => {
    getFirstList();
});

watch([
    sorting,
    sortingDesc,
    () => pagination.currentPage,
], () => {
    if (isFirstLoad.value) return;
    getFirstList();
});

watch(() => pagination.currentPage, () => {
    if (isFirstLoad.value) return;
    getList();
});

const { openDialog, toggleDialog } = useDialogStore();

const deleteLlm = async (mcpServerId: string) => {
    try {
        setIsLoading(true);
        const { data } = await apiDeleteMcpServer(mcpServerId);
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
                            <template #type="{ data }">
                                <p class="text-start">
                                    {{ getMcpServerTypeName(data) }}
                                </p>
                            </template>
                            <template #actions="{ data }">
                                <v-table-btn @click="goDetail(data)">
                                    詳細
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
