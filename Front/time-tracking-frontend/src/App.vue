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