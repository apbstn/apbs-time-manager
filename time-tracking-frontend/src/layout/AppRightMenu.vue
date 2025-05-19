<script setup>
import { useLayout } from '@/layout/composables/layout';

const { layoutState } = useLayout();
</script>

<template>
    <Drawer v-model:visible="layoutState.rightMenuActive" position="right" pt:header="!hidden" pt:content="!p-3"> </Drawer>
</template>
