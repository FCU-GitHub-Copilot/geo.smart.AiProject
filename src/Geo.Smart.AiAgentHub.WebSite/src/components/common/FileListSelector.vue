<script setup lang="ts">
import useDialogStore from '@/stores/dialog';

import type { FileUpload } from '@smart/vue-uploadfile';

type Item = {
    [key: string]: unknown,
};

type Props = {
    list: Item[],
    isAdd?: boolean,
    isDelete?: boolean,
    uploadBtnTitle?: string,
    limitMbSize?: number,
    extensions?: string[],
};

type Emit = {
    'update:file': [f:FileUpload],
    'delete:file': [fileId:string],
    'download:file': [fileId:string, fileName?:string],
};

const {
    uploadBtnTitle = '文件上傳',
    limitMbSize = 15,
    extensions = ['PDF', 'DOCX', 'TIFF', 'JPG', 'JPEG'],
} = defineProps<Props>();
const emits = defineEmits<Emit>();

const headerList = [
    {
        name: '上傳日期',
        key: 'createdDate',
        slot: 'date',
    },
    {
        name: '檔案名稱',
        key: 'fileName',
    },
    {
        name: '檔案說明',
        class: 'text-truncate',
        key: 'note',
    },
    {
        name: ' ',
        key: 'fileId',
        slot: 'actions',
    },
];

const uploadFile = async (f: FileUpload) => {
    if (!f.file) return;
    emits('update:file', f);
};

const { openDialog, toggleDialog } = useDialogStore();

const deleteAlert = (fileId: string) => {
    openDialog({
        title: '刪除檔案',
        content: '確定要刪除此檔案嗎？',
        isShowHeaderCancel: true,
        isShowFooterCancel: true,
        submitAction: () => {
            toggleDialog(false);
            emits('delete:file', fileId);
        },
    });
};

</script>

<template>
    <div
        v-if="isAdd"
        class="text-end mb-3"
    >
        <SFileupload
            is-show-note
            :limit-mb-size="limitMbSize"
            :btn-label="uploadBtnTitle"
            :extensions="extensions"
            dialog-btn-cancel-color="primary"
            @update:files="uploadFile"
        />
    </div>
    <SListComponent
        :items="list"
        :headers="headerList"
        class="pa-0 pa-sm-4"
    >
        <template #date="{ data }">
            <p v-dateformat:tw="data" />
        </template>
        <template #actions="{ data, tdData }">
            <v-table-btn @click="emits('download:file', data, tdData.fileName)">
                下載
            </v-table-btn>
            <v-table-btn
                v-if="isDelete"
                color="error"
                @click="deleteAlert(data)"
            >
                刪除
            </v-table-btn>
        </template>
    </SListComponent>
</template>

<style scoped>

</style>
