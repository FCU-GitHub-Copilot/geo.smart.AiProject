<!-- eslint-disable vue/attribute-hyphenation -->
<script setup lang="ts">
import { computed, ref } from 'vue';
import { fieldType, useVerify } from '@smart/vue-form';

import type { ProjectEdit } from '@/types/api/projectMng';
import type { VuetifyForm, InfoListItem } from '@/types/models/common';
import type { McpServerQuery } from '@/types/api/mcpServerMng';
import type { LlmQuery } from '@/types/api/llmMng';

type Props = {
    llmList: LlmQuery[];
    mcpServerList: McpServerQuery[];
};

type Emit = {
    'on:submit': [];
    'on:back': [];
    'open:chat': [];
};

const { llmList, mcpServerList } = defineProps<Props>();
const emits = defineEmits<Emit>();
const formRef = ref<VuetifyForm>(null);

const onSubmit = async () => {
    if (!formRef.value) return;
    const { valid } = await formRef.value.validate();
    if (!valid) return;
    emits('on:submit');
};

const params = defineModel<ProjectEdit>({
    default: () => ({}),
});

const { common } = useVerify();

const ruleTemperature = (v?: string) => {
    if (!v) return true;
    const num = Number(v);
    if (Number.isNaN(num)) return '請輸入數字';
    if (num < 0 || num > 2) return '請輸入 0 到 2 之間的數字';
    return true;
};

const ruleTopP = (v?: string) => {
    if (!v) return true;
    const num = Number(v);
    if (Number.isNaN(num)) return '請輸入數字';
    if (num < 0.1 || num > 2) return '請輸入 0.1 到 2 之間的數字';
    return true;
};

const infoList = computed<InfoListItem<ProjectEdit>[]>(() => ([
    {
        title: '名稱​',
        key: 'name',
        type: fieldType.eInput,
        required: true,
        rules: [common.required],
    },
    {
        title: 'LLM 模型',
        key: 'llmIds',
        type: fieldType.eSelect,
        list: llmList,
        multiple: true,
        itemTitle: 'serviceId',
        itemValue: 'llmId',
    },
    {
        title: 'MCP Server',
        key: 'mcpServerIds',
        type: fieldType.eSelect,
        list: mcpServerList,
        multiple: true,
        itemTitle: 'name',
        itemValue: 'mcpServerId',
    },
    {
        title: '說明',
        key: 'description',
        type: fieldType.eArea,
    },
    {
        title: '提示詞',
        key: 'systemPrompt',
        type: fieldType.eArea,
        required: true,
        rules: [common.required],
    },
    {
        title: 'Temperature',
        key: 'temperature',
        type: fieldType.eInput,
        rules: params.value?.temperature ? [ruleTemperature] : [],
        hint: '溫度，控制 LLM 的創造力，範圍 0 到 2 之間',
    },
    {
        title: 'TopP',
        key: 'topP',
        type: fieldType.eInput,
        rules: params.value?.topP ? [ruleTopP] : [],
        hint: '控制 LLM 文本生成的機率篩選器，範圍 0.1 到 2 之間',
    },
    {
        title: 'TopK',
        key: 'topK',
        type: fieldType.eInput,
        rules: params.value?.topK ? [common.isNonNegative] : [],
        hint: 'LLM 只會從機率最高的 k 個 Tokens 中進行選擇',
    },
    {
        title: 'MaxTokens',
        key: 'maxTokens',
        type: fieldType.eInput,
        rules: params.value?.maxTokens ? [common.isNonNegative] : [],
        hint: '最大的 token 數量',
    },
]));

const isDetail = computed(() => !!params.value?.projectId);

const submitBtn = computed(() => (isDetail.value ? '儲存' : '新增'));

</script>

<template>
    <v-form ref="formRef">
        <v-container>
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
                        chips
                        :items="item.list"
                        :item-title="item.itemTitle"
                        :item-value="item.itemValue"
                        :multiple="item.multiple"
                        :rules="item.required ? [common.requiredAllowZero] : []"
                        size="xLarge"
                    />
                    <STextField
                        v-if="item.type === fieldType.eInput"
                        v-model="params[item.key]"
                        :rules="item.rules"
                        :placeholder="item.placeholder"
                    />
                    <v-textarea
                        v-if="item.type === fieldType.eArea"
                        v-model="params[item.key]"
                        :rules="item.rules"
                    />
                    <p
                        v-if="item.hint"
                        class="mt-1"
                    >
                        {{ item.hint }}
                    </p>
                </v-col>
            </v-row>
            <v-row justify="end">
                <v-col cols="auto">
                    <v-btn
                        v-show="isDetail"
                        color="secondary"
                        class="mr-2"
                        @click="emits('open:chat')"
                    >
                        開啟聊天室
                    </v-btn>
                    <v-btn
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
        </v-container>
    </v-form>
</template>

<style scoped>

</style>
