<script setup lang="ts">
import { computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useDisplay } from 'vuetify';

// 路由資訊
const route = useRoute();
const router = useRouter();

const { mobile } = useDisplay();

interface LinkResult {
    title: string | undefined;
    href: string | null;
}

const MAX_TEXT_LENGTH = 14;

/**
 * 檢查文字長度如果超過14字則截斷，並補上...
 * @param text 文字
 */
const truncateText = (text: string): string => {
    if (text.length > MAX_TEXT_LENGTH) {
        return `${text.substring(0, MAX_TEXT_LENGTH)}...`;
    }
    return text;
};

// 尋找route依照path
const findLink = (path: string): LinkResult | null => {
    const { routes } = router.options;
    const currentRoute = routes.find((routeItem) => routeItem.path === path);
    if (path.includes('?')) return null;
    if (currentRoute) {
        return {
            title: currentRoute.meta?.isReplaceByQuery
                ? truncateText(route.query.name as string)
                : truncateText(currentRoute.meta?.title as string),
            href:
                currentRoute.components
                && currentRoute.path !== route.path
                && !currentRoute.path.includes(':')
                    ? currentRoute.path
                    : null,
        };
    }
    return null;
};

// 依載入頁面不同建立連結列表
const getBreadcrumb = () => {
    const { path, params, matched } = route;
    let isParams = false;
    Object.values(params).forEach((param) => {
        if (param) {
            isParams = true;
            return;
        }
    });

    let breadcrumbData = (isParams ? matched[0].path : path).split('/');

    // 建立路徑陣列
    breadcrumbData = breadcrumbData.map((_, index, array) => {
        if (index === 0) return '/';
        let link = '';
        for (let i = 0; i <= index; i += 1) {
            link += `${array[i]}`;
            if (i !== index) link += '/';
        }
        return link;
    });

    return breadcrumbData
        .map((item) => (item === '/' ? '/home' : item))
        .filter((value, index, self) => self.indexOf(value) === index)
        .map((item) => findLink(item))
        .filter((item): item is LinkResult => !!item && !!item.title)
        .map((item) => ({
            title: item.title || '',
            to: item.href || undefined,
            disabled: !item.href,
        }));
};

const breadcrumb = computed(() => getBreadcrumb());
</script>

<template>
    <v-col
        cols="12"
        lg="6"
        class="d-flex"
        :class="mobile ? 'justify-start' : 'justify-end'"
    >
        <v-breadcrumbs
            :items="breadcrumb"
            class="pointer-normal"
        >
            <template #title="{ item }">
                <p>
                    {{ item.title }}
                </p>
            </template>
        </v-breadcrumbs>
    </v-col>
</template>

<style lang="sass" scoped>
.v-breadcrumbs
    flex-wrap: wrap
    padding-block: 0
</style>
