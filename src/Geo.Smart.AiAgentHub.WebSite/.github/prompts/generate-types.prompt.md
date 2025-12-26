---
mode: 'edit'
description: '由 swagger.json 產生 TypeScript 型別定義'
---

# 產生 TypeScript 型別定義

- 請根據 `swagger.json` 檔案和指定功能需求，產生 TypeScript 型別定義
- 符合 `eslint.config.mjs` 與 `tsconfig.json` 的規範
- 所有介面和型別都必須包含 JSDoc 註解
- 所有屬性都必須明確定義為必填或選填
- 使用 `readonly` 修飾詞保護不可變資料
- 實作泛型型別支援可重用的資料結構
- 使用條件型別處理複雜的業務邏輯
- 定義工具型別 (utility types) 提升開發效率
- 日期時間：統一使用 `string` 型別，並加上 JSDoc 註解說明格式
- 檔案上傳：定義 `File` 和 `FormData` 相關型別

## 型別目錄
```
types/
├── index.d.ts        # 匯出所有型別
├── api/              # API 相關型別
│   ├── auth.d.ts     # 認證 API 型別
│   ├── news.d.ts     # 新聞 API 型別
│   └── request.d.ts  # 請求型別
├── common/           # 共用型別
│   ├── pagination.d.ts # 分頁型別
│   └── response.d.ts   # 回應型別
├── models/           # 資料模型型別
│   ├── auth.d.ts     # 認證模型
│   ├── news.d.ts     # 新聞模型
│   └── role.d.ts     # 角色模型
├── system/           # 系統型別
│   ├── nuxt.d.ts     # Nuxt 相關型別
│   └── vue-format.d.ts # Vue 格式化型別
└── ui/               # UI 相關型別
    └── menu.d.ts     # 選單型別
```