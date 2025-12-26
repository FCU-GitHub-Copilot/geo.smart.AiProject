<script setup lang="ts">
import { computed, useSlots, type Slots } from 'vue';
import { useRoute } from 'vue-router';

import FooterShared from '@/components/shared/FooterShared.vue';
import PageHeaderShared from '@/components/shared/PageHeaderShared.vue';

defineProps({
    pageTitle: {
        type: String,
        default: '',
    },
});

const route = useRoute();
const slots = useSlots() as Slots;

const hasContentSlots = computed(() => slots.default || slots.btn);
</script>
<template>
    <v-container>
        <v-row class="d-print-none">
            <v-col>
                <h5 class="text-h5 font-weight-bold">
                    {{ pageTitle || route.meta.title }}
                </h5>
            </v-col>
            <PageHeaderShared />
        </v-row>
        <template v-if="hasContentSlots">
            <v-sheet
                class="my-2 py-4"
                min-height="75vh"
            >
                <v-container>
                    <v-row v-if="slots.btn">
                        <v-col
                            cols="12"
                            class="text-end"
                        >
                            <slot name="btn" />
                        </v-col>
                    </v-row>
                    <slot />
                </v-container>
            </v-sheet>
        </template>
    </v-container>
    <FooterShared />
</template>
