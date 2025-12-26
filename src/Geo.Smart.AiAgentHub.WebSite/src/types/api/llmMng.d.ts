/**
 * LLM 資料列表 ViewModel
 */
export type LlmQuery = {
    /** LLM ID */
    llmId?: string;
    /** 模型管理名稱、服務識別碼 */
    serviceId?: string;
    /** LLM 模型名稱 */
    modelId?: string;
    llmSourceType?: LlmSourceType;
    /** 說明 */
    description?: string;
};

/**
 * 建立 LLM 設定資料
 */
export interface LlmEdit extends LlmQuery {
    /** API 金鑰 */
    apiKey?: string;
    /** 端點網址 */
    endpoint?: string;
    /** 部署名稱 */
    deploymentName?: string;
};

/**
 * LLM 詳細資料 ViewModel
 */
export interface LlmDetail extends LlmEdit {
    /** 擁有者 UserId */
    userId?: string;
    /** 擁有者姓名 */
    userName?: string;
}


