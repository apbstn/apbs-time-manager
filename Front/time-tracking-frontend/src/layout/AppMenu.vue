<script setup>
import { ref, computed } from 'vue';
import AppMenuItem from './AppMenuItem.vue';

// Get role from localStorage (make sure to set this during login)
const userRole = ref(localStorage.getItem('role')); // e.g., 'Owner', 'employee', 'User', or null

const fullMenu = ref([
    {
        label: 'Dashboards',
        icon: 'pi pi-home',
        items: [
            {
                label: 'Home',
                icon: 'pi pi-fw pi-home',
                to: '/home',
                showFor: ['Owner', 'employee']
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
                label: 'Invitation',
                icon: 'pi pi-fw pi-pencil',
                to: '/invite',
                showFor: ['Owner', 'employee', 'all'] // 'all' to indicate visibility regardless of role
            }
        ]
    }
]);

// Filter menu based on role
const model = computed(() => {
    return fullMenu.value.map(section => ({
        ...section,
        items: section.items.filter(item => {
            // Always show items with 'all' in showFor
            if (item.showFor.includes('all')) return true;
            // If no role, only show items with 'all'
            if (!userRole.value) return false;
            // Otherwise, check if role matches (case-insensitive)
            return item.showFor.some(role => 
                role.toLowerCase() === userRole.value.toLowerCase()
            );
        })
    }));
});
</script>

<template>
    <ul class="layout-menu">
        <template v-for="(item, i) in model" :key="item">
            <AppMenuItem :item="item" root :index="i" />
            <li class="menu-separator"></li>
        </template>
    </ul>
</template>