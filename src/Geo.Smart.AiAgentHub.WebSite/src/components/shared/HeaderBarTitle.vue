<script setup lang="ts">
import usePageStore from '@/stores/page';
import useUserStore from '@/stores/user';

const emits = defineEmits(['title:click']);
const userStore = useUserStore();
const pageStore = usePageStore();

</script>
<template>
    <div class="d-flex align-center">
        <div
            class="font-weight-bold text-h5 cursor-pointer"
            :class="{
                'ml-4': !pageStore.isSm && !pageStore.isMobile,
            }"
            @click="emits('title:click')"
        >
            {{
                pageStore.isSm || pageStore.isMobile
                    ? '管理後台'
                    : userStore.systemName
            }}
        </div>
    </div>
</template>
