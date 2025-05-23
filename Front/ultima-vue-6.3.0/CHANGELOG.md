# Changelog

## 6.3.0 (2024-12-09)

**Implemented New Features and Enhancements**

-   Refactored layout composable

## 6.2.0 (2024-12-03)

**Implemented New Features and Enhancements**

-   Refactored dashboard sections to components
-   Migrate sass from @import to @use

## 6.1.0 (2024-11-27)

**Implemented New Features and Enhancements**

-   Upgrade to PrimeVue version
-   Use the new material theme
-   Visual improvements

## 6.0.0 (2024-08-27)

**Implemented New Features and Enhancements**

-   Upgrade to PrimeVue v4

**Migration Guide**

-   Remove theme files: public/layout/styles/themes folder in favor of new v4 theming.
-   Replace PrimeFlex with Tailwind CSS, for more information: https://primevue.org/tailwind/
-   PrimeVue auto-import and tree-shaking implementation is enabled. For more information: https://primevue.org/autoimport/

## 5.3.0 (2024-03-11)

**Migration Guide**

-   Update theme files.

**Implemented New Features and Enhancements**

-   Upgrade to PrimeVue 3.49.1

## 5.2.0 (2023-10-12)

**Migration Guide**

-   Update theme files.
-   public/layout/preloading.scss file moved under public/layout/styles/preloading folder.
-   public/theme folder moved under public/layout/styles/theme folder.

**Implemented New Features and Enhancements**

-   Upgrade to PrimeVue 3.36.0

## 5.1.0 (2023-07-24)

**Migration Guide**

-   Update theme files.
-   Update assets style files
-   Update docs
-   Remove code highlight

**Implemented New Features and Enhancements**

-   Upgrade to PrimeVue 3.30.2

## 5.0.0 (2023-05-11)

**Implemented New Features and Enhancements**

-   Migrated to NextGen implementation.
-   New sample apps
-   New sample pages
-   New landing
-   New dashboards

## 4.0.0 (2023-04-12)

**Implemented New Features and Enhancements**

-   Migrated to Vite from Vue-CLI

## 3.4.0 (2023-01-25)

**Migration Guide**

-   Update theme files.

**Implemented New Features and Enhancements**

-   Upgrade to PrimeVue 3.22.4
-   Upgrade to PrimeFlex 3.3.0
-   Upgrade to PrimeIcons 6.0.1
-   New styles of PrimeVue components

## 3.3.0 (2021-12-23)

**Migration Guide**

-   Update theme files and layout files.
-   public/assets/sass folder is moved to src/assets and files of public/assets folder are moved public folder.

**Implemented New Features and Enhancements**

-   Upgrade to PrimeVue 3.10.0
-   Upgrade to PrimeFlex 3.1.2
-   Upgrade to PrimeIcons 5.0.0
-   New styles of PrimeVue components
-   Removed axios

## 3.2.1 (2021-10-08)

**Fixed Bugs**
-Added missing designer

## 3.2.0 (2021-09-08)

**Migration Guide**

-   Update theme files and layout files.
-   Implemented New Features and Enhancements

**Implemented New Features and Enhancements**

-   Upgrade to PrimeVue 3.7.1 and Vue 3.1.5
-   Styles of new PrimeVue components

## 3.1.1 (2021-03-02)

**Migration Guide**

-   Update to PrimeVue 3.3.2
-   Update App\*.vue files (AppWrapper.vue, App.vue and AppSubmenu.vue)
-   Update theme files and layout files.

**Fixed Bugs**

-   Menu mode changing issues
-   Topbar theme changing issues

## 3.1.0 (2021-02-24)

**Migration Guide**

-   Update to PrimeVue 3.3.0.
-   Update App\*.vue files (AppWrapper.vue, App.vue, AppMenu.vue, AppInlineMenu.vue, AppSubmenu.vue, AppRightPanel.vue, AppTopbar.vue, AppBreadcrumb.vue, AppFooter.vue and AppConfig.vue).
-   Update main.js and router.js.
-   Use AppRightPanel.vue instead of AppRightMenu.vue.
-   Update theme files and layout files.

**Implemented New Features and Enhancements:**

-   Implemented new Ultima design
-   Added dark mode, topbar and menu colors

## 3.0.1 (2020-12-10)

**Migration Guide**

-   Update App\*.vue files (App.vue, AppMenu.vue and App.Submenu.vue)
-   Update theme and layout files.

## 3.0.0 (2020-11-26)

**Migration Guide**

-   Update Vue version to v3.
-   Update main.js, router.js and event-bus.js.
-   Update App\*.vue files (App.vue, AppMenu.vue, App.Submenu.vue, AppRightMenu.vue and AppTopbar.vue)
-   Update theme and layout files.

**Implemented New Features and Enhancements:**

-   Upgrade to Vue and PrimeVue 3

## 2.0.0 (2020-08-13)

**Migration Guide**

-   Update to PrimeVue 2.0.x
-   Update theme and layout css files
-   Update containerClass computed property in App.vue.
-   Configure ripple directive inside main.js.
-   Update AppSubmenu.vue to get ripple effect on menuitems and for full compatibility with the PrimeVue MenuModel API properties such as visible, disabled and separator.

**Implemented New Features and Enhancements:**

-   Compatibility with PrimeVue 2.0.0
