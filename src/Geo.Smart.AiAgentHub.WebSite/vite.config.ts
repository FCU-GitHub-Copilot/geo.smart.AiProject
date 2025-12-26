// Plugins
import eslint from '@nabla/vite-plugin-eslint';
import { viteNoncePlugin } from '@smart/vite-nonce-plugin';
import vue from '@vitejs/plugin-vue';
import { createHtmlPlugin } from 'vite-plugin-html';
import mkcert from 'vite-plugin-mkcert';
import vuetify from 'vite-plugin-vuetify';

// Utilities
import { fileURLToPath, URL } from 'node:url';
import { defineConfig, loadEnv } from 'vite';

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
    const env = loadEnv(mode, process.cwd(), '');

    return {
        base: env.VITE_APP_PATH,
        plugins: [
            vue(),
            viteNoncePlugin(),
            createHtmlPlugin({
                minify: true,
                inject: {
                    data: {
                        title: env.VITE_APP_TITLE,
                    },
                },
            }),
            // https://github.com/vuetifyjs/vuetify-loader/tree/master/packages/vite-plugin
            vuetify(),
            eslint(),
            env.NODE_ENV === 'production' ? null : mkcert(),
        ],
        define: {
            'process.env': {},
        },
        resolve: {
            alias: {
                '@': fileURLToPath(new URL('./src', import.meta.url)),
            },
            extensions: ['.js', '.json', '.jsx', '.mjs', '.ts', '.tsx', '.vue'],
        },
        css: {
            preprocessorOptions: {
                sass: {
                    api: 'modern-compiler', // sass將移除legacy-compiler
                },
            },
        },
        server: {
            port: 5001,
            proxy: {
                '/Api': {
                    target: env.VITE_APP_API,
                    changeOrigin: true,
                    secure: false,
                    rewrite: (path) => path.replace(/^\/Api/, ''),
                },
            },
            open: true,
        },
    };
});
