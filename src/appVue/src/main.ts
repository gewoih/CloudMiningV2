import {createApp} from 'vue'
import './style.css'
import App from './App.vue'
import router from './router';

import PrimeVue from 'primevue/config';
import 'primevue/resources/themes/aura-light-noir/theme.css'
import 'primevue/resources/primevue.min.css'
import 'primeicons/primeicons.css'
import 'primeflex/primeflex.css';
import ToastService from 'primevue/toastservice';
import Toast from "primevue/toast";

const app = createApp(App);
app.use(router);
app.use(PrimeVue)
app.use(ToastService);
app.component('Toast', Toast);

app.mount('#app');

