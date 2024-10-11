import {createRouter, createWebHistory} from 'vue-router';
import Registration from './components/Registration.vue';
import Authentication from './components/Authentication.vue';
import Payments from './components/Payments.vue';
import NotFound from "@/components/NotFound.vue";
import NotificationsSettings from "@/components/NotificationsSettings.vue";
import Members from "@/components/Members.vue";
import Home from "@/components/Home.vue";
import {useUserStore} from "@/stores/user.ts";
import Forbidden from "@/components/Forbidden.vue";

const routes = [
    { path: '/', name: 'home', component: Home },
    {path: '/user/register', name: 'register', component: Registration},
    {path: '/user/login', name: 'login', component: Authentication},
    {path: '/payments', name: 'payments', component: Payments},
    {path: '/members', name: 'members', component: Members},
    {path: '/profile/settings/notifications', name: 'notificationsSettings', component: NotificationsSettings},
    {path: "/:pathMatch(.*)", name: "NotFound", component: NotFound},
    {path: "/forbidden", name: "Forbidden", component: Forbidden}
];


const router = createRouter({
    history: createWebHistory(),
    routes
});

router.beforeEach((to) => {
    const userStore = useUserStore();
    if (!userStore.isAuthenticated && to.name !== 'login')
        return '/user/login';

    if (to.name === 'members' && !userStore.isAdmin)
        return '/forbidden';
});



export default router;