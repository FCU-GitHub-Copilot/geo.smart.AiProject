<script setup>
import HeaderBarTitle from '@/components/shared/HeaderBarTitle.vue';
import usePageStore from '@/stores/page';

const pageStore = usePageStore();
</script>
<template>
    <v-app-bar>
        <v-app-bar-title>
            <HeaderBarTitle @title:click="pageStore.goPage('Login')" />
        </v-app-bar-title>
    </v-app-bar>
</template>
