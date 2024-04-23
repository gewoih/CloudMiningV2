import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router';
import Registration from './components/Registration.vue';


const routes: Array<RouteRecordRaw> = [
    {
      path: '/user/register',
      name: 'Registration',
      component: Registration
    },

  ];
  
  const router = createRouter({
    history: createWebHistory(),
    routes
  });
  
  export default router;