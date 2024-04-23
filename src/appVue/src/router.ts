import {createRouter, createWebHistory} from 'vue-router';
import Registration from './components/Registration.vue';
import Authentication from './components/Authentication.vue';

const routes = [
    {path: '/user/register', component: Registration},
    {path: '/user/login', component: Authentication},
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

export default router;