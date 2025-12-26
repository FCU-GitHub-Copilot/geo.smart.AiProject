<script setup>
import { ref, computed } from 'vue';
import { mdiEye, mdiEyeOff } from '@/utils/icons';

defineProps({
    placeholder: {
        type: String,
        default: '請輸入密碼',
    },
    rules: {
        type: Array,
        default: () => [],
    },
    size: {
        type: String,
        default: 'full',
    },
});

const isShowPassword = ref(false);
const toggleShowPassword = () => {
    isShowPassword.value = !isShowPassword.value;
};

const inputType = computed(() => (isShowPassword.value ? 'text' : 'password'));

const model = defineModel({ type: String, default: '' });
</script>

<template>
    <s-text-field
        v-model="model"
        :rules="rules"
        :size="size"
        :type="inputType"
        :placeholder="placeholder"
    >
        <template #append-inner>
            <v-tooltip
                :text="isShowPassword ? '不顯示密碼' : '顯示密碼'"
                location="top"
            >
                <template #activator="{ props }">
                    <v-icon
                        v-bind="props"
                        :icon="isShowPassword ? mdiEyeOff : mdiEye"
                        color="grey-darken-1"
                        @click="toggleShowPassword"
                    />
                </template>
            </v-tooltip>
        </template>
    </s-text-field>
</template>
