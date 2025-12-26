import {
    onMounted,
    ref,
} from 'vue';

import {
    apiGetProjectLlms,
    apiGetProjectMcpServers,
} from '@/api';
import usePageStore from '@/stores/page';
import useSnackbarStore from '@/stores/snackbar';
import type { McpServerQuery } from '@/types/api/mcpServerMng';
import type { LlmQuery } from '@/types/api/llmMng';

const useProjectModelTool = () => {
    const { setIsLoading } = usePageStore();
    const { errSnack } = useSnackbarStore();

    const llmList = ref<LlmQuery[]>([]);
    const mcpServerList = ref<McpServerQuery[]>([]);

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
        getSettingList();
    });

    return {
        llmList,
        mcpServerList,
    };
};

export default useProjectModelTool;