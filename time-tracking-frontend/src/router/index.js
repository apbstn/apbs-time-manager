import { createRouter, createWebHistory } from 'vue-router';
import AppLayout from '@/layout/AppLayout.vue';

import Login from '@/views/Login.vue';
import Home from '@/views/Home.vue';

import TimeTracking from '@/views/TimeTracking.vue';
import Planning from '@/views/Planning.vue';


import Employe from '@/views/Employe.vue';
import Teams from '@/views/teams.vue';

import ListDemandeEmploye from '@/views/List demandeEmploye.vue';
import DemandeDeCongé from '@/views/Demande de congé.vue';
import Congés from '@/views/congés.vue';
import EditCongé from '@/views/Edit Congé.vue';

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
                path: 'Time-Tracking', // Fix path
                name: 'TimeTracking',
                component: TimeTracking,
                meta: { requiresAuth: true }
            },
            {
                path: 'plan', // Fix path
                name: 'plan',
                component: Planning,
                meta: { requiresAuth: true }
            },
            {
                path: 'Teams',
                name: 'Teams',
                component: Teams,
                meta: { requiresAuth: true }
            },
            {
                path: 'employe', // Fix path
                name: 'employe',
                component: Employe,
                meta: { requiresAuth: true }
            },
            {
                path: 'listdemande',
                name: 'List De demande',
                component: ListDemandeEmploye,
                meta: { requiresAuth: true }
            },
            {
                path: 'demande',
                name: 'demande',
                component: DemandeDeCongé,
                meta: { requiresAuth: true}
            },
            {
                path: 'conges',
                name: 'conges',
                component: Congés,
                meta: { requiresAuth: true}
            },
            {
                path: 'edit',
                name: 'edit',
                component: EditCongé,
                meta: { requiresAuth: true}
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
