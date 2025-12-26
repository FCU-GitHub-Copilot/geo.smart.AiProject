import stylistic from '@stylistic/eslint-plugin';
import importPlugin from 'eslint-plugin-import';
import pluginVue from 'eslint-plugin-vue';
import { defineConfig } from 'eslint/config';
import tseslint from 'typescript-eslint';
import * as vueParser from 'vue-eslint-parser';

export default defineConfig([
    ...tseslint.configs.recommended,
    ...pluginVue.configs['flat/recommended'],

    // TypeScript 檔案專用配置
    {
        files: ['**/*.ts'],
        languageOptions: {
            parser: tseslint.parser,
            parserOptions: {
                ecmaVersion: 'latest',
                sourceType: 'module',
                project: './tsconfig.json', // 啟用型別檢查
            },
        },
        plugins: {
            '@stylistic': stylistic,
            '@typescript-eslint': tseslint.plugin,
            import: importPlugin,
        },
        rules: {},
    },

    // Vue 檔案專用配置
    {
        files: ['**/*.vue'],
        languageOptions: {
            parser: vueParser,
            parserOptions: {
                ecmaVersion: 'latest',
                sourceType: 'module',
                parser: tseslint.parser, // Vue 檔案中的 TypeScript 解析
                extraFileExtensions: ['.vue'],
            },
        },
        plugins: {
            '@stylistic': stylistic,
            '@typescript-eslint': tseslint.plugin,
            import: importPlugin,
            vue: pluginVue,
        },
        rules: {
            // Vue 3 專用規則
            'vue/multi-word-component-names': 'error',
            'vue/component-definition-name-casing': ['error', 'PascalCase'],
            'vue/component-name-in-template-casing': ['error', 'PascalCase'],
            'vue/define-macros-order': ['error', {
                order: ['defineProps', 'defineEmits'],
            }],
            'vue/no-deprecated-v-on-native-modifier': 'error',
            'vue/no-deprecated-v-bind-sync': 'error',
            'vue/html-indent': ['error', 4, {
                baseIndent: 1,
            }],
        },
    },

    // 通用設定（JavaScript、Vue、TypeScript）
    {
        files: ['**/*.js', '**/*.mjs', '**/*.vue', '**/*.ts'],
        plugins: {
            '@stylistic': stylistic,
            import: importPlugin,
        },
        rules: {
            '@stylistic/indent': ['error', 4],
            '@stylistic/linebreak-style': 0,
            '@stylistic/quotes': ['error', 'single'],
            '@stylistic/semi': 2,
            '@stylistic/no-multiple-empty-lines': 'error',
            '@stylistic/no-multi-spaces': 'error',
            '@stylistic/no-trailing-spaces': 'error',
            'import/no-extraneous-dependencies': ['error', { devDependencies: true }],
            'no-console': process.env.NODE_ENV === 'production' ? 2 : 1,
            'no-debugger': process.env.NODE_ENV === 'production' ? 2 : 1,
            'import/extensions': [
                'error',
                'ignorePackages',
                {
                    js: 'never',
                    jsx: 'never',
                    ts: 'never',
                    tsx: 'never',
                },
            ],
            'no-shadow': 'off',
            '@typescript-eslint/no-shadow': 'warn',
            'no-unused-vars': 'off',
            '@typescript-eslint/no-unused-vars': 'error',
            'vue/no-multiple-template-root': 0,
        },
        settings: {
            'import/resolver': {
                alias: {
                    map: [['@', './src']],
                    extensions: ['.ts', '.js', '.vue', '.mjs'],
                },
            },
        },
    },
]);