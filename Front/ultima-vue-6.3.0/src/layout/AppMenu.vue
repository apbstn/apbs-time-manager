<script setup>
import { ref } from 'vue';
import AppMenuItem from './AppMenuItem.vue';

const model = ref([
    {
        label: 'Dashboards',
        icon: 'pi pi-home',
        items: [
            {
                label: 'Home',
                icon: 'pi pi-fw pi-home',
                to: '/home'
            },
            {
                label: 'Tenant Management',
                icon: 'pi pi-fw pi-desktop',
                to: '/tenant-management'
            },
            {
                label: 'Account Creation',
                icon: 'pi pi-fw pi-user',
                to: '/user-management'
            }
            
            
        ]
    }
]);
</script>
<template>
    <ul class="layout-menu">
        <template v-for="(item, i) in model" :key="item">
            <AppMenuItem :item="item" root :index="i" />

            <li class="menu-separator"></li>
        </template>
    </ul>
</template>
