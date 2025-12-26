import request from '@/utils/axios';
import {
    AUTH,
    LLM_MNG,
    COMMON,
    MCP_SERVER_MNG,
    PROJECT_MNG,
    CHAT_ROOM,
    PROFILE,
} from './constants';
import type {
    LoginParams,
    RefreshTokenParams,
} from '@/types/api/auth';
import type { LlmEdit } from '@/types/api/llmMng';
import type { McpServer } from '@/types/api/mcpServerMng';
import type { ProjectEdit } from '@/types/api/projectMng';
import type { Ask, Rename } from '@/types/api/chat';
import type { PageParams } from '@/types/common/pagination';

// #region 登入相關
// 帳號登入
export const apiLogin = (params: LoginParams) => request.post(AUTH.LOGIN, params);
// 更新 token
export const apiRefreshToken = (params: RefreshTokenParams) => request.post(AUTH.REFRESH, params);
// 取得驗證碼
export const apiGetCaptcha = () => request.get(AUTH.GET_CAPTCHA);
// #endregion

// #region Profile
// 取得使用者資訊
export const apiProfileMe = () => request.get(PROFILE.PROFILE_ME);
// #endregion

// #region LLM 模型管理
// 查詢 LLM 模型列表
export const apiQueryLlmMng = (params: PageParams) => request.get(LLM_MNG.QUERY, { params });
// 取得 LLM 模型詳細資料
export const apiDetailLlmMng = (id: string) => request.get(`${LLM_MNG.DETAIL}/${id}`);
// 新增 LLM 模型
export const apiCreateLlmMng = (params: LlmEdit) => request.post(LLM_MNG.CREATE, params);
// 更新 LLM 模型
export const apiUpdateLlmMng = (params: LlmEdit) => request.post(LLM_MNG.UPDATE, params);
// 刪除 LLM 模型
export const apiDeleteLlmMng = (id: string) => request.post(`${LLM_MNG.DELETE}/${id}`);
// #endregion

// #region 共用
// 取得共用下拉選單資料
export const apiGetCommonMappings = () => request.get(COMMON.MAPPINGS);
// #endregion

// #region Mcp Server 管理
// 查詢 Mcp Server 列表
export const apiQueryMcpServer = (params: PageParams) => request.get(MCP_SERVER_MNG.QUERY, { params });
// 取得 Mcp Server 詳細資料
export const apiDetailMcpServer = (mcpServerId: string) => request.get(`${MCP_SERVER_MNG.DETAIL}/${mcpServerId}`);
// 新增 Mcp Server
export const apiCreateMcpServer = (params: McpServer) => request.post(MCP_SERVER_MNG.CREATE, params);
// 更新 Mcp Server
export const apiUpdateMcpServer = (params: McpServer) => request.post(MCP_SERVER_MNG.UPDATE, params);
// 刪除 Mcp Server
export const apiDeleteMcpServer = (mcpServerId: string) => request.post(`${MCP_SERVER_MNG.DELETE}/${mcpServerId}`);
// #endregion

// #region AI 專案管理
// 查詢 AI 專案列表
export const apiQueryProjectMng = (params: PageParams) => request.get(PROJECT_MNG.QUERY, { params });
// 取得 AI 專案詳細資料
export const apiDetailProjectMng = (projectId: string) => request.get(`${PROJECT_MNG.DETAIL}/${projectId}`);
// 新增 AI 專案
export const apiCreateProjectMng = (params: ProjectEdit) => request.post(PROJECT_MNG.CREATE, params);
// 更新 AI 專案
export const apiUpdateProjectMng = (params: ProjectEdit) => request.post(PROJECT_MNG.UPDATE, params);
// 刪除 AI 專案
export const apiDeleteProjectMng = (projectId: string) => request.post(`${PROJECT_MNG.DELETE}/${projectId}`);
// 下載 AI 專案設定檔
export const apiDownloadProjectSetting = (projectId: string) => request.get(`${PROJECT_MNG.DOWNLOAD_SETTING}/${projectId}`, { responseType: 'blob' });
// 取得 LLM 列表
export const apiGetProjectLlms = () => request.get(PROJECT_MNG.LLMS);
// 取得 MCP Server 列表
export const apiGetProjectMcpServers = () => request.get(PROJECT_MNG.MCP_SERVERS);
// 取得模型可用工具列表
export const apiGetProjectModelTools = (projectId?: string) => request.get(PROJECT_MNG.MODEL_TOOLS, { params: { projectId } });
// #endregion

// #region 聊天室
// 查詢聊天室列表
export const apiQueryChatRoom = (params: PageParams) => request.get(CHAT_ROOM.QUERY, { params });
// 取得聊天室詳細資料
export const apiDetailChatRoom = (roomId: string) => request.get(`${CHAT_ROOM.DETAIL}/${roomId}`);
// 使用者提問
export const apiAskChatRoom = (params: Ask) => request.post(CHAT_ROOM.ASK, params);
// 聊天室重新命名
export const apiRenameChatRoom = (params: Rename) => request.post(CHAT_ROOM.RENAME, params);
// 刪除聊天室
export const apiDeleteChatRoom = (roomId: string) => request.post(`${CHAT_ROOM.DELETE}/${roomId}`);
// 取得模型可用工具列表
export const apiGetModelTools = (projectId?: string) => request.get(CHAT_ROOM.MODEL_TOOLS, { params: { projectId } });
// #endregion
