<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue';
import {
    ChatWindow,
    useChat,
    provideMessageHistory,
    useSnackbar,
    provideModelSwitcher,
} from '@smart/vue-chat';

import {
    apiQueryChatRoom,
    apiDetailChatRoom,
    apiGetModelTools,
    apiGetProjectModelTools,
    apiRenameChatRoom,
    apiDeleteChatRoom,
    apiAskChatRoom,
} from '@/api';
import usePageStore from '@/stores/page';
import type { Conversation, FloatingMode } from '@smart/vue-chat';
import type { ChatRoomQuery, McpServerItem, ChatMessage, LlmItem } from '@/types/api/chat';

type Props = {
    projectId?: string;
    chatTitle?: string;
    isProjectMng?: boolean;
};

const {
    projectId = '',
    chatTitle = '',
    isProjectMng = false,
} = defineProps<Props>();

const { setIsLoading } = usePageStore();
// 外部對話清單狀態
const apiConversations = ref<Conversation[]>([]);

// 通知系統
const { showSuccess, showError } = useSnackbar();
const allTools = ref<Record<string, string[]>>({});

/**
 * 從 API 載入模型工具清單
 */
const loadModelToolsFromApi = async () => {
    try {
        const api = isProjectMng ? apiGetProjectModelTools : apiGetModelTools;
        const { data } = await api(projectId);
        if (!data.success) throw data.message;
        data.data.mcpServers.forEach((server: McpServerItem) => {
            allTools.value[server.name] = server.tools;
        });
        selectedTools.value = allTools.value;
        return {
            llms: data.data.llms.map((llm: LlmItem) => ({
                ...llm,
                displayName: llm.serviceId,
            })),
            mcpServers: data.data.mcpServers.map((server: McpServerItem) => ({
                ...server,
                displayName: server.name,
                mcpServerId: server.name,
            })),
        };
    } catch (error) {
        const errorMessage = error instanceof Error ? error.message : '載入模型工具清單失敗';
        showError(errorMessage);
        throw error;
    }
};

const {
    selectedModel,
    switchModel,
    selectedTools,
} = provideModelSwitcher({
    onLoadModelTools: loadModelToolsFromApi,
});

/**
 * 從 API 載入對話清單
 */
const loadConversationsFromApi = async (): Promise<void> => {
    setIsLoading(true);
    try {
        const params = {
            keyword: '',
            pageSize: 9999,
            currentPage: 1,
            sorting: 'createdDate',
            sortingDesc: true,
        };
        const { data } = await apiQueryChatRoom(params);
        if (!data.success) throw data.message;
        apiConversations.value = data.data.map((room: ChatRoomQuery) => ({
            id: room.roomId,
            title: room.name || '未命名對話',
            messages:  [],
            updatedAt: room.createdDate,
            messagesCount: room.messagesCount,
        }));
    } catch (error) {
        const errorMessage = error instanceof Error ? error.message : '載入對話清單失敗';
        showError(errorMessage);
    } finally {
        setIsLoading(false);
    }
};

onMounted(() => {
    if (projectId) return;
    loadConversationsFromApi();
});

watch(() => chatTitle, (val) => {
    if (!val) return;
    title.value = val;
});

const getRole = (role: string) => {
    if (role === 'ai') return 'assistant';
    return role;
};

const getChatInfo = async (roomId: string) => {
    try {
        setIsLoading(true);
        const { data } = await apiDetailChatRoom(roomId);
        if (!data.success) throw data.message;
        title.value = data.data.name || '未命名對話';
        messages.value = data.data.chatMessages.map((msg: ChatMessage) => ({
            ...msg,
            role: getRole(msg.role),
            timestamp: new Date(msg.sentAt).getTime(),
        }));
        if (data.data.llmServiceId) {
            switchModel(data.data.llmServiceId);
        }
        if (data.data.toolSelected) {
            selectedTools.value = data.data.toolSelected;
        }
    } catch {
        showError('取得對話資訊失敗');
    } finally {
        setIsLoading(false);
    }
};

// 事件處理函數
const handleSendMessage = async (content: string) => {
    await sendMessage(content);
};


// 聊天功能
const {
    title,
    messages,
    currentConversationId,
    status,
    canStop,
    sendMessage,
    stopResponse,
    clearMessages
} = useChat({
    apiHandler: async (content: string): Promise<string> => {
        const params = {
            roomId: currentConversationId.value || undefined,
            message: content,
            serviceId: selectedModel.value ? selectedModel.value.id : undefined,
            toolSelected: selectedTools.value,
            projectId: projectId || undefined,
        };
        const { data } = await apiAskChatRoom(params);

        if (!data.success) {
            showError(data.message || '訊息傳送失敗');
            return '訊息傳送失敗';
        }

        if (!currentConversationId.value) {
            currentConversationId.value = data.id;
        }

        if (!currentConversationId.value && !projectId) {
            await loadConversationsFromApi();
            const currentChat = apiConversations.value.find((c) => c.id === data.id);
            if (currentChat) {
                title.value = currentChat.title;
            }
        }

        return data.data as string;
    },
});

// 對話管理
const {
    currentConversation,
    createConversation,
} = provideMessageHistory({
    conversations: apiConversations,
    onRename: async (id: string, newTitle: string) => {
        setIsLoading(true);
        try {
            const { data } = await apiRenameChatRoom({
                id,
                name: newTitle,
            });
            if (!data.success) throw data.message;
            await loadConversationsFromApi();
            return true;
        } catch {
            return false;
        } finally {
            setIsLoading(false);
        }
    },
    onDelete: async (id: string) => {
        setIsLoading(true);
        try {
            const { data } = await apiDeleteChatRoom(id);
            if (!data.success) throw data.message;
            await loadConversationsFromApi();
            return true;
        } catch {
            return false;
        } finally {
            setIsLoading(false);
        }
    },
    onRefresh: async () => {
        await loadConversationsFromApi();
    },
});

// 浮動模式
const floatingMode = defineModel<FloatingMode>('float', {
    default: 'normal',
});

const handleCopyMessage = () => {
    showSuccess('訊息已複製到剪貼板');
};

const handleNewConversation = () => {
    clearMessages();
    title.value = '新對話';
    currentConversationId.value = '';
    selectedTools.value = allTools.value;
    createConversation();
    showSuccess('已建立新對話');
};

const handleSelectConversation = (conversation: Conversation) => {
    currentConversationId.value = conversation.id;
    getChatInfo(conversation.id);
};

const handleFloatingModeChange = (mode: FloatingMode) => {
    floatingMode.value = mode;
};

const showConversationList = computed(() => !projectId);
</script>
<template>
    <ChatWindow
        :title="title"
        :current-conversation="currentConversation"
        :current-conversation-id="currentConversationId"
        :messages="messages"
        :status="status"
        :can-stop="canStop"
        :show-conversation-list="showConversationList"
        :floating-mode="floatingMode"
        :is-floating="true"
        :on-load-model-tools="loadModelToolsFromApi"
        :floating-width="450"
        :is-close-conversation-list-on-switch="false"
        @send-message="handleSendMessage"
        @stop-response="stopResponse"
        @copy-message="handleCopyMessage"
        @new-conversation="handleNewConversation"
        @select-conversation="handleSelectConversation"
        @floating-mode-change="handleFloatingModeChange"
    />
</template>