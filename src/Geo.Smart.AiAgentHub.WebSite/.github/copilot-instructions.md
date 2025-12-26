# Copilot AI 指南：Geo.Smart.AdminHub.WebSite

## 主要指令

- `yarn dev`：本地開發模式（Vite 啟動，含熱重新整理）
- `yarn stag`：測試環境建構（型別檢查+Vite）
- `yarn build`：生產環境建構
- `yarn preview`：預覽建構結果
- `npx eslint .`：執行 ESLint 檢查
- `npx vue-tsc --noEmit`：型別檢查

> 無內建單元測試/文件產生/遷移指令，請依需求擴充。

---

## 架構與核心元件

- **前端框架**：Vue 3 (Composition API)
- **型別系統**：TypeScript 5.8+
- **UI 函式庫**：Vuetify 3 (Material Design 3)
- **狀態管理**：Pinia
- **路由管理**：Vue Router 4
- **建構工具**：Vite 6
- **API 請求**：Axios（`src/utils/axios`，含全域攔截、權限/逾時處理）
- **主題系統**：多主題（`src/themes`，Pinia 控制）
- **功能開關**：`src/composables/featureFlag`（可動態啟用/停用功能）
- **登入/權限**：JWT 驗證、角色型權限（`src/stores/user`、`src/types/models/role.ts`）
- **全域元件註冊**：於 `src/plugins/index.ts` 統一註冊
- **API 常數**：`src/api/constants.ts`，所有 API 路徑集中管理

### 主要資料流

- **Pinia Store**：`user`（登入/權限）、`page`（主題/佈局）、`profile`（個人資訊）、`dialog`/`snackbar`（全域訊息）、`inactivityTimer`（閒置自動登出）
- **API**：`src/api/index.ts` 封裝所有後端呼叫，統一錯誤處理
- **路由**：`src/router/index.ts` 動態產生，依選單/權限自動過濾

---

## 風格與規範

- **縮排**：4 空格
- **字串**：單引號
- **結尾**：每行必須加分號
- **命名**：
    - Vue 元件/型別/介面：PascalCase
    - 變數/函式/檔案：camelCase
    - 常數：SCREAMING_SNAKE_CASE
    - 路由名稱：PascalCase，路徑 kebab-case
- **型別**：所有函式/變數必須明確型別
- **匯入順序**：Node 內建 → 第三方 → @ alias 專案內
- **ESLint**：
    - TypeScript/Vue 3 推薦規則
    - 嚴格模式、禁止多餘空行/空格/行尾空格
    - Vue 組件名稱/模板 casing 強制 PascalCase
- **Vue 組件**：
    - 使用 `<script setup lang="ts">`
    - 組合式 API，響應式資料用 ref/reactive
    - Props/Emits 型別明確
    - 重要函式加 JSDoc
    - 可複用邏輯抽為 composable
- **錯誤處理**：API/非同步操作皆需 try/catch，錯誤訊息統一顯示於 snackbar/dialog

---

## 外部服務/依賴

- **後端 API**：所有資料存取皆透過 RESTful API，路徑集中於 `src/api/constants.ts`
- **第三方函式庫**：@smart/vue-*（表單、表格、上傳、Dialog、Snackbar 等）、axios、jose、@vueuse/core

---

## 重要專案規則摘要

- **主題切換**：`usePageStore().setTheme('defaultDark')` 控制
- **功能開關**：`useFeatureFlag().isFeatureEnabled('featureName')`
- **登入/逾時**：JWT 驗證，逾時自動彈窗登出
- **選單/權限**：MenuItem 依角色自動過濾，路由自動產生
- **分頁/查詢參數**：`useStorageParams`（SessionStorage 快取）
- **全域訊息**：`useSnackbarStore().succSnack/errSnack`、`useDialogStore().openDialog`

---

## 其他

- **環境變數**：`VITE_APP_TITLE`、`VITE_APP_PATH`、`NODE_ENV`、`VITE_APP_API`
- **Sass**：全域樣式於 `src/assets/sass/main.sass`
- **型別路徑 alias**：`@` 對應 `src/`
