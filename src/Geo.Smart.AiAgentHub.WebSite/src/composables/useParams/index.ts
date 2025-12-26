import { useSessionStorage } from '@vueuse/core';
import { computed } from 'vue';
import type {
    StorageParams,
    UseStorageParamsOptions,
    UseStorageParamsReturn,
} from './types';

// 系統名稱，從環境變數中獲取
const SYSTEM_NAME = import.meta.env.VITE_APP_TITLE;
const STORAGE_KEY = `${SYSTEM_NAME}_`;

/**
 * useStorageParams 儲存列表分頁狀態
 */
const useStorageParams = ({
    storageKey = 'storageParams',
    paramsRef,
    paginationRef,
}: UseStorageParamsOptions): UseStorageParamsReturn => {
    const storageParams = useSessionStorage<StorageParams>(
        STORAGE_KEY + storageKey,
        {},
    );

    const hasStorageParams = computed(
        () => !!storageParams.value && !!Object.keys(storageParams.value).length,
    );

    const setStorageParams = (params: StorageParams) => {
        storageParams.value = params;
    };

    const clearStorageParams = () => {
        storageParams.value = null;
    };

    const handleStorageParams = () => {
        if (hasStorageParams.value) {
            Object.assign(paramsRef, storageParams.value);
            paginationRef.pageSize = paramsRef.pageSize as number;
            paginationRef.currentPage = paramsRef.currentPage as number;
        }
    };

    return {
        storageParams,
        hasStorageParams,
        setStorageParams,
        clearStorageParams,
        handleStorageParams,
    };
};

export default useStorageParams;
