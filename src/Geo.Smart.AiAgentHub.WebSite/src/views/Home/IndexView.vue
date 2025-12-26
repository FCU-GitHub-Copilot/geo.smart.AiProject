<script setup lang="ts">
import PageLayout from '@/components/shared/PageLayout.vue';
</script>

<template>
    <PageLayout>
        <section class="mb-12">
            <h2 class="mb-3">
                .NET8 開發範本說明
            </h2>
            <ul class="ms-10">
                <li>由此版本開始採用前後端分離的模式</li>
                <li>
                    後端為 ASP.NET Core 8、Ef Core 8 開發，僅提供 Web
                    API，必須要有 Swagger 文件及註解說明
                </li>
                <li>
                    原始碼版控位置為
                    <a
                        href="https://git.geo.local/Modules/geo.smart.coretemplate7"
                        target="_blank"
                    >
                        Geo.Smart.CoreTemplate8
                    </a>
                </li>
                <li>
                    要快速套用此專案範本，請參照<a
                        href="https://gtech.geo.local/Blog/Seq/10974"
                        target="_blank"
                    >建立多重專案本 文件說明
                    </a>
                </li>
                <li>
                    前端使用 Vue3 與 Vuetify 3 介面框架搭配使用，以 Vite
                    作為前端建置工具，為 SPA(單一頁面應用程式) 專案
                </li>
            </ul>
        </section>
        <section class="my-12">
            <h2 class="mb-3">
                主題設置
            </h2>
            <ul class="ms-10">
                <li>
                    請參照
                    <a
                        href="https://vuetifyjs.com/zh-Hans/styles/colors/#material-82725f698868"
                        target="_blank"
                    >Material 色彩表</a>
                    挑選配色
                </li>
                <li>
                    按鈕的配色僅能使用由<strong> 預設主題 </strong>中搭配好的色彩，請勿自行搭配
                </li>
                <li>表格中的按鈕尺寸預設為<strong> SMALL </strong></li>
                <li>
                    "取消"、"返回"、"否" 等負面表述按鈕，預設使用 PRIMARY 的
                    <strong>outlined</strong> 樣式
                </li>
            </ul>
            <v-container>
                <h3>預設主題</h3>
                <v-btn
                    color="primary"
                    class="ma-2"
                >
                    primary
                </v-btn>
                <v-btn
                    color="secondary"
                    class="ma-2"
                >
                    secondary
                </v-btn>
                <v-btn
                    color="tertiary"
                    class="ma-2"
                >
                    tertiary
                </v-btn>
                <v-btn
                    color="success"
                    class="ma-2"
                >
                    success
                </v-btn>
                <v-btn
                    color="warning"
                    class="ma-2"
                >
                    warning
                </v-btn>
                <v-btn
                    color="error"
                    class="ma-2"
                >
                    error
                </v-btn>
                <v-btn
                    color="info"
                    class="ma-2"
                >
                    info
                </v-btn>
                <h3>變異：外框線(outlined)</h3>
                <v-btn
                    color="primary"
                    variant="outlined"
                    class="ma-2"
                >
                    primary
                </v-btn>
                <v-btn
                    color="secondary"
                    variant="outlined"
                    class="ma-2"
                >
                    secondary
                </v-btn>
                <v-btn
                    color="tertiary"
                    variant="outlined"
                    class="ma-2"
                >
                    tertiary
                </v-btn>
                <v-btn
                    color="success"
                    variant="outlined"
                    class="ma-2"
                >
                    success
                </v-btn>
                <v-btn
                    color="warning"
                    variant="outlined"
                    class="ma-2"
                >
                    warning
                </v-btn>
                <v-btn
                    color="error"
                    variant="outlined"
                    class="ma-2"
                >
                    error
                </v-btn>
                <v-btn
                    color="info"
                    variant="outlined"
                    class="ma-2"
                >
                    info
                </v-btn>
                <h3>尺寸</h3>
                <v-btn
                    color="primary"
                    size="x-small"
                    class="ma-2"
                >
                    x-small
                </v-btn>
                <v-btn
                    color="primary"
                    size="small"
                    class="ma-2"
                >
                    small
                </v-btn>
                <v-btn
                    color="primary"
                    class="ma-2"
                >
                    預設
                </v-btn>
                <v-btn
                    color="primary"
                    size="large"
                    class="ma-2"
                >
                    large
                </v-btn>
                <v-btn
                    color="primary"
                    size="x-large"
                    class="ma-2"
                >
                    x-large
                </v-btn>
                <h3>圓角</h3>
                <v-btn
                    color="primary"
                    rounded="0"
                    class="ma-2"
                >
                    無圓角
                </v-btn>
                <v-btn
                    color="primary"
                    rounded="xs"
                    class="ma-2"
                >
                    xs
                </v-btn>
                <v-btn
                    color="primary"
                    rounded="sm"
                    class="ma-2"
                >
                    sm
                </v-btn>
                <v-btn
                    color="primary"
                    rounded="md"
                    class="ma-2"
                >
                    md
                </v-btn>
                <v-btn
                    color="primary"
                    rounded="lg"
                    class="ma-2"
                >
                    lg
                </v-btn>
                <v-btn
                    color="primary"
                    rounded="xl"
                    class="ma-2"
                >
                    xl
                </v-btn>
            </v-container>
        </section>
        <section class="my-12">
            <h2 class="mb-3">
                為什麼要使用 Vuetify
            </h2>
            <ul class="ms-10">
                <li>建立在 Vue.js 之上的 UI 框架</li>
                <li>提供豐富且吸引使用者體驗所需的工具</li>
                <li>
                    符合
                    <a
                        href="https://m3.material.io/"
                        target="_blank"
                    >Material Design</a>
                    的設計組件 模組化、響應式(RWD)、優秀的效能
                </li>
                <li>採用 Mobile First 的設計</li>
                <li>長期支援 Long-term Support、每周發布更新</li>
            </ul>
        </section>

        <v-divider />

        <section class="my-12">
            <h2 class="mb-3">
                範本預計包含項目
            </h2>
            <ul class="ms-10">
                <li class="test">
                    主題配色 Theme
                </li>
                <li>Layout: Header、Body、Footer</li>
                <li>登入管理</li>
                <li>動態選單</li>
                <li>狀態頁: 403、404、500</li>
                <li>元件</li>
                <li>新版圖台功能</li>
            </ul>
        </section>
    </PageLayout>
</template>
