import type { App } from 'vue';
// Plugins
import FileListSelector from '@/components/common/FileListSelector.vue';
import { DatepickerSelector } from '@smart/vue-datepicker';
import { Dialog } from '@smart/vue-dialog';
import {
    CaptchaSelector,
    FormTitle,
    RadioSelector,
    Selector,
    TextField,
    Title,
} from '@smart/vue-form';
import { QuillEditor } from '@smart/vue-quill';
import {
    AdvSearchCard,
    KeywordSearch,
    ListComponent,
    TabComponent,
    TableLayout,
} from '@smart/vue-table';
import {
    FileuploadComponent,
    UploadImgListComponent,
} from '@smart/vue-uploadfile';
import axios from 'axios';
import VueAxios from 'vue-axios';

import vueFormat from './vueFormat';
import vuetify from './vuetify';

import '@smart/vue-chat/style.css';
import '@smart/vue-datepicker/style.css';
import '@smart/vue-dialog/style.css';
import '@smart/vue-form/style.css';
import '@smart/vue-loading/style.css';
import '@smart/vue-quill/style.css';
import '@smart/vue-snackbar/style.css';
import '@smart/vue-table/style.css';
import '@smart/vue-uploadfile/style.css';

const registerPlugins = (app: App) => {
    // 註冊全局插件
    app.use(VueAxios, axios);
    app.provide('axios', app.config.globalProperties.axios); // 提供 axios
    app.use(vuetify);
    app.use(vueFormat);

    // 註冊全局元件
    const components = [
        { name: 'SDialog', component: Dialog },
        { name: 'STitle', component: Title },
        { name: 'STextField', component: TextField },
        { name: 'SSelector', component: Selector },
        { name: 'SRadioSelector', component: RadioSelector },
        { name: 'SFormTitle', component: FormTitle },
        { name: 'SCaptchaSelector', component: CaptchaSelector },
        { name: 'SListComponent', component: ListComponent },
        { name: 'STabComponent', component: TabComponent },
        { name: 'STableLayout', component: TableLayout },
        { name: 'SKeywordSearch', component: KeywordSearch },
        { name: 'SAdvSearchCard', component: AdvSearchCard },
        { name: 'SDatepicker', component: DatepickerSelector },
        { name: 'SEditor', component: QuillEditor },
        { name: 'SFileupload', component: FileuploadComponent },
        { name: 'SUploadImgList', component: UploadImgListComponent },
        { name: 'SFileListSelector', component: FileListSelector },
    ];
    components.forEach((item) => {
        app.component(item.name, item.component);
    });
};

export default registerPlugins;
