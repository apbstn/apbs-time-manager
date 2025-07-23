<script setup>
import { ref, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';

const home = { icon: 'pi pi-home', to: '/home' };
const breadcrumbRoutes = ref([]);

const route = useRoute();
const router = useRouter();

function navigate() {
    router.push(home.to);
}

function setBreadcrumbRoutes() {
    if (route.meta.breadcrumb) {
        breadcrumbRoutes.value = route.meta.breadcrumb;

        return;
    }

    breadcrumbRoutes.value = route.fullPath
        .split('/')
        .filter((item) => item !== '')
        .filter((item) => isNaN(Number(item)))
        .map((item) => item.charAt(0).toUpperCase() + item.slice(1));
}

watch(
    route,
    () => {
        setBreadcrumbRoutes();
    },
    { immediate: true }
);
</script>

<template>
    <div class="layout-breadcrumb-container">
        <nav class="layout-breadcrumb">
            <ol>
                <li>
                    <i :class="home.icon" @click="navigate"></i> 
                </li>
                <li><i class="pi pi-angle-right"></i></li>
                <template v-for="(item, index) in breadcrumbRoutes" :key="index">
                    <li>
                        <span> {{ item.label }}</span>
                    </li>
                    <li v-if="index !== breadcrumbRoutes.length - 1">
                        <i class="pi pi-angle-right"></i>
                    </li>
                </template>
            </ol>
        </nav>

        <div class="layout-breadcrumb-buttons">
            <!-- <Button icon="pi pi-bookmark" rounded text plain></Button> -->
        </div>
    </div>
</template>
