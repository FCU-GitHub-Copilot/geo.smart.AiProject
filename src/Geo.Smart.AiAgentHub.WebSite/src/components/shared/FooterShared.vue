<script setup lang="ts">
import useUserStore from '@/stores/user';

const { systemName } = useUserStore();
const year = new Date().getFullYear();
</script>
<template>
    <v-footer
        app
        class="bg-black position-absolute d-print-none"
    >
        <v-container
            ref="container"
            class="py-2"
        >
            <v-row>
                <v-col cols="12">
                    &copy;{{ year }} {{ systemName }}
                </v-col>
            </v-row>
        </v-container>
    </v-footer>
</template>
