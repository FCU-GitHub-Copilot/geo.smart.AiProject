<script setup lang="ts">
import { computed, ref } from 'vue';
import { fieldType, useVerify } from '@smart/vue-form';

import useCommonStore from '@/stores/common';
import type { LlmEdit } from '@/types/api/llmMng';
import {
    LlmSourceType,
    type VuetifyForm,
    type InfoListItem,
} from '@/types/models/common';

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
const params = defineModel<LlmEdit>({
    default: () => ({}),
});

const { common } = useVerify();
const commonStore = useCommonStore();

const isApiKeyRequired = computed(() => {
    const requiresList = [LlmSourceType.OpenAi, LlmSourceType.AzureOpenAi, LlmSourceType.Gemini, LlmSourceType.Afs];
    if (!params.value.llmSourceType) return false;
    return requiresList.includes(params.value.llmSourceType);
});

const isDeploymentNameRequired = computed(() => params.value?.llmSourceType === LlmSourceType.AzureOpenAi);

const infoList = computed<InfoListItem<LlmEdit>[]>(() => ([
    {
        title: '模型管理名稱',
        key: 'serviceId',
        type: fieldType.eInput,
        required: true,
        rules: [common.required],
    },
    {
        title: 'LLM 模型名稱',
        key: 'modelId',
        type: fieldType.eInput,
        required: true,
        rules: [common.required],
    },
    {
        title: 'LLM 服務來源類型',
        key: 'llmSourceType',
        type: fieldType.eSelect,
        list: commonStore.llmSourceTypeList,
        required: true,
        itemTitle: 'name',
        itemValue: 'key',
    },
    {
        title: 'API 金鑰',
        key: 'apiKey',
        required: isApiKeyRequired.value,
        rules: isApiKeyRequired.value ? [common.required] : [],
        type: fieldType.eInput,
    },
    {
        title: '端點網址',
        key: 'endpoint',
        type: fieldType.eInput,
    },
    {
        title: '部署名稱',
        key: 'deploymentName',
        required: isDeploymentNameRequired.value,
        rules: isDeploymentNameRequired.value ? [common.required] : [],
        type: fieldType.eInput,
    },
    {
        title: '說明',
        key: 'description',
        type: fieldType.eInput,
    },
]));

const submitBtn = computed(() => (params.value?.llmId ? '儲存' : '新增'));

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
                />
            </v-col>
        </v-row>
    </v-form>
    <v-row justify="end">
        <v-col cols="auto">
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
</template>

<style scoped>

</style>
