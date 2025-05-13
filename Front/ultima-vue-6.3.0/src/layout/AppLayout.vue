<script setup>
import { useLayout } from '@/layout/composables/layout';
import { computed, onBeforeUnmount, ref, watch } from 'vue';
import AppBreadCrumb from './AppBreadcrumb.vue';
import AppConfig from './AppConfig.vue';
import AppFooter from './AppFooter.vue';
import AppRightMenu from './AppRightMenu.vue';
import AppSidebar from './AppSidebar.vue';
import AppTopbar from './AppTopbar.vue';

const { layoutConfig, layoutState, isSidebarActive } = useLayout();

const outsideClickListener = ref(null);

function unbindOutsideClickListener() {
    if (outsideClickListener.value) {
        document.removeEventListener('click', outsideClickListener.value);
        outsideClickListener.value = null;
    }
}

function bindOutsideClickListener() {
    if (!outsideClickListener.value) {
        outsideClickListener.value = (event) => {
            if (isOutsideClicked(event)) {
                layoutState.overlayMenuActive = false;
                layoutState.overlaySubmenuActive = false;
                layoutState.staticMenuMobileActive = false;
                layoutState.menuHoverActive = false;
                layoutState.configSidebarVisible = false;
            }
        };
        document.addEventListener('click', outsideClickListener.value);
    }
}

function isOutsideClicked(event) {
    const sidebarEl = document.querySelector('.layout-sidebar');
    const topbarButtonEl = document.querySelector('.layout-menu-button');

    return !(sidebarEl?.isSameNode(event.target) || sidebarEl?.contains(event.target) || topbarButtonEl?.isSameNode(event.target) || topbarButtonEl?.contains(event.target));
}

watch(isSidebarActive, (newVal) => {
    if (newVal) {
        bindOutsideClickListener();
    } else {
        unbindOutsideClickListener();
    }
});

onBeforeUnmount(() => {
    unbindOutsideClickListener();
});

const containerClass = computed(() => {
    let styleClass = {
        'layout-overlay': layoutConfig.menuMode === 'overlay',
        'layout-static': layoutConfig.menuMode === 'static',
        'layout-slim': layoutConfig.menuMode === 'slim',
        'layout-slim-plus': layoutConfig.menuMode === 'slim-plus',
        'layout-horizontal': layoutConfig.menuMode === 'horizontal',
        'layout-reveal': layoutConfig.menuMode === 'reveal',
        'layout-drawer': layoutConfig.menuMode === 'drawer',
        'layout-sidebar-dark': layoutConfig.colorScheme === 'dark',
        'layout-static-inactive': layoutState.staticMenuDesktopInactive && layoutConfig.menuMode === 'static',
        'layout-overlay-active': layoutState.overlayMenuActive,
        'layout-mobile-active': layoutState.staticMenuMobileActive,
        'layout-topbar-menu-active': layoutState.topbarMenuActive,
        'layout-menu-profile-active': layoutState.rightMenuActive,
        'layout-sidebar-active': layoutState.sidebarActive,
        'layout-sidebar-anchored': layoutState.anchored
    };

    styleClass['layout-topbar-' + layoutConfig.topbarTheme] = true;
    styleClass['layout-menu-' + layoutConfig.menuTheme] = true;
    styleClass['layout-menu-profile-' + layoutConfig.menuProfilePosition] = true;

    return styleClass;
});
</script>

<template>
    <div :class="['layout-container', { ...containerClass }]">
        <AppTopbar />
        <AppRightMenu />
        <AppSidebar />

        <div class="layout-content-wrapper">
            <AppBreadCrumb></AppBreadCrumb>
            <div class="layout-content">
                <router-view></router-view>
            </div>

            <AppFooter></AppFooter>
        </div>

        <AppConfig />
    </div>
</template>
