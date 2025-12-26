import { ref } from 'vue';
import type { UseFeatureFlagReturn } from './types';

/**
 * 使用功能標誌
 * @returns {Object} 包含檢查、啟用和禁用功能的方法
 */
export default function useFeatureFlag(): UseFeatureFlagReturn {
    const flags = ref<{ [key: string]: boolean }>({
        themeRightSidebar: true, // 右側欄功能
    });

    /**
     * 檢查功能是否啟用
     * @param {string} featureName - 功能名稱
     * @returns {boolean} 功能是否啟用
     */
    const isFeatureEnabled = (featureName: string) => flags.value[featureName] ?? false;

    /**
     * 啟用功能
     * @param {string} featureName - 功能名稱
     */
    const enableFeature = (featureName: string) => {
        if (flags.value[featureName] !== undefined) {
            flags.value[featureName] = true;
        }
    };

    /**
     * 禁用功能
     * @param {string} featureName - 功能名稱
     */
    const disableFeature = (featureName: string) => {
        if (flags.value[featureName] !== undefined) {
            flags.value[featureName] = false;
        }
    };

    return {
        isFeatureEnabled,
        enableFeature,
        disableFeature,
    };
}
