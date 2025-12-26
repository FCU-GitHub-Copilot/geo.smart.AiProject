import { defineStore } from 'pinia';
import { ref, onMounted } from 'vue';

import {
    apiGetCommonMappings,
} from '@/api';
import usePageStore from '@/stores/page';
import useSnackbarStore from '@/stores/snackbar';
import type { CommonItem } from '@/types/api/common';


const useCommon = defineStore('common', () => {
    const { setIsLoading } = usePageStore();
    const { errSnack } = useSnackbarStore();

    const llmSourceTypeList = ref<CommonItem[]>([]);
    const mcpServerTypeList = ref<CommonItem[]>([]);
    const ogcGeometryTypeList = ref<CommonItem[]>([]);

    const getCommonMappings = async () => {
        setIsLoading(true);
        try {
            const res = await apiGetCommonMappings();
            llmSourceTypeList.value = res.data.LlmSourceType;
            mcpServerTypeList.value = res.data.McpServerType;
            ogcGeometryTypeList.value = res.data.OgcGeometryType;
        } catch (error) {
            const message = typeof error === 'string' ? error : '取得共用下拉選單資料失敗';
            errSnack(message);
        } finally {
            setIsLoading(false);
        }
    };

    onMounted(() => {
        getCommonMappings();
    });

    const getLlmSourceTypeName = (key: number) => {
        const item = llmSourceTypeList.value.find((i) => i.key === key);
        return item ? item.name : '';
    };

    const getMcpServerTypeName = (key: number) => {
        const item = mcpServerTypeList.value.find((i) => i.key === key);
        return item ? item.name : '';
    };

    const getOgcGeometryTypeName = (key: number) => {
        const item = ogcGeometryTypeList.value.find((i) => i.key === key);
        return item ? item.name : '';
    };

    return {
        llmSourceTypeList,
        mcpServerTypeList,
        ogcGeometryTypeList,
        getLlmSourceTypeName,
        getMcpServerTypeName,
        getOgcGeometryTypeName,
    };
});

export default useCommon;