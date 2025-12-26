# Vue3 管理後台專案開發指引

## 專案概述
這是一個基於 Vue 3 + Vuetify 3 + Vite 的管理後台專案，採用前後端分離架構，使用 Pinia 進行狀態管理，並整合多個自訂元件庫。

## 框架與元件庫
- **Vue 3** (v3.5.16) - 漸進式 JavaScript 框架，使用 Composition API
- **Vuetify 3** (v3.8.8) - Material Design 元件庫
- **@vueuse/core** (v13.3.0) - Vue 組合式函式集合
- **@mdi/js** (v7.4.47) - Material Design Icons
- **Pinia** (v3.0.3) - Vue 3 官方狀態管理庫
- **Vite** (v6.3.5) - 新一代前端建構工具
- **ESLint** (v9.28.0) - JavaScript 程式碼檢查工具
- **Sass** (v1.89.1) - CSS 預處理器
- **Axios** (v1.9.0) - HTTP 請求庫

## 專案結構

```
src/
├── api/                    # API 接口定義
├── assets/                 # 靜態資源
│   └── sass/              # SASS 樣式檔案
├── components/            # 元件
│   ├── login/             # 登入相關元件
│   └── shared/            # 共用元件
├── composables/           # 組合式函式
├── plugins/               # 外掛程式
├── router/                # 路由設定
├── stores/                # Pinia 狀態管理
├── themes/                # 主題設定
├── utils/                 # 工具函式
├── views/                 # 頁面元件
├── App.vue               # 根元件
└── main.js               # 應用程式入口
```

## 程式碼風格規範

### 命名規則

#### 檔案命名
- **Vue 元件檔案**: 使用 PascalCase (例: `HeaderShared.vue`, `PageLayout.vue`)
- **JavaScript 檔案**: 使用 camelCase (例: `index.js`, `featureFlag.js`)
- **資料夾**: 使用 camelCase (例: `composables`, `stores`)

#### 變數命名
- **一般變數**: 使用 camelCase (例: `userName`, `isLogin`)
- **常數**: 使用 UPPER_SNAKE_CASE (例: `TOKEN_KEY`, `SYSTEM_NAME`)
- **函式**: 使用 camelCase，動詞開頭 (例: `setRole`, `jwtDecode`)

#### 元件命名
- **全域元件**: 使用 PascalCase 並加上前綴 (例: `SFullLoading`, `SSnackBar`)
- **區域元件**: 使用 PascalCase (例: `PageLayout`, `HeaderShared`)

### 程式碼格式

#### ESLint 規則
- 縮排: 4 個空格
- 引號: 單引號
- 分號: 必須使用
- 禁止多餘空行
- 禁止行尾空格

#### Vue 元件結構
```vue
<script setup>
// 1. 匯入套件
import { ref, computed } from 'vue';

// 2. 匯入自訂模組
import useUserStore from '@/stores/user';

// 3. 定義 props 和 emits
const props = defineProps({
    title: String,
});

const emit = defineEmits(['update']);

// 4. 響應式資料
const isVisible = ref(false);

// 5. 計算屬性
const displayTitle = computed(() => props.title || '預設標題');

// 6. 方法
const handleClick = () => {
    emit('update', true);
};
</script>

<template>
    <!-- 模板內容 -->
</template>

<style lang="sass" scoped>
/* 樣式 */
</style>
```

## 架構設計

### 狀態管理 (Pinia)

#### Store 檔案結構
```javascript
import { defineStore } from 'pinia';
import { ref, computed } from 'vue';

const useExampleStore = defineStore('example', () => {
    // 狀態
    const state = ref(null);
    
    // 計算屬性
    const computedValue = computed(() => state.value);
    
    // 動作
    const setState = (value) => {
        state.value = value;
    };
    
    return {
        state,
        computedValue,
        setState,
    };
});

export default useExampleStore;
```

### 路由設計

#### 路由設定原則
- 使用 `generateRoutes` 函式動態生成路由
- 支援多層選單結構
- 整合權限控制 (`roles`)
- 統一使用具名路由
- 支援路由元資訊 (`meta`)

#### 選單結構
```javascript
{
    id: '唯一識別碼',
    title: '選單標題',
    routeName: '路由名稱',
    icon: '圖示',
    path: '路由路徑',
    component: () => import('元件路徑'),
    roles: ['角色陣列'],
    subMenus: [], // 子選單
    skipRegist: false, // 是否跳過路由註冊
}
```

### API 呼叫

#### API 檔案結構
```javascript
// api/constants.js - API 端點常數
export const AUTH = {
    LOGIN: '/auth/login',
    REFRESH: '/auth/refresh',
};

// api/index.js - API 函式
export const apiLogin = (params) => request.post(AUTH.LOGIN, params);
```

#### 錯誤處理
- 統一在 `axios` 攔截器處理
- 使用 `snackbar` 顯示錯誤訊息
- 自動處理 token 過期

### 元件設計

#### 頁面佈局
- 使用 `PageLayout` 作為頁面容器
- 支援具名插槽 (`btn`, `default`)
- 自動生成頁面標題

#### 表單元件
- 使用 `@smart/vue-form` 套件
- 統一驗證規則
- 支援多種輸入類型

#### 表格元件
- 使用 `@smart/vue-table` 套件
- 支援搜尋、分頁、排序
- 統一的資料格式

## 開發規範

### Composables 設計

#### 檔案結構
```javascript
// composables/example/index.js
export default function useExample() {
    const state = ref(null);
    
    const method = () => {
        // 邏輯處理
    };
    
    return {
        state,
        method,
    };
}
```

#### 命名規則
- 檔案名稱使用 camelCase
- 函式名稱使用 `use` 前綴
- 回傳物件使用解構

### 主題系統

#### 主題檔案結構
```javascript
// themes/themeName.js
export const light = {
    colors: {
        primary: '#色碼',
        // 其他顏色定義
    },
};

export const dark = {
    colors: {
        primary: '#色碼',
        // 其他顏色定義
    },
};
```

### 工具函式

#### 檔案組織
- 按功能分類到不同資料夾
- 每個模組匯出純函式
- 避免副作用

### 環境變數

#### 命名規則
- 使用 `VITE_` 前綴
- 使用 UPPER_SNAKE_CASE
- 按功能分組

```env
# 應用程式設定
VITE_APP_TITLE=應用程式名稱
VITE_APP_PATH=/

# API 設定
VITE_API_BASE_URL=https://api.example.com
```

### 文件
1. 為複雜函式添加 JSDoc 註解
2. README 檔案說明專案設定
3. API 文件與後端同步更新

## 常用指令

```bash
# 開發環境
yarn dev

# 建置測試環境
yarn stag

# 建置正式環境
yarn build

# 預覽建置結果
yarn preview
```

## 注意事項

1. 遵循 ESLint 規則，提交前確保無警告
2. 新增功能時更新相關文件
4. 定期更新相依套件
