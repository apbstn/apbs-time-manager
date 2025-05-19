import { createRouter, createWebHistory } from 'vue-router';
import AppLayout from '@/layout/AppLayout.vue';
import UserManagement from '@/views/UserManagement.vue';
import Login from '@/views/Login.vue';
import Home from '@/views/Home.vue';
import Phonetest from '@/views/Phonetest.vue';

const routes = [
    {
        path: '/',
        component: AppLayout, // Wrap main routes inside layout
        children: [
            {
                path: 'home',
                name: 'home',
                component: Home,
                meta: { requiresAuth: true }
            },
            {
                path: 'tenant-management', // Fix path
                name: 'tenant Management',
                component: UserManagement,
                meta: { requiresAuth: true }
            },
            {
                path: 'phone', // Fix path
                name: 'phone',
                component: Phonetest,
                meta: { requiresAuth: true }
            }
        ]
    },
    {
        path: '/login',
        name: 'Login',
        component: Login
    } // Keep login separate
];

const router = createRouter({
    history: createWebHistory(),
    routes,
    scrollBehavior() {
        return { left: 0, top: 0 };
    }
});

router.beforeEach((to, from, next) => {
    const isLoggedIn = !!localStorage.getItem('accessToken');
  
    if (to.meta.requiresAuth && !isLoggedIn) {
      next('/login'); // ✅ Redirect to login
    } else {
      next(); // ✅ Allow navigation
    }
  });

export default router;
