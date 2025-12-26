interface DebouncedFunction<T extends (...args: unknown[]) => unknown> {
    (...args: Parameters<T>): void;
    cancel(): void;
}

/**
 * 延遲執行函式，用於限制函式的執行頻率。
 *
 * @param {Function} func - 要執行的函式。
 * @param {number} wait - 延遲時間（毫秒）。
 * @param {boolean} [immediate=false] - 是否立即執行函式。
 * @returns {Function} - 延遲執行的函式。
 */
const debounce = <T extends (...args: unknown[]) => unknown>(
    func: T,
    wait: number,
    immediate: boolean = false,
): DebouncedFunction<T> => {
    let timeout: ReturnType<typeof setTimeout> | null;

    const debounced: DebouncedFunction<T> = (...args: Parameters<T>) => {
        const later = () => {
            timeout = null;
            if (!immediate) func(...args);
        };

        const callNow = immediate && !timeout;

        if (timeout) {
            clearTimeout(timeout);
        }

        timeout = setTimeout(later, wait);

        if (callNow) func(...args);
    };

    /**
     * 取消待執行的函數
     */
    debounced.cancel = () => {
        if (timeout) {
            clearTimeout(timeout);
            timeout = null;
        }
    };

    return debounced;
};

export default debounce;
