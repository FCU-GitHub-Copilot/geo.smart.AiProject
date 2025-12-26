export interface UseFeatureFlagReturn {
    /**
     * 檢查功能是否啟用
     * @param {string} featureName - 功能名稱
     * @returns {boolean} 功能是否啟用
     */
    isFeatureEnabled: (featureName: string) => boolean;
    /**
     * 啟用功能
     * @param {string} featureName - 功能名稱
     */
    enableFeature: (featureName: string) => void;
    /**
     * 禁用功能
     * @param {string} featureName - 功能名稱
     */
    disableFeature: (featureName: string) => void;
}
