import type { LlmQuery } from './llmMng';
import type { McpServerQuery } from './mcpServerMng';

/**
 * 共用：專案基本欄位
 */
export interface ProjectBaseFields {
    /** 專案 ID */
    projectId?: string;
    /** 專案名稱 */
    name?: string;
    /** 專案說明 */
    description?: string;
}

/**
 * AI 專案列表的 ViewModel
 * （共用欄位來自 ProjectBaseFields）
 */
export interface ProjectMngQuery extends ProjectBaseFields {
    /** LLM 名稱清單 */
    llmNames?: string[];
    /** MCP Server 名稱清單 */
    mcpServerNames?: string[];
}

/**
 * AI 專案更新的 ViewModel
 * （共用欄位：projectId / name / description）
 */
export interface ProjectEdit extends ProjectBaseFields {
    /** 系統提示詞 */
    systemPrompt?: string;
    /** LLM 的識別碼清單 */
    llmIds?: string[];
    /** MCP Server 的識別碼清單 */
    mcpServerIds?: string[];
    /** 溫度，控制 LLM 的創造力，範圍 0 到 2 之間 */
    temperature?: number;
    /** 控制 LLM 文本生成的機率篩選器，範圍 0.1 到 2 之間 */
    topP?: number;
    /** LLM 只會從機率最高的 k 個 Tokens 中進行選擇 */
    topK?: number;
    /** 最大的 token 數量 */
    maxTokens?: number;
}

/**
 * AI 專案建立的 ViewModel
 * （共用欄位：projectId / name / description）
 */
export interface ProjectDetail extends ProjectBaseFields {
    /** 系統提示詞 */
    systemPrompt?: string;
    /** 專案擁有者 */
    owner?: string;
    /** LLM 清單 */
    llmInfos?: LlmQuery[];
    /** MCP Server 清單 */
    mcpServers?: McpServerQuery[];
}

