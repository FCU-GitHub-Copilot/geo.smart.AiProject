import VueFormat from '@smart/vue-format';
import type { App } from 'vue';

const config = {
    customDateFormat: {
        year: 'YYYY',
        tw2: 'YYY/MM/DD',
        tw2M: 'YYY/MM',
        tw2Full: 'YYY/MM/DD HH:mm:ss',
        tw2HM: 'YYY/MM/DD HH:mm',
        tw3M: 'YYY年MM月',
        fileDate: 'YYYY/MM/DD/HH/mm/ss',
    },
};

export default {
    install(app: App) {
        // 註冊 VueFormat 並傳遞自訂的日期格式設定
        app.use(VueFormat, config);
    },
};
