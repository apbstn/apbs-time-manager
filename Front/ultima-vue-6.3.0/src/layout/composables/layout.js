import { computed, reactive } from 'vue';

const layoutConfig = reactive({
    primary: 'green',
    surface: null,
    darkTheme: false, // Keep the app in light theme
    menuMode: 'static',
    menuTheme: 'light',
    topbarTheme: 'dark', // Use a dark topbar theme (if supported by your layout)
    menuProfilePosition: 'end'
});

const layoutState = reactive({
    staticMenuDesktopInactive: false,
    overlayMenuActive: false,
    configSidebarVisible: false,
    staticMenuMobileActive: false,
    menuHoverActive: false,
    rightMenuActive: false,
    topbarMenuActive: false,
    sidebarActive: false,
    anchored: false,
    activeMenuItem: null,
    overlaySubmenuActive: false,
    menuProfileActive: false
});

export function useLayout() {
    const setActiveMenuItem = (item) => {
        layoutState.activeMenuItem = item.value || item;
    };

    const toggleMenu = () => {
        if (layoutConfig.menuMode === 'overlay') {
            layoutState.overlayMenuActive = !layoutState.overlayMenuActive;
        }

        if (window.innerWidth > 991) {
            layoutState.staticMenuDesktopInactive = !layoutState.staticMenuDesktopInactive;
        } else {
            layoutState.staticMenuMobileActive = !layoutState.staticMenuMobileActive;
        }
    };

    const toggleConfigSidebar = () => {
        if (isSidebarActive.value) {
            layoutState.overlayMenuActive = false;
            layoutState.overlaySubmenuActive = false;
            layoutState.staticMenuMobileActive = false;
            layoutState.menuHoverActive = false;
            layoutState.configSidebarVisible = false;
        }

        layoutState.configSidebarVisible = !layoutState.configSidebarVisible;
    };

    const isSidebarActive = computed(() => layoutState.overlayMenuActive || layoutState.staticMenuMobileActive || layoutState.overlaySubmenuActive);
    const isDesktop = computed(() => window.innerWidth > 991);

    const isSlim = computed(() => layoutConfig.menuMode === 'slim');
    const isSlimPlus = computed(() => layoutConfig.menuMode === 'slim-plus');
    const isHorizontal = computed(() => layoutConfig.menuMode === 'horizontal');
    const isOverlay = computed(() => layoutConfig.menuMode === 'overlay');

    const isDarkTheme = computed(() => layoutConfig.darkTheme);
    const getPrimary = computed(() => layoutConfig.primary);
    const getSurface = computed(() => layoutConfig.surface);

    return {
        layoutConfig,
        layoutState,
        getPrimary,
        getSurface,
        isDarkTheme,
        toggleConfigSidebar,
        toggleMenu,
        isSidebarActive,
        setActiveMenuItem,
        isSlim,
        isSlimPlus,
        isHorizontal,
        isDesktop,
        isOverlay
    };
}
