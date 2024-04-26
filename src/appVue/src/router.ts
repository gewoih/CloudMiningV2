import {createRouter, createWebHistory} from 'vue-router';
import Registration from './components/Registration.vue';
import Authentication from './components/Authentication.vue';
import Payments from './components/Payments.vue';

const routes = [
    {path: '/user/register', name: 'register', component: Registration},
    {path: '/user/login', name: 'login', component: Authentication},
    {path: '/payments', name: 'payments', component: Payments}
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

export default router;