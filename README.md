# SMART AI Agent Hub 研發專案

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)](https://dotnet.microsoft.com/)
[![Vue.js](https://img.shields.io/badge/Vue.js-3.5-4FC08D)](https://vuejs.org/)
[![License](https://img.shields.io/badge/License-Proprietary-red.svg)](LICENSE)

企業級 AI 代理中心平台，整合多種大型語言模型（LLM）與模型上下文協議（MCP）伺服器，提供統一的 AI 服務介面與聊天室功能

## 目錄

- [專案概述](#專案概述)
- [技術架構](#技術架構)
- [專案結構](#專案結構)
- [核心功能](#核心功能)
- [開始使用](#開始使用)
- [開發指南](#開發指南)
- [API 文件](#api-文件)
- [已知問題與未來計劃](#已知問題與未來計劃)

## 專案概述

SMART AI Agent Hub 是一個企業級的 AI 代理管理平台，主要功能包括：

- **多模型整合**：支援 OpenAI、Google Gemini、Ollama 等多種 LLM 提供商
- **MCP 協議支援**：實作 Model Context Protocol，提供標準化的工具呼叫機制
- **聊天室服務**：完整的多輪對話管理與歷史紀錄
- **專案管理**：支援多個 AI 專案的建立與配置
- **使用者權限**：基於 ASP.NET Core Identity 的完整身分驗證與授權機制
- **資料追蹤**：完整的使用者足跡與操作日誌記錄
- **現代化前端**：基於 Vue 3 與 Vuetify 的響應式管理介面

## 技術架構

### 後端技術棧

- **框架**：.NET 8.0 / .NET Framework 4.8
- **ORM**：Entity Framework Core 8.0
- **資料庫**：SQL Server (支援 NetTopologySuite 空間資料)
- **AI 核心**：Microsoft Semantic Kernel 1.67.1
- **認證授權**：ASP.NET Core Identity + JWT Bearer
- **日誌記錄**：Serilog (支援 MSSqlServer, Seq)
- **API 文件**：Swagger/OpenAPI
- **MCP 協議**：ModelContextProtocol 0.3.0-preview.4

### 前端技術棧

- **框架**：Vue 3.5 (Composition API)
- **型別系統**：TypeScript 5.9+
- **UI 函式庫**：Vuetify 3.9 (Material Design 3)
- **狀態管理**：Pinia 3.0
- **路由管理**：Vue Router 4.5
- **建構工具**：Vite 7.1
- **HTTP 客戶端**：Axios 1.9
- **JWT 處理**：jose 6.1
- **工具函式庫**：@vueuse/core 13.9
- **自訂元件**：@smart/vue-* 系列 (表單、表格、聊天、上傳等)

### 架構層級

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                        │
│  ┌──────────────────┐  ┌────────────────────────────────┐  │
│  │ WebApi           │  │ McpSseServer                   │  │
│  │ (REST API)       │  │ (MCP Protocol Server)          │  │
│  └──────────────────┘  └────────────────────────────────┘  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ WebSite (Vue 3 SPA)                                  │  │
│  │ - Vuetify UI Components                              │  │
│  │ - Pinia State Management                             │  │
│  │ - Vue Router                                          │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                     Business Layer                           │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ Services (業務邏輯服務)                               │  │
│  │ - UserMngService                                      │  │
│  │ - ProjectMngService                                   │  │
│  │ - LlmMngService                                       │  │
│  │ - McpServerMngService                                 │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                      AI Kernel Layer                         │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ AiKernel (Semantic Kernel 整合)                      │  │
│  │ - ChatRoomService                                     │  │
│  │ - AiAgentChat                                         │  │
│  │ - MCP Client/Server 整合                             │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                    Data Access Layer                         │
│  ┌──────────────────┐  ┌────────────────────────────────┐  │
│  │ DataAccess       │  │ Entities                       │  │
│  │ (EF Core Context)│  │ (ViewModel & DTO)              │  │
│  └──────────────────┘  └────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                    Infrastructure Layer                      │
│  ┌──────────────────┐  ┌────────────────────────────────┐  │
│  │ Infras           │  │ EfGenerator                    │  │
│  │ (共用基礎設施)    │  │ (資料庫模型產生器)             │  │
│  └──────────────────┘  └────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

## 專案結構

### 後端專案

| 專案名稱 | 說明 | 技術 |
|---------|------|------|
| **Geo.Smart.AiAgentHub.WebApi** | 主要 Web API 應用程式 | ASP.NET Core Web API |
| **Geo.Smart.AiAgentHub.AiKernel** | AI 核心功能，整合 Semantic Kernel | Semantic Kernel, MCP Protocol |
| **Geo.Smart.AiAgentHub.Services** | 業務邏輯服務層 | .NET 8 Class Library |
| **Geo.Smart.AiAgentHub.DataAccess** | 資料存取層 (EF Core DbContext) | Entity Framework Core |
| **Geo.Smart.AiAgentHub.Entities** | 實體模型與 ViewModel | .NET 8 Class Library |
| **Geo.Smart.AiAgentHub.Infras** | 基礎設施與共用工具 | .NET 8 Class Library |
| **Geo.Smart.AiAgentHub.McpSseServer** | MCP SSE 協議伺服器 | ASP.NET Core Web API |
| **Geo.Smart.AiAgentHub.EfGenerator** | EF Core Reverse POCO 產生器 | .NET Framework 4.8 Console |

### 前端專案

| 專案名稱 | 說明 | 技術 |
|---------|------|------|
| **Geo.Smart.AiAgentHub.WebSite** | 前端管理介面 (SPA) | Vue 3 + TypeScript + Vuetify |

#### 前端專案結構

```
WebSite/
├── src/
│   ├── api/              # API 請求封裝
│   │   ├── index.ts      # API 函式集中管理
│   │   └── constants.ts  # API 路徑常數
│   ├── assets/           # 靜態資源
│   │   └── sass/         # 全域樣式
│   ├── components/       # Vue 元件
│   │   ├── chat/         # 聊天室元件
│   │   ├── login/        # 登入相關元件
│   │   ├── shared/       # 共用元件
│   │   ├── llmMng/       # LLM 管理元件
│   │   ├── mcpServerMng/ # MCP Server 管理元件
│   │   └── projectMng/   # 專案管理元件
│   ├── composables/      # 組合式函式
│   │   └── featureFlag/  # 功能開關
│   ├── layouts/          # 佈局元件
│   ├── plugins/          # Vue 插件
│   ├── router/           # 路由配置
│   ├── stores/           # Pinia 狀態管理
│   │   ├── user.ts       # 使用者狀態
│   │   ├── page.ts       # 頁面設定 (主題、佈局)
│   │   ├── profile.ts    # 個人資訊
│   │   ├── dialog.ts     # 全域對話框
│   │   ├── snackbar.ts   # 全域訊息提示
│   │   └── inactivityTimer.ts # 閒置計時器
│   ├── themes/           # Vuetify 主題配置
│   ├── types/            # TypeScript 型別定義
│   │   ├── api/          # API 型別
│   │   └── models/       # 資料模型型別
│   ├── utils/            # 工具函式
│   ├── views/            # 頁面元件
│   ├── App.vue           # 根元件
│   └── main.ts           # 應用程式入口
├── .env.development      # 開發環境變數
├── .env.staging          # 測試環境變數
├── .env.production       # 生產環境變數
├── eslint.config.mjs     # ESLint 配置
├── vite.config.ts        # Vite 配置
├── tsconfig.json         # TypeScript 配置
└── package.json          # 專案依賴
```

### 測試專案

| 專案名稱 | 說明 |
|---------|------|
| **Geo.Smart.AiAgentHub.AiKernelTests** | AI Kernel 單元測試 |
| **Geo.Smart.AiAgentHub.ServicesTests** | Services 單元測試 |

### 資料庫專案

| 專案名稱 | 說明 |
|---------|------|
| **Geo.Smart.AiAgentHub.DbModel** | SQL Server 資料庫結構定義 (SSDT) |

## 核心功能

### 1. 聊天室服務 (ChatRoom)

- 多輪對話管理
- 聊天歷史紀錄持久化
- 即時訊息處理
- 支援多種 LLM 模型切換
- Token 使用量統計
- 即時 UI 互動 (基於 @smart/vue-chat)

### 2. AI 專案管理

- 建立與管理多個 AI 專案
- 配置專案專屬的 System Prompt
- 關聯 LLM 模型與 MCP 工具
- 專案權限控制
- 視覺化管理介面

### 3. LLM 模型管理

支援的 LLM 提供商：
- OpenAI (GPT-3.5, GPT-4 系列)
- Google Gemini
- Ollama (本地部署)

功能：
- 模型參數配置
- API Key 管理
- 模型效能監控
- 視覺化管理介面

### 4. MCP Server 管理

- MCP Server 註冊與配置
- 工具 (Tools) 管理
- SSE (Server-Sent Events) 傳輸支援
- 自訂工具整合
- 視覺化管理介面

### 5. 使用者管理

- ASP.NET Core Identity 整合
- JWT Token 認證
- 角色權限管理
- 使用者足跡追蹤
- 密碼變更強制機制
- 閒置自動登出機制

### 6. 檔案管理

- 圖片上傳 (PhotoManager)
- 一般檔案上傳 (FilesManager)
- Azure Storage 整合
- 前端檔案上傳元件 (@smart/vue-uploadfile)

### 7. 前端特色功能

- **主題系統**：多主題支援 (亮色/暗色模式切換)
- **功能開關**：動態啟用/停用功能
- **響應式設計**：支援桌面與行動裝置
- **全域訊息**：統一的錯誤處理與訊息提示
- **權限控制**：基於角色的路由與選單過濾
- **閒置逾時**：自動檢測使用者閒置並登出

## 開始使用

### 前置需求

**後端**：
- .NET 8.0 SDK
- .NET Framework 4.8 Developer Pack (EfGenerator 需要)
- SQL Server 2019 或更新版本
- Visual Studio 2022 或 Visual Studio Code

**前端**：
- Node.js 22.16.0 或更新版本
- Yarn 套件管理工具

### 安裝步驟

#### 1. 複製專案
```bash
git clone https://github.com/FCU-GitHub-Copilot/geo.smart.AiProject.git
cd geo.smart.aiagenthub
```

#### 2. 後端設定

**2.1 設定資料庫連線**

編輯 `src/Geo.Smart.AiAgentHub.AiKernel/Geo.Smart.AiAgentHub.WebApi/appsettings.json`：
```json
{
  "ConnectionStrings": {
    "GdbConnection": "加密後的連線字串",
    "StorageConnection": "加密後的 Storage 連線字串"
  }
}
```

**2.2 執行資料庫遷移**
```bash
# 使用 SSDT 部署資料庫專案
# 或使用 EfGenerator 產生實體模型
```

**2.3 設定 LLM 配置**

編輯 `appsettings.json` 中的 `AiHubProject` 區段：
```json
{
  "AiHubProject": {
    "SystemPrompt": "你是一個有用的 AI 助理",
    "LlmInfos": [
      {
        "ServiceId": "your-service-id",
        "ModelId": "gpt-4",
        "LlmSourceType": "OpenAI"
      }
    ]
  }
}
```

**2.4 啟動後端 API**
```bash
cd src/Geo.Smart.AiAgentHub.AiKernel/Geo.Smart.AiAgentHub.WebApi
dotnet run
```

**2.5 存取 Swagger UI**
```
https://localhost:5001/swagger
```

#### 3. 前端設定

**3.1 安裝依賴套件**
```bash
cd src/Geo.Smart.AiAgentHub.WebSite
yarn install
```

**3.2 設定環境變數**

編輯 `.env.development` (開發環境)：
```env
VITE_APP_TITLE=SMART AI Agent Hub
VITE_APP_PATH=/
VITE_APP_API=https://localhost:5001
NODE_ENV=development
```

**3.3 啟動開發伺服器**
```bash
yarn dev
```

前端應用程式將在 `https://localhost:5001` 啟動

**3.4 建置生產版本**
```bash
# 測試環境
yarn stag

# 生產環境
yarn build
```

## 開發指南

### 後端開發

#### 專案相依性

```
WebApi
  ├─ AiKernel
  │   └─ Infras
  ├─ Services
  │   ├─ AiKernel
  │   ├─ DataAccess
  │   ├─ Entities
  │   └─ Infras
  ├─ DataAccess
  ├─ Entities
  └─ Infras

McpSseServer
  └─ (獨立運行)
```

#### 程式碼規範

專案遵循以下規範：

- ✅ 符合 .NET Analyzers、ASP.NET Core Analyzers 警告層級以上的規範
- ✅ 所有 `if` 陳述式必須使用大括號 `{}`
- ✅ XML 註解使用繁體中文，句尾不加句號
- ✅ 類別、方法、參數都需要有註解說明
- ✅ 私有方法也需要註解

#### 服務註冊機制

專案使用自動註冊機制：

```csharp
// 自動註冊所有 Service (命名慣例：XxxService 實作 IXxxService)
void DiServiceLifetime(Assembly serviceAssembly)
{
    var serviceTypes = serviceAssembly.GetTypes()
        .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service"))
        .Select(t => new
        {
            ServiceType = t.GetInterfaces().FirstOrDefault(i => i.Name == $"I{t.Name}"),
            ImplementationType = t
        })
        .Where(x => x.ServiceType != null);

    foreach (var type in serviceTypes)
    {
        builder.Services.AddScoped(type.ServiceType!, type.ImplementationType);
    }
}

// Helper 依據 DiLifetimeAttribute 決定生命週期
void DiHelperLifetime(Assembly serviceAssembly) { /* ... */ }
```

#### 新增服務

1. 在 `Services` 專案建立服務介面與實作
   ```csharp
   public interface IMyService { /* ... */ }
   
   public class MyService : IMyService { /* ... */ }
   ```

2. 服務會自動註冊，無需手動加入 `Program.cs`

#### 新增 Controller

在 `WebApi/Controllers` 建立 Controller：

```csharp
[ApiController]
[Route("[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class MyController(IMyService _service) : SmartController
{
    /// <summary>
    /// 方法說明
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<Result<MyVm>>> Get()
    {
        return Ok(await _service.Get());
    }
}
```

#### 資料庫模型更新

使用 EfGenerator 產生實體模型：

1. 修改資料庫結構
2. 執行 `Geo.Smart.AiAgentHub.EfGenerator`
3. 檢查產生的 `DataAccess/Entities` 檔案
4. 更新 DbContext 配置

### 前端開發

#### 主要指令

```bash
# 開發模式 (含熱重載)
yarn dev

# 型別檢查
yarn vue-tsc --noEmit

# ESLint 檢查
npx eslint .

# 建置測試環境
yarn stag

# 建置生產環境
yarn build

# 預覽建置結果
yarn preview
```

#### 程式碼規範

- **縮排**：4 空格
- **字串**：單引號
- **結尾**：每行必須加分號
- **命名規則**：
  - Vue 元件/型別/介面：PascalCase
  - 變數/函式/檔案：camelCase
  - 常數：SCREAMING_SNAKE_CASE
  - 路由名稱：PascalCase，路徑 kebab-case
- **型別**：所有函式/變數必須明確型別
- **匯入順序**：Node 內建 → 第三方 → @ alias 專案內

#### Vue 元件開發

使用 Composition API 與 `<script setup>` 語法：

```vue
<script setup lang="ts">
import { ref, computed } from 'vue';
import type { MyData } from '@/types/models/myData';

// Props 定義
interface Props {
    title: string;
    items: MyData[];
}

const props = defineProps<Props>();

// Emits 定義
const emit = defineEmits<{
    update: [value: string];
    delete: [id: number];
}>();

// 響應式資料
const count = ref<number>(0);

// 計算屬性
const displayTitle = computed(() => `${props.title} (${props.items.length})`);

// 方法
function handleClick(): void {
    count.value++;
    emit('update', 'clicked');
}
</script>

<template>
    <v-card>
        <v-card-title>{{ displayTitle }}</v-card-title>
        <v-card-text>
            <v-btn @click="handleClick">Count: {{ count }}</v-btn>
        </v-card-text>
    </v-card>
</template>
```

#### API 呼叫

所有 API 呼叫集中在 `src/api/index.ts`：

```typescript
// src/api/index.ts
import axiosInstance from '@/utils/axios';
import { API_ENDPOINTS } from './constants';
import type { Result, MyData } from '@/types';

export const myApi = {
    async getData(): Promise<Result<MyData[]>> {
        const response = await axiosInstance.get(API_ENDPOINTS.MY_DATA);
        return response.data;
    },
    
    async createData(data: MyData): Promise<Result<string>> {
        const response = await axiosInstance.post(API_ENDPOINTS.MY_DATA, data);
        return response.data;
    },
};
```

#### 狀態管理 (Pinia)

建立新的 Store：

```typescript
// src/stores/myStore.ts
import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import type { MyData } from '@/types/models/myData';

export const useMyStore = defineStore('myStore', () => {
    // State
    const data = ref<MyData[]>([]);
    const loading = ref<boolean>(false);
    
    // Getters
    const dataCount = computed(() => data.value.length);
    
    // Actions
    async function fetchData(): Promise<void> {
        loading.value = true;
        try {
            const result = await myApi.getData();
            if (result.success) {
                data.value = result.data;
            }
        } finally {
            loading.value = false;
        }
    }
    
    return {
        data,
        loading,
        dataCount,
        fetchData,
    };
});
```

#### 路由管理

路由在 `src/router/index.ts` 中動態產生，依據選單配置與使用者權限自動過濾。

新增路由：

```typescript
// src/router/index.ts
const routes = [
    {
        path: '/my-page',
        name: 'MyPage',
        component: () => import('@/views/MyPageView.vue'),
        meta: {
            requiresAuth: true,
            roles: ['Admin', 'User'],
        },
    },
];
```

#### 主題切換

```typescript
import { usePageStore } from '@/stores/page';

const pageStore = usePageStore();

// 切換主題
pageStore.setTheme('defaultDark'); // 'default' | 'defaultDark' | 'blue' | 'green' | 'brown'
```

#### 全域訊息

```typescript
import { useSnackbarStore } from '@/stores/snackbar';
import { useDialogStore } from '@/stores/dialog';

const snackbarStore = useSnackbarStore();
const dialogStore = useDialogStore();

// 成功訊息
snackbarStore.succSnack('操作成功');

// 錯誤訊息
snackbarStore.errSnack('操作失敗');

// 對話框
dialogStore.openDialog({
    title: '確認刪除',
    message: '確定要刪除此項目嗎？',
    onConfirm: async () => {
        // 執行刪除
    },
});
```

## API 文件

### 主要 API 端點

#### 認證與授權
- `POST /Token/Login` - 使用者登入
- `POST /Token/Refresh` - 重新整理 Token
- `POST /Token/Logout` - 登出

#### 聊天室
- `GET /ChatRoom/Query` - 取得聊天室列表
- `GET /ChatRoom/Datail/{roomId}` - 取得聊天室詳細資訊
- `POST /ChatRoom/Ask` - 傳送訊息
- `POST /ChatRoom/Rename` - 重新命名聊天室
- `POST /ChatRoom/Delete/{roomId}` - 刪除聊天室
- `GET /ChatRoom/ModelTools` - 取得可用的模型與工具

#### AI 專案管理
- `GET /ProjectMng/Query` - 取得專案列表
- `GET /ProjectMng/Detail/{projectId}` - 取得專案詳細資訊
- `POST /ProjectMng/Create` - 建立專案
- `POST /ProjectMng/Update` - 更新專案
- `POST /ProjectMng/Delete/{projectId}` - 刪除專案

#### LLM 管理
- `GET /LlmMng/Query` - 取得 LLM 列表
- `POST /LlmMng/Create` - 新增 LLM
- `POST /LlmMng/Update` - 更新 LLM
- `POST /LlmMng/Delete/{llmId}` - 刪除 LLM

#### MCP Server 管理
- `GET /McpServerMng/Query` - 取得 MCP Server 列表
- `POST /McpServerMng/Create` - 新增 MCP Server
- `POST /McpServerMng/Update` - 更新 MCP Server
- `POST /McpServerMng/Delete/{mcpServerId}` - 刪除 MCP Server

#### 使用者足跡
- `POST /Footprint/Frontend` - 記錄前端頁面軌跡

### 使用 Swagger

啟動專案後，透過瀏覽器存取：
```
https://localhost:5001/swagger
```

在 Swagger UI 中可以：
- 瀏覽所有 API 端點
- 測試 API 呼叫
- 查看請求/回應模型
- 使用 JWT Token 進行認證測試

## 已知問題與未來計劃

### 已知問題

- [x] 部分 LLM 模型在高並發時會產生延遲
- [x] Azure Blob Storage 大檔案上傳有時會失敗
- [ ] JWT Token 在某些情況下無法即時更新
- [ ] 部分 API 回應時間較長

### 未來計劃

- 優化 LLM 模型的載入與切換速度
- 增加更多 LLM 提供商的支援
- 改進使用者介面與體驗
- 加強系統的穩定性與安全性
- 提供更完整的 API 文檔與範例
- 前端增加單元測試覆蓋率
- 實作更多自訂 MCP 工具
- 改善聊天室即時互動體驗

## 授權

本專案為私有軟體，未經授權不得使用、複製或散布

© 2024 Geo Information Technology Corporation. All rights reserved.

---

**開發團隊**: Geo AI Team  
**聯絡方式**: joe@geo.com.tw  
**專案位置**: https://github.com/FCU-GitHub-Copilot/geo.smart.AiProject
