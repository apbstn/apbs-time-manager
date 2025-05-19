<script setup>
import { useLayout } from '@/layout/composables/layout';
import { updatePreset, updateSurfacePalette } from '@primevue/themes';
import Material from '@primevue/themes/material';
import { computed, onBeforeUnmount, ref, watch } from 'vue';

defineProps({
    simple: {
        type: Boolean,
        default: false
    }
});

const { layoutState, layoutConfig, isSidebarActive, isDarkTheme, toggleConfigSidebar } = useLayout();

const themeOptions = ref([
    { name: 'Light', value: false },
    { name: 'Dark', value: true }
]);
const darkTheme = ref(layoutConfig.darkTheme);
const menuMode = ref(layoutConfig.menuMode);
const menuProfilePosition = ref(layoutConfig.menuProfilePosition);
const outsideClickListener = ref(null);

const primaryColors = computed(() => {
    const presetPalette = Material.primitive;
    const colors = ['emerald', 'green', 'lime', 'orange', 'amber', 'yellow', 'teal', 'cyan', 'sky', 'blue', 'indigo', 'violet', 'purple', 'fuchsia', 'pink', 'rose'];
    const palettes = [{ name: 'noir', palette: {} }];

    colors.forEach((color) => {
        palettes.push({
            name: color,
            palette: presetPalette[color]
        });
    });

    return palettes;
});

const surfaces = ref([
    {
        name: 'slate',
        palette: { 0: '#ffffff', 50: '#f8fafc', 100: '#f1f5f9', 200: '#e2e8f0', 300: '#cbd5e1', 400: '#94a3b8', 500: '#64748b', 600: '#475569', 700: '#334155', 800: '#1e293b', 900: '#0f172a', 950: '#020617' }
    },
    {
        name: 'gray',
        palette: { 0: '#ffffff', 50: '#f9fafb', 100: '#f3f4f6', 200: '#e5e7eb', 300: '#d1d5db', 400: '#9ca3af', 500: '#6b7280', 600: '#4b5563', 700: '#374151', 800: '#1f2937', 900: '#111827', 950: '#030712' }
    },
    {
        name: 'zinc',
        palette: { 0: '#ffffff', 50: '#fafafa', 100: '#f4f4f5', 200: '#e4e4e7', 300: '#d4d4d8', 400: '#a1a1aa', 500: '#71717a', 600: '#52525b', 700: '#3f3f46', 800: '#27272a', 900: '#18181b', 950: '#09090b' }
    },
    {
        name: 'neutral',
        palette: { 0: '#ffffff', 50: '#fafafa', 100: '#f5f5f5', 200: '#e5e5e5', 300: '#d4d4d4', 400: '#a3a3a3', 500: '#737373', 600: '#525252', 700: '#404040', 800: '#262626', 900: '#171717', 950: '#0a0a0a' }
    },
    {
        name: 'stone',
        palette: { 0: '#ffffff', 50: '#fafaf9', 100: '#f5f5f4', 200: '#e7e5e4', 300: '#d6d3d1', 400: '#a8a29e', 500: '#78716c', 600: '#57534e', 700: '#44403c', 800: '#292524', 900: '#1c1917', 950: '#0c0a09' }
    },
    {
        name: 'soho',
        palette: { 0: '#ffffff', 50: '#f4f4f4', 100: '#e8e9e9', 200: '#d2d2d4', 300: '#bbbcbe', 400: '#a5a5a9', 500: '#8e8f93', 600: '#77787d', 700: '#616268', 800: '#4a4b52', 900: '#34343d', 950: '#1d1e27' }
    },
    {
        name: 'viva',
        palette: { 0: '#ffffff', 50: '#f3f3f3', 100: '#e7e7e8', 200: '#cfd0d0', 300: '#b7b8b9', 400: '#9fa1a1', 500: '#87898a', 600: '#6e7173', 700: '#565a5b', 800: '#3e4244', 900: '#262b2c', 950: '#0e1315' }
    },
    {
        name: 'ocean',
        palette: { 0: '#ffffff', 50: '#fbfcfc', 100: '#F7F9F8', 200: '#EFF3F2', 300: '#DADEDD', 400: '#B1B7B6', 500: '#828787', 600: '#5F7274', 700: '#415B61', 800: '#29444E', 900: '#183240', 950: '#0c1920' }
    }
]);

const menuThemes = ref([
    { name: 'light', color: '#FDFEFF' },
    { name: 'dark', color: '#434B54' },
    { name: 'indigo', color: '#1A237E' },
    { name: 'bluegrey', color: '#37474F' },
    { name: 'brown', color: '#4E342E' },
    { name: 'cyan', color: '#006064' },
    { name: 'green', color: '#2E7D32' },
    { name: 'deeppurple', color: '#4527A0' },
    { name: 'deeporange', color: '#BF360C' },
    { name: 'pink', color: '#880E4F' },
    { name: 'purple', color: '#6A1B9A' },
    { name: 'teal', color: '#00695C' }
]);

const topbarThemes = ref([
    { name: 'lightblue', color: '#2E88FF' },
    { name: 'dark', color: '#363636' },
    { name: 'white', color: '#FDFEFF' },
    { name: 'blue', color: '#1565C0' },
    { name: 'deeppurple', color: '#4527A0' },
    { name: 'purple', color: '#6A1B9A' },
    { name: 'pink', color: '#AD1457' },
    { name: 'cyan', color: '#0097A7' },
    { name: 'teal', color: '#00796B' },
    { name: 'green', color: '#43A047' },
    { name: 'lightgreen', color: '#689F38' },
    { name: 'lime', color: '#AFB42B' },
    { name: 'yellow', color: '#FBC02D' },
    { name: 'amber', color: '#FFA000' },
    { name: 'orange', color: '#FB8C00' },
    { name: 'deeporange', color: '#D84315' },
    { name: 'brown', color: '#5D4037' },
    { name: 'grey', color: '#616161' },
    { name: 'bluegrey', color: '#546E7A' },
    { name: 'indigo', color: '#3F51B5' }
]);

function executeDarkModeToggle() {
    layoutConfig.darkTheme = !layoutConfig.darkTheme;
    layoutConfig.menuTheme = isDarkTheme.value ? 'dark' : 'light';

    document.documentElement.classList.toggle('app-dark');
}

function toggleDarkMode() {
    if (!document.startViewTransition) {
        executeDarkModeToggle();

        return;
    }

    document.startViewTransition(() => executeDarkModeToggle(event));
}

function getPresetExt() {
    const color = primaryColors.value.find((c) => c.name === layoutConfig.primary);

    if (color.name === 'noir') {
        return {
            semantic: {
                primary: {
                    50: '{surface.50}',
                    100: '{surface.100}',
                    200: '{surface.200}',
                    300: '{surface.300}',
                    400: '{surface.400}',
                    500: '{surface.500}',
                    600: '{surface.600}',
                    700: '{surface.700}',
                    800: '{surface.800}',
                    900: '{surface.900}',
                    950: '{surface.950}'
                },
                colorScheme: {
                    light: {
                        primary: {
                            color: '{primary.950}',
                            contrastColor: '#ffffff',
                            hoverColor: '{primary.800}',
                            activeColor: '{primary.700}'
                        },
                        highlight: {
                            background: '{primary.950}',
                            focusBackground: '{primary.700}',
                            color: '#ffffff',
                            focusColor: '#ffffff'
                        }
                    },
                    dark: {
                        primary: {
                            color: '{primary.50}',
                            contrastColor: '{primary.950}',
                            hoverColor: '{primary.200}',
                            activeColor: '{primary.300}'
                        },
                        highlight: {
                            background: '{primary.50}',
                            focusBackground: '{primary.300}',
                            color: '{primary.950}',
                            focusColor: '{primary.950}'
                        }
                    }
                }
            }
        };
    } else {
        return {
            semantic: {
                primary: color.palette,
                colorScheme: {
                    light: {
                        primary: {
                            color: '{primary.500}',
                            contrastColor: '#ffffff',
                            hoverColor: '{primary.400}',
                            activeColor: '{primary.300}'
                        },
                        highlight: {
                            background: 'color-mix(in srgb, {primary.color}, transparent 88%)',
                            focusBackground: 'color-mix(in srgb, {primary.color}, transparent 76%)',
                            color: '{primary.700}',
                            focusColor: '{primary.800}'
                        }
                    },
                    dark: {
                        primary: {
                            color: '{primary.400}',
                            contrastColor: '{surface.900}',
                            hoverColor: '{primary.300}',
                            activeColor: '{primary.200}'
                        },
                        highlight: {
                            background: 'color-mix(in srgb, {primary.400}, transparent 84%)',
                            focusBackground: 'color-mix(in srgb, {primary.400}, transparent 76%)',
                            color: 'rgba(255,255,255,.87)',
                            focusColor: 'rgba(255,255,255,.87)'
                        }
                    }
                }
            }
        };
    }
}

function updateColors(type, color) {
    if (type === 'primary') {
        layoutConfig.primary = color.name;
    } else if (type === 'surface') {
        layoutConfig.surface = color.name;
    }

    applyTheme(type, color);
}

function applyTheme(type, color) {
    if (type === 'primary') {
        updatePreset(getPresetExt());
    } else if (type === 'surface') {
        updateSurfacePalette(color.palette);
    }
}

function setMenuMode(mode) {
    layoutConfig.menuMode = mode;

    if (mode === 'static') {
        layoutState.staticMenuDesktopInactive = false;
    }
}

function setProfilePosition(value) {
    layoutConfig.menuProfilePosition = value;
}

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
</script>

<template>
    <button v-if="simple" class="layout-config-button config-link" type="button" @click="toggleConfigSidebar">
        <i class="pi pi-cog"></i>
    </button>
    <Drawer
        v-model:visible="layoutState.configSidebarVisible"
        position="right"
        class="layout-config-sidebar w-80"
        header="Settings"
        :pt="{
            pcCloseButton: { root: 'ml-auto' }
        }"
    >
        <div class="flex flex-col gap-4">
            <div>
                <span class="text-lg font-semibold">Primary</span>
                <div class="pt-2 flex gap-2 flex-wrap">
                    <button
                        v-for="primaryColor of primaryColors"
                        :key="primaryColor.name"
                        :title="primaryColor.name"
                        type="button"
                        @click="updateColors('primary', primaryColor)"
                        :class="['cursor-pointer w-6 h-6 rounded-full flex flex-shrink-0 items-center justify-center p-0 outline-none outline-offset-1', { 'outline-primary': layoutConfig.primary === primaryColor.name }]"
                        :style="{ backgroundColor: `${primaryColor.name === 'noir' ? 'var(--text-color)' : primaryColor.palette['500']}` }"
                    ></button>
                </div>
            </div>

            <div>
                <span class="text-lg font-semibold">Surface</span>
                <div class="pt-2 flex gap-2 flex-wrap">
                    <button
                        v-for="surface of surfaces"
                        :key="surface.name"
                        :title="surface.name"
                        type="button"
                        @click="updateColors('surface', surface)"
                        :class="[
                            'cursor-pointer w-6 h-6 rounded-full flex flex-shrink-0 items-center justify-center p-0 outline-none outline-offset-1',
                            { 'outline-primary': layoutConfig.surface ? layoutConfig.surface === surface.name : isDarkTheme ? surface.name === 'zinc' : surface.name === 'slate' }
                        ]"
                        :style="{ backgroundColor: `${surface.palette['500']}` }"
                    ></button>
                </div>
            </div>

            <div>
                <div class="flex flex-col gap-2">
                    <span class="text-lg font-semibold">Color Scheme</span>
                    <SelectButton v-model="darkTheme" @change="toggleDarkMode" :options="themeOptions" optionLabel="name" optionValue="value" :allowEmpty="false" size="small" />
                </div>
            </div>

            <template v-if="!simple">
                <div>
                    <div class="flex flex-col gap-2">
                        <span class="text-lg font-semibold">Menu Type</span>
                        <div class="flex flex-wrap flex-col gap-3">
                            <div class="flex">
                                <div class="flex items-center gap-2 w-1/2">
                                    <RadioButton name="menuMode" value="static" v-model="menuMode" @update:modelValue="setMenuMode" inputId="static"></RadioButton>
                                    <label for="static">Static</label>
                                </div>

                                <div class="flex items-center gap-2 w-1/2">
                                    <RadioButton name="menuMode" value="overlay" v-model="menuMode" @update:modelValue="setMenuMode" inputId="overlay"></RadioButton>
                                    <label for="overlay">Overlay</label>
                                </div>
                            </div>
                            <div class="flex">
                                <div class="flex items-center gap-2 w-1/2">
                                    <RadioButton name="menuMode" value="slim" v-model="menuMode" @update:modelValue="setMenuMode" inputId="slim"></RadioButton>
                                    <label for="slim">Slim</label>
                                </div>
                                <div class="flex items-center gap-2 w-1/2">
                                    <RadioButton name="menuMode" value="slim-plus" v-model="menuMode" @update:modelValue="setMenuMode" inputId="slimplus"></RadioButton>
                                    <label for="slimplus">Slim+</label>
                                </div>
                            </div>
                            <div class="flex">
                                <div class="flex items-center gap-2 w-1/2">
                                    <RadioButton name="menuMode" value="reveal" v-model="menuMode" @update:modelValue="setMenuMode" inputId="reveal"></RadioButton>
                                    <label for="reveal">Reveal</label>
                                </div>
                                <div class="flex items-center gap-2 w-1/2">
                                    <RadioButton name="menuMode" value="drawer" v-model="menuMode" @update:modelValue="setMenuMode" inputId="drawer"></RadioButton>
                                    <label for="drawer">Drawer</label>
                                </div>
                            </div>
                            <div class="flex">
                                <div class="flex items-center gap-2 w-1/2">
                                    <RadioButton name="menuMode" value="horizontal" v-model="menuMode" @update:modelValue="setMenuMode" inputId="horizontal"></RadioButton>
                                    <label for="horizontal">Horizontal</label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div>
                    <div class="flex flex-col gap-2">
                        <span class="text-lg font-semibold">Menu Profile</span>
                        <div class="flex flex-wrap flex-col gap-4">
                            <div class="flex">
                                <div class="flex items-center gap-2 w-1/2">
                                    <RadioButton name="menuProfilePosition" value="start" v-model="menuProfilePosition" @update:modelValue="setProfilePosition" inputId="start"></RadioButton>
                                    <label for="start">Start</label>
                                </div>

                                <div class="flex items-center gap-2 w-1/2">
                                    <RadioButton name="menuProfilePosition" value="end" v-model="menuProfilePosition" @update:modelValue="setProfilePosition" inputId="end"></RadioButton>
                                    <label for="end">End</label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div>
                    <span class="text-lg font-semibold">Menu Themes</span>
                    <div v-if="!layoutConfig.darkTheme" class="pt-2 flex gap-2 flex-wrap">
                        <button
                            v-for="(theme, i) in menuThemes"
                            :key="i"
                            class="cursor-pointer w-6 h-6 rounded-full flex flex-shrink-0 items-center justify-center p-0 outline-none outline-offset-1 shadow"
                            @click="() => (layoutConfig.menuTheme = theme.name)"
                            :style="{ 'background-color': theme.color }"
                        >
                            <i v-if="theme.name === layoutConfig.menuTheme" :class="['pi pi-check', theme.name === layoutConfig.menuTheme && layoutConfig.menuTheme !== 'light' ? 'text-white' : 'text-dark']"></i>
                        </button>
                    </div>
                    <template v-else>
                        <p>Menu themes are only available in light mode by design as large surfaces can emit too much brightness in dark mode.</p>
                    </template>
                </div>

                <div>
                    <span class="text-lg font-semibold">Topbar Themes</span>
                    <div class="pt-2 flex gap-2 flex-wrap">
                        <button
                            v-for="(theme, i) in topbarThemes"
                            :key="i"
                            class="cursor-pointer w-6 h-6 rounded-full flex flex-shrink-0 items-center justify-center p-0 outline-none outline-offset-1 shadow"
                            @click="() => (layoutConfig.topbarTheme = theme.name)"
                            :style="{ 'background-color': theme.color }"
                        >
                            <i v-if="theme.name === layoutConfig.topbarTheme" :class="['pi pi-check', theme.name === layoutConfig.topbarTheme && layoutConfig.topbarTheme !== 'white' ? 'text-white' : 'text-dark']"></i>
                        </button>
                    </div>
                </div>
            </template>
        </div>
    </Drawer>
</template>
