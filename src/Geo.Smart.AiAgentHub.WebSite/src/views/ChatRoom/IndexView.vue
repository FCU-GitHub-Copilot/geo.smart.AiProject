<script setup lang="ts">
import { ref } from 'vue';

import ChatComponent from '@/components/chat/ChatComponent.vue';
import type { FloatingMode } from '@smart/vue-chat';

// 浮動模式
const floatingMode = ref<FloatingMode>('normal');
</script>
<template>
    <v-card
        elevation="2"
        height="calc(100vh - 120px)"
        class="z-index-top"
    >
        <ChatComponent
            v-model:float="floatingMode"
        />
    </v-card>
</template>

<style scoped lang="sass">
.z-index-top
    z-index: 2399
</style>