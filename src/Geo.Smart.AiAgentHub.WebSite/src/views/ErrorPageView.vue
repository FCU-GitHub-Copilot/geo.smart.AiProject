<script setup lang="ts">
import useUserStore from '@/stores/user';
import { useRoute, useRouter } from 'vue-router';

// 路由
const route = useRoute();
const router = useRouter();

const { homePath, systemName } = useUserStore();

const goHome = () => {
    router.push({ path: homePath });
};
</script>

<template>
    <v-responsive class="align-center text-center fill-height bg-secondary">
        <v-container>
            <v-row>
                <v-col>
                    <h1 class="text-h3 font-weight-bold">
                        {{ systemName }}
                    </h1>
                </v-col>
            </v-row>
            <v-row>
                <v-col>
                    <h2 class="text-h3 font-weight-bold mb-5">
                        {{ route.meta.title }}
                    </h2>
                    <p>{{ route.meta.msg }}</p>
                </v-col>
            </v-row>
            <v-row>
                <v-col>
                    <v-hover>
                        <template
                            #default="{ isHovering, props: hProps }"
                        >
                            <v-btn
                                v-bind="hProps"
                                :color="
                                    isHovering
                                        ? 'amber-darken-4'
                                        : 'amber-darken-1'
                                "
                                @click="goHome"
                            >
                                回首頁
                            </v-btn>
                        </template>
                    </v-hover>
                </v-col>
            </v-row>
        </v-container>
    </v-responsive>
</template>
