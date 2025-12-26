<script setup lang="ts">
import { computed, ref } from 'vue';
import { fieldType, useVerify } from '@smart/vue-form';

import type { McpServer } from '@/types/api/mcpServerMng';
import {
    McpServerType,
    type VuetifyForm,
    type InfoListItem,
} from '@/types/models/common';

import useCommonStore from '@/stores/common';

type Emit = {
    'on:submit': [];
    'on:back': [];
};

const emits = defineEmits<Emit>();
const formRef = ref<VuetifyForm>(null);

const onSubmit = async () => {
    if (!formRef.value) return;
    const { valid } = await formRef.value.validate();
    if (!valid) return;
    emits('on:submit');
};

const params = defineModel<McpServer>({
    default: () => ({}),
});

const { common } = useVerify();
const commonStore = useCommonStore();

// 英數字與底線驗證
const namePatternRule = (v?: string): boolean | string => {
    if (!v) return true;
    return /^[A-Za-z0-9_]+$/.test(v) || '僅可輸入英數字與底線';
};

const isStdio = computed(() => params.value?.mcpServerType === McpServerType.Stdio);

const infoList = computed(() => {
    const defaultList: InfoListItem<McpServer>[] = [
        {
            title: '名稱',
            key: 'name',
            type: fieldType.eInput,
            required: true,
            rules: [common.required, namePatternRule],
            hint: '僅可輸入英數字與底線',
        },
        {
            title: 'MCP 類型​',
            key: 'mcpServerType',
            type: fieldType.eSelect,
            list: commonStore.mcpServerTypeList,
            required: true,
            itemTitle: 'name',
            itemValue: 'key',
        },
        {
            title: '服務位置​',
            key: 'sseUrl',
            type: fieldType.eInput,
            hint: '伺服器的 SSE 端點 URL',
            required: true,
            rules: [common.required],
        },
        {
            title: 'StdioCommand',
            key: 'stdioCommand',
            type: fieldType.eInput,
            disabled: true,
        },
        {
            title: 'StdioArgs',
            key: 'stdioArgs',
            type: fieldType.eInput,
            disabled: true,
        },
        {
            title: 'StdioEnv',
            key: 'stdioEnv',
            type: fieldType.eInput,
            disabled: true,
        },
    ];

    const removeList = isStdio.value
        ? ['sseUrl']
        : ['stdioCommand', 'stdioArgs', 'stdioEnv'];

    return defaultList.filter((item) => !removeList.includes(item.key));
});

const submitBtn = computed(() => (params.value?.mcpServerId ? '儲存' : '新增'));


</script>

<template>
    <v-form ref="formRef">
        <v-row
            v-for="(item, index) in infoList"
            :key="index"
        >
            <s-form-title
                :required="item.required"
            >
                {{ item.title }}
            </s-form-title>
            <v-col>
                <SSelector
                    v-if="item.type === fieldType.eSelect"
                    v-model="params[item.key]"
                    :items="item.list"
                    :item-title="item.itemTitle"
                    :item-value="item.itemValue"
                    :rules="item.required ? [common.requiredAllowZero] : []"
                    size="xLarge"
                />
                <STextField
                    v-if="item.type === fieldType.eInput"
                    v-model="params[item.key]"
                    :rules="item.rules"
                    :placeholder="item.placeholder"
                    :disabled="item.disabled"
                />
                <p
                    v-if="item.hint"
                    class="mt-1"
                >
                    {{ item.hint }}
                </p>
            </v-col>
        </v-row>
    </v-form>
    <v-row justify="end">
        <v-col cols="auto">
            <v-btn
                v-show="!isStdio"
                class="mr-2"
                @click="onSubmit"
            >
                {{ submitBtn }}
            </v-btn>
            <v-back-btn @click="emits('on:back')">
                返回
            </v-back-btn>
        </v-col>
    </v-row>
</template>

<style scoped>

</style>
