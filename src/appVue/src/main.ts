import {createApp} from 'vue'
import './style.css'
import App from './App.vue'
import router from './router';

import PrimeVue from 'primevue/config';
import 'primevue/resources/themes/aura-light-noir/theme.css'
import 'primevue/resources/primevue.min.css'
import 'primeicons/primeicons.css'
import 'primeflex/primeflex.css';
import ConfirmationService from 'primevue/confirmationservice';
import ToastService from 'primevue/toastservice';
import Toast from "primevue/toast";
import Tooltip from "primevue/tooltip";
import { createPinia } from 'pinia';

const app = createApp(App);
const pinia = createPinia();

app.use(pinia);
app.use(router);
app.use(PrimeVue)
app.use(ToastService);
app.use(ConfirmationService);
app.component('Toast', Toast);
app.directive('tooltip', Tooltip);

app.mount('#app');

