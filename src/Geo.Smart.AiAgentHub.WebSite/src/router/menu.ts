import {
    mdiGlobeModel,
    mdiRobotOutline,
    mdiServer,
} from '@/utils/icons';

// 系統選單預設值
const defaultMenu = [
    {
        id: '1',
        title: 'LLM 模型管理',
        routeName: 'LlmMng',
        icon: mdiGlobeModel,
        path: '/llmMng',
        component: () => import('@/views/LlmMng/IndexView.vue'),
        roles: [],
        subMenus: [],
    },
    {
        id: '2',
        title: 'MCP Server管理',
        routeName: 'McpServerMng',
        icon: mdiServer,
        path: '/mcpServerMng',
        component: () => import('@/views/McpServerMng/IndexView.vue'),
        roles: [],
        subMenus: [],
    },
    {
        id: '3',
        title: 'AI 專案管理',
        routeName: 'AiProjectMng',
        icon: mdiRobotOutline,
        path: '/aiProjectMng',
        component: () => import('@/views/ProjectMng/IndexView.vue'),
        roles: [],
        subMenus: [],
    },
    // 聊天室
    {
        id: '4',
        title: '聊天室',
        routeName: 'ChatRoom',
        icon: mdiRobotOutline,
        path: '/chatRoom',
        component: () => import('@/views/ChatRoom/IndexView.vue'),
        roles: [],
        subMenus: [],
    },
];

export default defaultMenu;
