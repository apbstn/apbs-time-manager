<script setup>
import { ref, computed } from 'vue';
import AppMenuItem from './AppMenuItem.vue';

// Get role from localStorage (make sure to set this during login)
const userRole = ref(localStorage.getItem('role')); // 'owner' or 'employee'

const fullMenu = ref([
    {
        label: 'Dashboards',
        icon: 'pi pi-home',
        items: [
            {
                label: 'Home',
                icon: 'pi pi-fw pi-home',
                to: '/home',
                showFor: ['Owner', 'employee'] // Added home for employees
            },
            {
                label: 'Time Tracking',
                icon: 'pi pi-fw pi-stopwatch',
                to: '/time-tracking',
                showFor: ['Owner', 'employee']
            },
            {
                label: 'Plan',
                icon: 'pi pi-fw pi-calendar',
                to: '/plan',
                showFor: ['Owner']
            },
            {
                label: 'Users',
                icon: 'pi pi-fw pi-users',
                to: '/employe',
                showFor: ['Owner']
            },
            {
                label: 'Teams',
                icon: 'pi pi-fw pi-sitemap',
                to: '/Teams',
                showFor: ['Owner']
            },
            {
                label: 'List Demande',
                icon: 'pi pi-fw pi-list',
                to: '/listdemande',
                showFor: ['Owner', 'employee']
            },
            {
                label: 'Conges',
                icon: 'pi pi-fw pi-calendar-times',
                to: '/conges',
                showFor: ['Owner']
            },
            {
                label: 'Edit',
                icon: 'pi pi-fw pi-pencil',
                to: '/edit',
                showFor: ['employee']
            },
            {
                label: 'Logout',
                icon: 'pi pi-fw pi-sign-out',
                command: () => handleLogout(),
                showFor: ['Owner', 'employee']
            }
        ]
    }
]);

// Filter menu based on role
const model = computed(() => {
    if (!userRole.value) return []; // Show nothing if no role
    
    return fullMenu.value.map(section => ({
        ...section,
        items: section.items.filter(item => 
            item.showFor.includes(userRole.value)
        )
    }));
});

const handleLogout = async () => {
    try {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('userRole');
        delete axios.defaults.headers.common['Authorization'];
        window.location.href = '/login';
    } catch (error) {
        console.error('Logout failed:', error);
        window.location.href = '/login';
    }
};
</script>

<template>
    <ul class="layout-menu">
        <template v-for="(item, i) in model" :key="item">
            <AppMenuItem :item="item" root :index="i" />
            <li class="menu-separator"></li>
        </template>
    </ul>
</template>