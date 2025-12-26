import type { Pagination } from '@smart/vue-table';
import type { Ref } from 'vue';

/**
 * 儲存參數的介面
 */
export interface StorageParams {
    [key: string]: unknown;
}

/**
 * useStorageParams 函數的參數介面
 */
export interface UseStorageParamsOptions {
    /** 儲存參數的 key */
    storageKey?: string;
    /** 參數 Ref 物件 */
    paramsRef: StorageParams;
    /** 分頁 Ref 物件 */
    paginationRef: Pagination;
}

/**
 * useStorageParams 函數的返回型別介面
 */
export interface UseStorageParamsReturn {
    /** 儲存參數 */
    storageParams: Ref<StorageParams>;
    /** 是否有儲存參數 */
    hasStorageParams: Ref<boolean>;
    /**
     * 設定儲存參數
     * @param params {StorageParams} 儲存參數
     */
    setStorageParams: (params: StorageParams) => void;
    /**
     * 清除儲存參數
     */
    clearStorageParams: () => void;
    /**
     * 將儲存參數賦值給分頁Ref物件
     */
    handleStorageParams: () => void;
}
