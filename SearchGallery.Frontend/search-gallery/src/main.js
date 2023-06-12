import CoreuiVue from '@coreui/vue-pro';
import { CIcon } from '@coreui/icons-vue';

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'

const app = createApp(App)

app.use(router)
app.use(CoreuiVue);
app.component('CIcon', CIcon);

app.mount('#app')
