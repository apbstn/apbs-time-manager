<!-- src/App.vue -->
<template>
  <div>
    <DeleteDialog ref="deleteDialog" />
    <router-view />
  </div>
</template>

<script setup>
import { ref, provide } from 'vue'
import DeleteDialog from './views/Componant/DeleteDialog.vue'

const deleteDialog = ref(null)

provide('deleteDialog', deleteDialog)
</script>
<style scoped>
/* Customize PrimeVue tooltip */
:deep(.p-tooltip) {
  background-color: #000000;
  color: #ff0000;
  border-radius: 4px;
  padding: 0.5rem;
  font-size: 0.9rem;
}
:deep(.p-tooltip-arrow) {
  border-bottom-color: #000000;
}
</style>