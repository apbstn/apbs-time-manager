<script setup>
import { useLayout } from '@/layout/composables/layout';

const { layoutState, toggleMenu, toggleConfigSidebar } = useLayout();

function onTopbarMenuToggle() {
    layoutState.topbarMenuActive = !layoutState.topbarMenuActive;
}
</script>

<template>
    <div class="layout-topbar">
        <div class="layout-topbar-start">
            <a ref="menuButton" class="layout-menu-button" @click="toggleMenu">
                <i class="pi pi-chevron-right"></i>
            </a>

            <button class="app-config-button app-config-mobile-button" @click="toggleConfigSidebar">
                <i class="pi pi-cog"></i>
            </button>

            <a ref="mobileMenuButton" class="layout-topbar-mobile-button" @click="onTopbarMenuToggle">
                <i class="pi pi-ellipsis-v"></i>
            </a>
        </div>

        <div class="layout-topbar-end">
            <div class="layout-topbar-actions-start"></div>
            <div class="layout-topbar-actions-end">
                <ul class="layout-topbar-items">
                    <li>
                        <button class="app-config-button" @click="toggleConfigSidebar">
                            <i class="pi pi-cog"></i>
                        </button>
                    </li>
                    <li>
                        <a v-styleclass="{ selector: '@next', enterFromClass: 'hidden', enterActiveClass: 'animate-scalein', leaveToClass: 'hidden', leaveActiveClass: 'animate-fadeout', hideOnOutsideClick: true }" class="">
                            <OverlayBadge severity="warn" :pt="{ pcBadge: { root: '!outline-0' } }">
                                <i class="pi pi-bell !align-middle"></i>
                            </OverlayBadge>
                        </a>
                    </li>

                    <li>
                        <a @click="layoutState.rightMenuActive = true" class="">
                            <i class="pi pi-arrow-left"></i>
                        </a>
                    </li>
                </ul>
            </div>
        </div>
    </div>
</template>
