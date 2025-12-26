import { createApp } from 'vue';

import registerPlugins from '@/plugins';
import pinia from '@/store';
import router from './router';

import '@/assets/sass/main.sass';

import App from './App.vue';

const app = createApp(App);

registerPlugins(app);

app.use(pinia);
app.use(router);

app.mount('#app');
