// * AUTH
export const AUTH = {
    LOGIN: '/Api/Token/Ldap',
    REFRESH: '/Api/Token/Refresh',
    GET_CAPTCHA: '/Api/Token/Captcha',
};

// * Profile
export const PROFILE = {
    PROFILE_ME: '/Api/Profile/Me',
};

// * LLM_MNG
export const LLM_MNG = {
    QUERY: '/Api/LlmMng/Query',
    DETAIL: '/Api/LlmMng/Detail',
    CREATE: '/Api/LlmMng/Create',
    UPDATE: '/Api/LlmMng/Update',
    DELETE: '/Api/LlmMng/Delete',
};

// * COMMON
export const COMMON = {
    MAPPINGS: '/Api/Common/Mappings',
};

// * MCP_SERVER_MNG
export const MCP_SERVER_MNG = {
    QUERY: '/Api/McpServerMng/Query',
    DETAIL: '/Api/McpServerMng/Detail',
    CREATE: '/Api/McpServerMng/Create',
    UPDATE: '/Api/McpServerMng/Update',
    DELETE: '/Api/McpServerMng/Delete',
};

// * PROJECT_MNG
export const PROJECT_MNG = {
    QUERY: '/Api/ProjectMng/Query',
    DETAIL: '/Api/ProjectMng/Detail',
    CREATE: '/Api/ProjectMng/Create',
    UPDATE: '/Api/ProjectMng/Update',
    DELETE: '/Api/ProjectMng/Delete',
    DOWNLOAD_SETTING: '/Api/ProjectMng/DownloadSetting',
    LLMS: '/Api/ProjectMng/Llms',
    MCP_SERVERS: '/Api/ProjectMng/McpServers',
    MODEL_TOOLS: '/Api/ProjectMng/ModelTools',
};

// * CHAT_ROOM
export const CHAT_ROOM = {
    QUERY: '/Api/ChatRoom/Query',
    DETAIL: '/Api/ChatRoom/Detail',
    ASK: '/Api/ChatRoom/Ask',
    RENAME: '/Api/ChatRoom/Rename',
    DELETE: '/Api/ChatRoom/Delete',
    MODEL_TOOLS: '/Api/ChatRoom/ModelTools',
};