import type { VForm } from 'vuetify/components';

export type VuetifyForm = InstanceType<typeof VForm> | null;

export type Rule = (v?: string) => boolean | string;

export type InfoListItem<T> = {
    title: string;
    key: keyof T;
    type: string;
    required?: boolean;
    list?: unknown[];
    itemTitle?: string;
    itemValue?: string;
    placeholder?: string;
    rules?: Rule[];
    multiple?: boolean;
    disabled?: boolean;
    hint?: string;
};

// 大語言模型來源列舉
export enum LlmSourceType {
    OpenAi,
    AzureOpenAi,
    Ollama,
    Gemini,
    Afs,
}

// MCP Server 類型列舉
export enum McpServerType {
    Stdio,
    Sse,
    Streamable,
}

