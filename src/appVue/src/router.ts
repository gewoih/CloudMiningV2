import {createRouter, createWebHistory} from 'vue-router';
import Registration from './components/Registration.vue';
import Authentication from './components/Authentication.vue';

const routes = [
    {path: '/user/register', name: 'Registration', component: Registration},
    {path: '/user/login', name: 'Authentication', component: Authentication},
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

export default router;