/**
 * plugins/vuetify.js
 *
 * Framework documentation: https://vuetifyjs.com`
 */

// Styles
import 'vuetify/styles';

// Composables
import { themes } from '@/themes';
import { createVuetify } from 'vuetify';
import { md3 } from 'vuetify/blueprints';
import { VBtn, VCol } from 'vuetify/components';
import { aliases, mdi } from 'vuetify/iconsets/mdi-svg';

// https://vuetifyjs.com/en/introduction/why-vuetify/#feature-guides
export default createVuetify({
    blueprint: md3,
    icons: {
        defaultSet: 'mdi',
        aliases,
        sets: {
            mdi,
        },
    },
    theme: {
        cspNonce:
            typeof __VUETIFY_NONCE__ !== 'undefined' ? __VUETIFY_NONCE__ : '',
        themes, // 預設值於 page.js
    },
    display: {
        mobileBreakpoint: 'xs',
        thresholds: {
            xs: 600,
        },
    },
    aliases: {
        VTableBtn: VBtn,
        VBackBtn: VBtn,
        VDialogBtn: VBtn,
        VTableCol: VCol,
    },
    defaults: {
        VAppBar: {
            class: 'd-print-none bg-secondary',
            height: '64',
        },
        VNavigationDrawer: {
            color: 'primary',
            class: 'd-print-none',
            width: 285,
        },
        VContainer: {
            fluid: true,
        },
        VCol: {
            alignSelf: 'center',
        },
        VSheet: {
            elevation: '2',
            rounded: true,
        },
        VBtn: {
            rounded: '4',
        },
        VTableBtn: {
            size: 'small',
            class: 'mb-1 mr-1',
            color: 'primary',
        },
        VBackBtn: {
            variant: 'outlined',
            color: 'primary',
        },
        VDialogBtn: {
            variant: 'flat',
            color: 'primary',
        },
        VTableCol: {
            alignSelf: 'center',
            class: 'py-2',
        },
        VSelect: {
            hideDetails: 'auto',
            density: 'compact',
            variant: 'outlined',
            bgColor: 'white',
            noDataText: '查無資料',
            menuProps: {
                attach: false,
                zIndex: 2400,
            },
        },
        VMenu: {
            zIndex: 2400,
        },
        VAutocomplete: {
            hideDetails: 'auto',
            density: 'compact',
            variant: 'outlined',
            bgColor: 'white',
            placeholder: '請選擇',
            noDataText: '查無資料',
            menuProps: {
                attach: false,
                zIndex: 2400,
            },
        },
        VTextField: {
            hideDetails: 'auto',
            density: 'compact',
            variant: 'outlined',
            bgColor: 'white',
        },
        VCheckbox: {
            hideDetails: 'auto',
            color: 'primary',
            density: 'compact',
        },
        VRadioGroup: {
            hideDetails: 'auto',
            color: 'primary',
            density: 'compact',
        },
        VTextarea: {
            density: 'compact',
            bgColor: 'white',
            variant: 'outlined',
            hideDetails: 'auto',
        },
        VCombobox: {
            hideDetails: 'auto',
            density: 'compact',
            variant: 'outlined',
            bgColor: 'white',
        },
        VAlert: {
            rounded: 'lg',
            density: 'compact',
        },
    },
});
