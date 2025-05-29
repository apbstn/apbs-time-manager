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
                    <li class="chronometer-container">
                        <span v-if="isInitialized && (showStart || showStop)" class="chronometer">{{ chronometerTime }}</span>
                    </li>
                    <li>
                        <button v-if="isInitialized && showStart" class="pi pi-play-circle" style="color: green; font-size: 2rem;" @click="customStartTracking"></button>
                    </li>
                    <li>
                        <button v-if="isInitialized && showStop" class="pi pi-stop-circle" style="color: red; font-size: 2rem;" @click="customStopTracking"></button>
                    </li>
                    <li>
                        <a v-styleclass="{ selector: '@next', enterFromClass: 'hidden', enterActiveClass: 'animate-scalein', leaveToClass: 'hidden', leaveActiveClass: 'animate-fadeout', hideOnOutsideClick: true }" class="">
                            <OverlayBadge severity="warn" :pt="{ pcBadge: { root: '!outline-0' } }">
                                <i class="pi pi-bell !align-middle"></i>
                            </OverlayBadge>
                        </a>
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
const { initialize, startTracking, stopTracking, showStart, showStop } = useTimeTracking();

const isInitialized = ref(false);
const chronometerInterval = ref(null);
const elapsedTime = ref(0); // Tracks elapsed time in seconds
const startTime = ref(null); // Stores the start time of the tracking session

// Computed property to format elapsed time as HH:MM:SS
const chronometerTime = computed(() => {
    const hours = Math.floor(elapsedTime.value / 3600).toString().padStart(2, '0');
    const minutes = Math.floor((elapsedTime.value % 3600) / 60).toString().padStart(2, '0');
    const seconds = (elapsedTime.value % 60).toString().padStart(2, '0');
    return `${hours}:${minutes}:${seconds}`;
});

// Calculate elapsed time based on the start time
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

// Start the chronometer
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

// Stop the chronometer (but don't reset elapsedTime)
const stopChronometer = () => {
    if (chronometerInterval.value) {
        console.log('Stopping chronometer at:', new Date().toLocaleString());
        clearInterval(chronometerInterval.value);
        chronometerInterval.value = null;
        // Do not reset elapsedTime here; keep it for display
        startTime.value = null; // Clear start time
        localStorage.removeItem('chronometerStartTime'); // Clear persisted start time
        console.log('Cleared startTime and localStorage at:', new Date().toLocaleString());
    }
};

// Override startTracking to include chronometer start
const customStartTracking = async () => {
    console.log('Custom start tracking called at:', new Date().toLocaleString());
    const now = new Date().toISOString();
    startTime.value = now; // Set start time
    localStorage.setItem('chronometerStartTime', now); // Persist start time
    await startTracking();
    startChronometer(); // Start immediately after tracking begins
};

// Override stopTracking to include chronometer stop
const customStopTracking = async () => {
    console.log('Custom stop tracking called at:', new Date().toLocaleString());
    await stopTracking();
    stopChronometer(); // Stop without resetting elapsedTime
};

// Watch showStop to manage chronometer state
watch(showStop, (newValue) => {
    if (newValue && startTime.value) {
        startChronometer(); // Resume chronometer if tracking is active
    } else if (!newValue && chronometerInterval.value) {
        stopChronometer(); // Stop if tracking is not active
    }
});

onMounted(async () => {
    // Load persisted start time from localStorage
    const savedStartTime = localStorage.getItem('chronometerStartTime');
    if (savedStartTime) {
        startTime.value = savedStartTime;
        console.log('Loaded startTime from localStorage:', startTime.value, 'at:', new Date().toLocaleString());
    }

    await initialize();
    isInitialized.value = true;

    // Calculate elapsed time if tracking is active
    if (showStop.value && startTime.value) {
        calculateElapsedTime();
        startChronometer();
    } else if (!showStop.value) {
        elapsedTime.value = 0; // Reset if not tracking
        localStorage.removeItem('chronometerStartTime');
    }

    console.log('layout-topbar mounted - showStart:', showStart.value, 'showStop:', showStop.value, 'elapsedTime:', elapsedTime.value, 'at:', new Date().toLocaleString());
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

.chronometer-container {
    margin-right: 0.5rem;
}

.chronometer {
    font-size: 1.5rem;
    font-weight: 500;
    color: #1f2937;
    padding: 0.25rem 0.5rem;
    background-color: #f3f4f6;
    border-radius: 4px;
}
</style>