<script setup>
import { useLayout } from '@/layout/composables/layout';
import { computed } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const { layoutState, layoutConfig, isHorizontal, isSlim } = useLayout();

const menuClass = computed(() => (isHorizontal.value ? 'overlay' : null));

function toggleMenu() {
    const menu = document.querySelector('.menu-transition');

    if (layoutState.menuProfileActive) {
        menu.style.maxHeight = '0';
        menu.style.opacity = '0';
        if (isHorizontal.value) {
            menu.style.transform = 'scaleY(0.8)';
        }
    } else {
        menu.style.maxHeight = menu.scrollHeight + 'px';
        menu.style.opacity = '1';
        if (isHorizontal.value) {
            menu.style.transform = 'scaleY(1)';
        }
    }

    layoutState.menuProfileActive = !layoutState.menuProfileActive;
}

const iconClass = computed(() => {
    const profilePositionStart = layoutConfig.menuProfilePosition === 'start';

    return {
        'pi-angle-up': (layoutState.menuProfileActive && (profilePositionStart || isHorizontal.value)) || (!layoutState.menuProfileActive && !profilePositionStart && !isHorizontal.value),
        'pi-angle-down': (!layoutState.menuProfileActive && profilePositionStart) || (layoutState.menuProfileActive && !profilePositionStart) || isHorizontal.value
    };
});

function tooltipValue(tooltipText) {
    return isSlim.value ? tooltipText : null;
}
</script>

<template>
    <div class="layout-menu-profile">
        <button v-tooltip="{ value: tooltipValue('Profile') }" @click="toggleMenu">
            <img src="../assets/fotor-20250518182628.png" alt="avatar" style="width: 32px; height: 32px" />
            <span class="text-start">
                <strong>{{name}}</strong>
                <small>{{role}}</small>
            </span>
            <i class="layout-menu-profile-toggler pi pi-fw" :class="iconClass"></i>
        </button>

        <ul :class="['menu-transition', menuClass]" style="overflow: hidden; max-height: 0; opacity: 0">
            <li v-tooltip="{ value: tooltipValue('Settings') }">
                <button @click="router.push('/settings')">
                    <i class="pi pi-cog pi-fw"></i>
                    <span>Settings</span>
                </button>
            </li>
            <li v-tooltip="{ value: tooltipValue('Support') }">
                <button @click="router.push('/Switch')">
                    <i class="pi pi-fw pi-sync pi-fw"></i>
                    <span>Switch</span>
                </button>
            </li>
            <li v-tooltip="{ value: tooltipValue('Logout') }" onclick="handleLogout">
                <button @click="router.push('/logout')">
                    <i class="pi pi-power-off pi-fw"></i>
                    <span>Logout</span>
                </button>
            </li>
        </ul>
    </div>
</template>
<script>
const name = localStorage.getItem('username');
const role = localStorage.getItem('role');

</script>
<style scoped>
.menu-transition {
    transition:
        max-height 400ms cubic-bezier(0.86, 0, 0.07, 1),
        opacity 400ms cubic-bezier(0.86, 0, 0.07, 1);
}
.menu-transition.overlay {
    transition:
        opacity 100ms linear,
        transform 120ms cubic-bezier(0, 0, 0.2, 1);
}
</style>