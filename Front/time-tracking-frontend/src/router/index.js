import { createRouter, createWebHistory } from 'vue-router';
import AppLayout from '@/layout/AppLayout.vue';
import Login from '@/views/Login.vue';
import Home from '@/views/Home.vue';
import TimeTracking from '@/views/TimeTracking.vue';
import Planning from '@/views/Planning.vue';
import Employe from '@/views/Employe.vue';
import Teams from '@/views/teams.vue';
import ListDemandeEmploye from '@/views/List demandeEmploye.vue';
import Conges from '@/views/conges.vue';
import Switch from '@/views/Switch.vue';
import Logout from '@/views/logout.vue';
import Settings from '@/views/settings.vue';
import Invitation from '@/views/invitation.vue';
import Invite from '@/views/Invite.vue';
import Invitedoesntexist from '@/views/invitedoesntexist.vue';
import Register from '@/views/register.vue';

const routes = [
    {
        path: '/',
        component: AppLayout,
        meta: { requiresAuth: true },
        children: [
            {
                path: 'home',
                name: 'home',
                component: Home,
                meta: { requiresAuth: true, breadcrumb: [{ label: 'Home' }] }
            },
            {
                path: 'Time-Tracking',
                name: 'TimeTracking',
                component: TimeTracking,
                meta: { requiresAuth: true, breadcrumb: [{ label: 'Time Tracking' }] }
            },
            {
                path: 'plan',
                name: 'plan',
                component: Planning,
                meta: { requiresAuth: true, breadcrumb: [{ label: 'Plan Of Work' }] }
            },
            {
                path: 'Teams',
                name: 'Teams',
                component: Teams,
                meta: { requiresAuth: true, breadcrumb: [{ label: 'Teams' }] }
            },
            {
                path: 'employe',
                name: 'employe',
                component: Employe,
                meta: { requiresAuth: true, breadcrumb: [{ label: 'Users' }] }
            },
            {
                path: 'listdemande',
                name: 'ListDeDemande',
                component: ListDemandeEmploye,
                meta: { requiresAuth: true, breadcrumb: [{ label: 'Leave Request' }] }
            },
            {
                path: 'conges',
                name: 'conges',
                component: Conges,
                meta: { requiresAuth: true, breadcrumb: [{ label: 'List Of Leave Requests' }] }
            },
            {
                path: 'Switch',
                name: 'Switch',
                component: Switch,
                meta: { requiresAuth: true, breadcrumb: [{ label: 'Switch' }] }
            },
            {
                path: 'logout',
                name: 'logout',
                component: Logout,
                meta: { requiresAuth: true }
            },
            {
                path: 'settings',
                name: 'settings',
                component: Settings,
                meta: { requiresAuth: true, breadcrumb: [{ label: 'Account Settings' }] }
            },
            {
                path: 'invitation',
                name: 'invitation',
                component: Invitation,
                meta: { requiresAuth: true, breadcrumb: [{ label: 'Invite' }] }
            },
            {
                path: 'invite/exists',
                name: 'exists',
                component: Invite,
                meta: { requiresAuth: true }
            }
        ]
    },
    {
        path: '/login',
        name: 'Login',
        component: Login
    },
    {
        path: '/register',
        name: 'Register',
        component: Register
    },
    {
        path: '/invite/dontExists',
        name: 'dontExists',
        component: Invitedoesntexist
    }

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
        next('/login');
    } else {
        next();
    }
});

export default router;