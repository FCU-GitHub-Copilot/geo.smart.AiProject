import useDialogStore from '@/stores/dialog';
import useUserStore from '@/stores/user';
import { logout, refreshToken } from '@/utils/auth';
import debounce from '@/utils/debounce';
import { useStorage } from '@vueuse/core';
import { decodeJwt } from 'jose';
import { defineStore } from 'pinia';
import { computed, ref, watch } from 'vue';

// 系統名稱，從環境變數中獲取
const SYSTEM_NAME = import.meta.env.VITE_APP_TITLE;

// localStorage 計時器重設時間戳 key
const RESET_AT = `${SYSTEM_NAME}_resetAt`;

// 設定閒置和自動刷新 Token 的時間閾值
const INACTIVITY_THRESHOLD = 30 * 60 * 1000; // 閒置超過30分鐘自動登出
const AUTO_RENEW_THRESHOLD = 10 * 60 * 1000; // Token 自動刷新時間閾值為10分鐘

// 使用 localStorage 保存計時器重設時間戳
const resetAt = useStorage(RESET_AT, '');
/**
 * 用於處理 localStorage 的 計時器重設時間戳
 */
function saveResetTimestamp() {
    resetAt.value = Date.now().toString();
}

/**
 * 計時器，用於處理閒置超時計時器
 * @param {Number} timeout 閒置超時時間
 * @param {Function} onTimeout 閒置超時callback
 * @returns {Object} - 包含計時器的狀態和方法
 */
function useTimer(timeout: number, onTimeout: () => void) {
    const remainingTime = ref(timeout);
    const timer = ref<ReturnType<typeof setInterval> | null>(null);
    function stopTimer() {
        if (timer.value) {
            clearInterval(timer.value);
            timer.value = null;
        }
    }
    function updateCountdown() {
        remainingTime.value = Math.max(0, remainingTime.value - 1000);
        if (remainingTime.value <= 0) {
            stopTimer();
            onTimeout();
        }
    }
    function startTimer() {
        stopTimer();
        timer.value = setInterval(updateCountdown, 1000);
    }
    function resetTimer() {
        saveResetTimestamp();
        remainingTime.value = timeout;
        startTimer();
    }
    return {
        remainingTime,
        startTimer,
        stopTimer,
        resetTimer,
    };
}

// 閒置超時處理函數
const handleInactivity = () => {
    const { openDialog, toggleDialog } = useDialogStore();
    openDialog({
        title: '登入逾時',
        content: '您已閒置超過30分鐘，請重新登入。',
        submitAction: () => {
            toggleDialog();
            logout();
        },
    });
};

/**
 * Token 刷新處理函數
 * @returns {Promise<Boolean>} - 是否刷新成功
 */
const handleTokenRefresh = async (): Promise<boolean> => {
    const isRefreshSucc = await refreshToken();
    return isRefreshSucc;
};

export default defineStore('inactivityTimer', () => {
    const userStore = useUserStore();
    const token = computed(() => userStore.token);

    // Token 過期時間
    const tokenExpiresAt = computed(() => {
        if (!token.value) return 0;
        const decoded = decodeJwt(token.value);
        return (decoded.iat ? decoded.iat * 1000 : 0) + INACTIVITY_THRESHOLD;
    });

    const isInactive = ref(false); // 是否已閒置

    const {
        remainingTime, startTimer, stopTimer, resetTimer,
    } = useTimer(
        INACTIVITY_THRESHOLD,
        () => {
            isInactive.value = true;
        },
    );

    const minutes = computed(() => Math.floor(remainingTime.value / 60000)); // 剩餘的分鐘數
    const seconds = computed(() => Math.floor((remainingTime.value % 60000) / 1000)
        .toString()
        .padStart(2, '0')); // 剩餘的秒數

    /**
     * 用戶活動處理器，使用防抖函數來防止頻繁重置計時器
     */
    const handleUserActivity = debounce(async () => {
        const now = Date.now();
        const timeRemaining = tokenExpiresAt.value - now;
        const inactivityDuration = INACTIVITY_THRESHOLD - timeRemaining;

        if (isInactive.value) return;

        if (timeRemaining <= 0) {
            isInactive.value = true;
            return;
        }
        if (inactivityDuration >= AUTO_RENEW_THRESHOLD) {
            const isRefreshSucc = await handleTokenRefresh();
            if (!isRefreshSucc) {
                isInactive.value = true;
            }
        } else {
            resetTimer();
        }
    }, 600);

    /**
     * localStorage變更處理器，當其他窗口重設計時器時同步更新
     */
    const handleResetATStorageChange = () => {
        if (tokenExpiresAt.value && tokenExpiresAt.value <= Date.now()) {
            isInactive.value = true;
            return;
        }
        remainingTime.value = INACTIVITY_THRESHOLD;
        startTimer();
    };

    let isListenerAdded = false; // 用來標記事件監聽器是否已經添加

    /**
     * 管理事件監聽器
     * @param {Boolean} add - 是否添加事件監聽器
     */
    const manageEventListeners = (add: boolean = true) => {
        const method = add ? 'addEventListener' : 'removeEventListener';
        window[method]('mousedown', handleUserActivity as () => void);
        window[method]('keydown', handleUserActivity as () => void);
        window[method]('touchstart', handleUserActivity as () => void);
        window[method]('visibilitychange', handleUserActivity as () => void);
        isListenerAdded = add;
    };

    /**
     * 初始化計時器
     */
    const initializeTimer = (isFirstLoad: boolean) => {
        if (!token.value) {
            isInactive.value = true;
            return;
        }
        // Token 已過期，設置為閒置狀態並返回
        if (tokenExpiresAt.value <= Date.now()) {
            isInactive.value = true;
            return;
        }

        // 第一次載入頁面時，先觸發一次用戶活動處理器，檢查是否需要自動刷新 Token
        if (isFirstLoad) {
            handleUserActivity();
        } else {
            remainingTime.value = INACTIVITY_THRESHOLD;
            startTimer();
        }

        if (!isListenerAdded) manageEventListeners(true);
    };

    /**
     * 銷毀計時器
     */
    const destoryTimer = () => {
        remainingTime.value = 0;
        stopTimer();
        manageEventListeners(false);
        resetAt.value = '';
    };

    watch(token, (value) => {
        if (value) {
            isInactive.value = false;
            initializeTimer(false);
        } else {
            destoryTimer();
        }
    });

    watch(isInactive, (value) => {
        if (value) {
            destoryTimer();
            if (token.value) handleInactivity();
        }
    });

    watch(resetAt, (value) => {
        if (value) {
            handleResetATStorageChange();
        }
    });

    return {
        isInactive,
        remainingTime,
        minutes,
        seconds,
        initializeTimer,
        destoryTimer,
    };
});
