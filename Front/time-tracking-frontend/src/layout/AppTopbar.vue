<template>
    <div class="layout-topbar">
        <div class="layout-topbar-start">
            <img src="../assets/logo2.png" alt="logo">
            <a ref="menuButton" class="layout-menu-button" @click="toggleMenu">
                <i class="pi pi-chevron-right"></i>
            </a>
            <a ref="mobileMenuButton" class="layout-topbar-mobile-button" @click="onTopbarMenuToggle">
                <i class="pi pi-ellipsis-v" style="color: #35D300 !important;"></i>
            </a>
        </div>
        <div class="layout-topbar-end">
            <div class="layout-topbar-actions-start">{{ nameoftenant }}</div>
            <div class="layout-topbar-actions-end">
                <ul class="layout-topbar-items">
                    <li class="chronometer-container">
                        <span v-if="isInitialized && (showStart || showStop || showPause)" class="chronometer">{{ chronometerTime }}</span>
                    </li>
                    <li>
                        <button v-if="isInitialized && showStart" class="pi pi-play-circle" style="color: #35D300; font-size: 2rem;" @click="customStartTracking"></button>
                    </li>
                    <li>
                        <button v-if="isInitialized && showPause" class="pi pi-pause-circle" style="color: #ff8000; font-size: 2rem;" @click="customPauseTracking"></button>
                    </li>
                    <li>
                        <button v-if="isInitialized && showStop" class="pi pi-stop-circle" style="color: #FF0000; font-size: 2rem;" @click="customStopTracking"></button>
                    </li>
                </ul>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted, computed, watch } from 'vue';
import { useLayout } from '@/layout/composables/layout';
import { useTimeTracking } from './composables/useTimeTracking';

const { layoutState, toggleMenu, toggleConfigSidebar } = useLayout();
const { initialize, startTracking, stopTracking, pauseTracking, showStart, showStop, showPause } = useTimeTracking();

const isInitialized = ref(false);
const chronometerInterval = ref(null);
const elapsedTime = ref(0);
const startTime = ref(null);
const nameoftenant = localStorage.getItem('Name_of_tenant');
console.log('nameoftenant', nameoftenant);

const chronometerTime = computed(() => {
    const hours = Math.floor(elapsedTime.value / 3600).toString().padStart(2, '0');
    const minutes = Math.floor((elapsedTime.value % 3600) / 60).toString().padStart(2, '0');
    const seconds = (elapsedTime.value % 60).toString().padStart(2, '0');
    return `${hours}:${minutes}:${seconds}`;
});

const calculateElapsedTime = () => {
    if (startTime.value) {
        const now = new Date().getTime();
        const start = new Date(startTime.value).getTime();
        elapsedTime.value = Math.floor((now - start) / 1000);
        console.log('Calculated elapsed time on load - elapsedTime:', elapsedTime.value, 'at:', new Date().toLocaleString());
    } else {
        elapsedTime.value = 0;
    }
};

const startChronometer = () => {
    if (!chronometerInterval.value) {
        console.log('Starting chronometer at:', new Date().toLocaleString());
        chronometerInterval.value = setInterval(() => {
            if (startTime.value) {
                const now = new Date().getTime();
                const start = new Date(startTime.value).getTime();
                elapsedTime.value = Math.floor((now - start) / 1000);
                console.log('Chronometer tick - elapsedTime:', elapsedTime.value, 'at:', new Date().toLocaleString());
            }
        }, 1000);
    }
};

const pauseChronometer = () => {
    if (chronometerInterval.value) {
        console.log('Pausing chronometer at:', new Date().toLocaleString());
        clearInterval(chronometerInterval.value);
        chronometerInterval.value = null;
        // Keep elapsedTime and startTime for resume
    }
};

const stopChronometer = () => {
    if (chronometerInterval.value) {
        console.log('Stopping chronometer at:', new Date().toLocaleString());
        clearInterval(chronometerInterval.value);
        chronometerInterval.value = null;
        startTime.value = null;
        localStorage.removeItem('chronometerStartTime');
        console.log('Cleared startTime and localStorage at:', new Date().toLocaleString());
    }
};

const customStartTracking = async () => {
    console.log('Custom start tracking called at:', new Date().toLocaleString());
    const now = new Date().toISOString();
    startTime.value = now;
    localStorage.setItem('chronometerStartTime', now);
    await startTracking();
    startChronometer();
};

const customPauseTracking = async () => {
    console.log('Custom pause tracking called at:', new Date().toLocaleString());
    if (startTime.value) {
        await pauseTracking();
        pauseChronometer(); // Pause the chronometer
    } else {
        console.warn('No active tracking to pause at:', new Date().toLocaleString());
    }
};

const customStopTracking = async () => {
    console.log('Custom stop tracking called at:', new Date().toLocaleString());
    await stopTracking();
    stopChronometer();
};

watch(() => [showStart.value, showPause.value, showStop.value], ([newShowStart, newShowPause, newShowStop]) => {
    if (newShowStop && startTime.value) {
        startChronometer();
    } else if (!newShowStop && !newShowPause && chronometerInterval.value) {
        stopChronometer();
    } else if (newShowPause && startTime.value) {
        pauseChronometer(); // Pause when pause button is visible
    }
});

onMounted(async () => {
    const savedStartTime = localStorage.getItem('chronometerStartTime');
    if (savedStartTime) {
        startTime.value = savedStartTime;
    }

    await initialize();
    isInitialized.value = true;

    if (showStop.value && startTime.value) {
        calculateElapsedTime();
        startChronometer();
    } else if (!showStop.value) {
        elapsedTime.value = 0;
        localStorage.removeItem('chronometerStartTime');
    }

    console.log('layout-topbar mounted - showStart:', showStart.value, 'showPause:', showPause.value, 'showStop:', showStop.value, 'elapsedTime:', elapsedTime.value, 'at:', new Date().toLocaleString());
});

function onTopbarMenuToggle() {
    layoutState.topbarMenuActive = !layoutState.topbarMenuActive;
}
</script>

<style scoped>
.layout-topbar-items {
    display: flex;
    align-items: center;
    gap: 1rem;
}

.layout-topbar-mobile-button {
    margin-left: auto !important;
}

.chronometer-container {
    margin-right: 0.5rem;
}

.chronometer {
    font-size: 1.5rem;
    font-weight: 500;
    color: #000000;
    padding: 0.25rem 0.5rem;
    background-color: #f3f4f6;
    border-radius: 4px;
}

.layout-topbar-actions-start {
    font-size: 18px !important;
    font-weight: 400;
    color: #ffffff;
}
</style>