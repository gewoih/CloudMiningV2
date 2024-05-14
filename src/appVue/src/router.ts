import {createRouter, createWebHistory} from 'vue-router';
import Registration from './components/Registration.vue';
import Authentication from './components/Authentication.vue';
import Payments from './components/Payments.vue';
import NotFound from "@/components/NotFound.vue";

const routes = [
    {path: '/user/register', name: 'register', component: Registration},
    {path: '/user/login', name: 'login', component: Authentication},
    {path: '/payments', name: 'payments', component: Payments},
    {path: "/:pathMatch(.*)", name: "NotFound", component: NotFound}
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

export default router;