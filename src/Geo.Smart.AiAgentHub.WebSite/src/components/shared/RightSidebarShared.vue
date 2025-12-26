<script setup lang="ts">
import { storeToRefs } from 'pinia';
import { computed, ref, watch } from 'vue';

import usePageStore from '@/stores/page';
import useSnackbarStore from '@/stores/snackbar';
import { themes, type ThemesKey } from '@/themes';
import {
    mdiContentCopy,
    mdiPaletteSwatchVariant,
    mdiTag,
    mdiTagCheck,
    mdiWindowClose,
} from '@/utils/icons';
import { useClipboard } from '@vueuse/core';

const rightDrawer = ref(false);
const setRightDrawer = (value: boolean) => {
    rightDrawer.value = value;
};

const pageStore = usePageStore();
const { setThemeFromIsDark, toggleDarkMode } = pageStore;
const { theme, isDark } = storeToRefs(pageStore);

const themeNames = Object.keys(themes).filter((key) => key.includes('Light'));

// 根據 isDark 計算顯示的主題名稱
const displayedThemeNames = computed(() => themeNames.map((name) => (isDark.value ? name.replace('Light', 'Dark') : name)));

const { succSnack } = useSnackbarStore();

const { copy, text, copied } = useClipboard({ legacy: true });

watch(copied, (value) => {
    if (value) {
        succSnack(`已複製${text.value}`);
    }
});
</script>

<template>
    <v-tooltip
        text="主題色系設定"
        location="bottom"
    >
        <template #activator="{ props }">
            <v-btn
                v-bind="props"
                class="rightSidebarBtn"
                :icon="mdiPaletteSwatchVariant"
                color="tertiary"
                @click="setRightDrawer(true)"
            />
        </template>
    </v-tooltip>
    <v-navigation-drawer
        v-model="rightDrawer"
        location="right"
        temporary
        color="surface"
    >
        <template #prepend>
            <div class="d-flex align-center justify-space-between">
                <p class="text-h6 px-4">
                    主題色系設定
                </p>
                <v-btn
                    :icon="mdiWindowClose"
                    variant="text"
                    color="secondary"
                    @click="setRightDrawer(false)"
                />
            </div>
        </template>

        <v-divider />

        <div class="px-4">
            <v-switch
                :model-value="isDark"
                color="secondary"
                :label="isDark ? '深色模式' : '亮色模式'"
                hide-details
                inset
                @update:model-value="toggleDarkMode"
            />

            <v-divider />

            <p class="text-subTitle-1 my-2">
                色系
            </p>
            <div class="d-flex flex-wrap justify-space-between">
                <div
                    v-for="themeName in displayedThemeNames"
                    :key="themeName"
                    class="d-flex flex-column align-center mb-2"
                >
                    <v-btn
                        :icon="themeName === theme ? mdiTagCheck : mdiTag"
                        :disabled="themeName === theme"
                        :color="themes[themeName as ThemesKey].colors.primary"
                        @click="setThemeFromIsDark(themeName as ThemesKey)"
                    />
                    <v-tooltip
                        :text="`複製${themeName}`"
                        location="bottom"
                    >
                        <template #activator="{ props }">
                            <v-chip
                                variant="tonal"
                                size="x-small"
                                rounded="pill"
                                :append-icon="mdiContentCopy"
                                v-bind="props"
                                class="text-body-2 my-1"
                                @click="copy(themeName)"
                            >
                                {{ themeName }}
                            </v-chip>
                        </template>
                    </v-tooltip>
                </div>
            </div>

            <v-divider class="mt-2" />

            <p class="text-subTitle-1 my-2">
                說明
            </p>
            <ul class="text-body-1 ml-4">
                <li>
                    選擇適合專案的主題後，你可以直接點選對應的主題名稱複製，再到<code>src/stores/page.js</code>中修改<code>theme</code>的值，即可套用預設主題。
                </li>
                <li>
                    目前你所選擇的模式與主題將會被儲存於<code>localStorage</code>中，開啟專案時將會自動套用。
                </li>
            </ul>
        </div>
    </v-navigation-drawer>
</template>

<style lang="sass" scoped>
.rightSidebarBtn
    position: fixed
    right: .5rem
    top: 20vw
    z-index: 3
</style>
