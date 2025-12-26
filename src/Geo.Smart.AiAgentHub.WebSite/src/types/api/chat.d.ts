/**
 * 聊天室清單 ViewModel
 */
export type ChatRoomQuery = {
    /** 聊天室 ID */
    roomId?: string;
    /** 聊天室名稱 */
    name?: string;
    createdDate: string;
    messagesCount: number;
};

/**
 * 聊天室訊息內容
 */
export type ChatMessage = {
    /** 訊息主鍵 */
    messageId: string;
    /** 發送者角色（user/system/ai） */
    role: string;
    /** 訊息內容 */
    content: string;
    /** 訊息發送時間 */
    sentAt: string;
};

/**
 * 聊天室詳細資訊的檢視模型
 */
export interface ChatRoomDetail extends ChatRoomVm {
    /** 聊天室訊息清單 */
    chatMessages?: ChatMessage[];
    /** 服務識別碼，必要，建議使用服務來源與 ModelId 組合 */
    llmServiceId?: string;
    /** 使用者挑選的工具清單 */
    toolSelected?: Tool;
}

type Tool = {
    [key: string]: string[],
};

export type McpServerItem = {
    name: string;
    mcpServerType: number;
    tools: string[];
};

export type LlmItem = {
    serviceId: string;
    modelId: string;
    llmSourceType: number;
};

/**
 * 使用者提問內容
 */
export type Ask = {
    /** 聊天室 ID，沒有的話要自動建立一個聊天室 */
    roomId?: string;
    /** 取得或設定提問內容 */
    message: string;
    /** 服務識別碼，必要，建議使用服務來源與 ModelId 組合 */
    serviceId?: string;
    /** 使用者挑選的工具清單 */
    toolSelected?: Tool;
    /** SingleR 的連線 ID */
    connectionId?: string;
    /** 參考圖片位置清單 */
    images?: string[];
    /** 專案 ID */
    projectId?: string;
};

export type Rename = {
    id?: string;
    name?: string;
};


