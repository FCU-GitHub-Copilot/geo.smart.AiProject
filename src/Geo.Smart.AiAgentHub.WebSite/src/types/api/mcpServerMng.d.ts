type StdioEnv = {
    [key: string]: string;
};

export type McpServerQuery = {
    mcpServerId?: string;
    name?: string;
    mcpServerType?: number;
    sseUrl?: string;
    stdioCommand?: string;
};

export interface McpServer extends McpServerQuery {
    stdioArgs?: string[];
    stdioEnv?: StdioEnv;
    tools?: string[];
};

/**
 * MCP Server 詳細資訊
 */
export interface McpServerDetail extends McpServer {
    /** 擁有者姓名 */
    userName?: string;
}
