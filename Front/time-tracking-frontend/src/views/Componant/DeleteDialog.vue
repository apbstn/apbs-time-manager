<template>
    <Dialog :visible="isVisible" modal header="Confirm Delete" :style="{ width: '650px' }" @update:visible="hideDeleteDialog">
        <div class="flex align-items-center gap-3 p-3">
            <i class="pi pi-exclamation-triangle" style="font-size: 2rem; color: #dc3545;"></i>
            <span>Are you sure you want to delete this {{ itemType }} "{{ itemName }}"?</span>
        </div>
        <template #footer>
            <Button label="No" icon="pi pi-times" class="add-button1" @click="hideDeleteDialog" v-tooltip="{
                value: 'Cancel Delete',
                pt: {
                  arrow: {
                    style: {
                      borderBottomColor: '#000000',
                    },
                  },
                  text: '!bg-black !text-white !font-medium',
                }
              }"/>
            <Button label="Yes" icon="pi pi-check" class="add-button" @click="confirmDelete" v-tooltip="{
                value: 'Confirm Deletion',
                pt: {
                  arrow: {
                    style: {
                      borderBottomColor: '#000000',
                    },
                  },
                  text: '!bg-black !text-white !font-medium',
                }
              }"/>
        </template>
    </Dialog>
</template>

<!-- src/views/componant/DeleteDialog.vue -->
<script setup>
import { ref, onMounted } from 'vue'
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'

const isVisible = ref(false)
const item = ref(null)
const itemType = ref('')
const itemName = ref('')
const onConfirmCallback = ref(null)

const showDeleteDialog = ({ item: deleteItem, type, name, onConfirm }) => {
    console.log('DeleteDialog.vue: Showing delete dialog for:', { type, name })
    item.value = deleteItem
    itemType.value = type
    itemName.value = name
    onConfirmCallback.value = onConfirm
    isVisible.value = true
}

const hideDeleteDialog = () => {
    console.log('DeleteDialog.vue: Hiding delete dialog')
    isVisible.value = false
    item.value = null
    itemType.value = ''
    itemName.value = ''
    onConfirmCallback.value = null
}

const confirmDelete = () => {
    console.log('DeleteDialog.vue: Confirming delete for:', itemType.value, itemName.value)
    if (onConfirmCallback.value) {
        onConfirmCallback.value(item.value)
    }
    hideDeleteDialog()
}

defineExpose({
    showDeleteDialog
})

onMounted(() => {
    console.log('DeleteDialog.vue: DeleteDialog mounted')
})
</script>

<style scoped>
.add-button {
  border-radius: 6px;
  padding: 0.5rem 1rem;
  font-weight: 500;
  transition: background-color 0.2s, transform 0.1s;
  color: #35D300 !important;
  border-color: #35D300 !important;
  background-color: white;
}

.add-button:hover {
  transform: translateY(-1px);
  background-color: #35D300 !important;
  color: white !important;
  border-color: white !important;
}
.add-button1 {
  border-radius: 6px;
  padding: 0.5rem 1rem;
  font-weight: 500;
  transition: background-color 0.2s, transform 0.1s;
  color: #ff0000 !important;
  border-color: #ff0000 !important;
  background-color: white;
}

.add-button1:hover {
  transform: translateY(-1px);
  background-color: #ff0000 !important;
  color: white !important;
  border-color: white !important;
}
:deep(.p-dialog) {
    border-radius: 12px;
    box-shadow: 0 8px 24px rgba(0, 0, 0, 0.2);
    background: #ffffff;
}

:deep(.p-dialog-header) {
    background: #ffffff;
    border-bottom: 1px solid #e5e7eb;
    padding: 1rem 1.5rem;
    font-size: 1.25rem;
    font-weight: 600;
}

:deep(.p-dialog-content) {
    padding: 0;
}

:deep(.p-dialog-footer) {
    border-top: 1px solid #e5e7eb;
    padding: 1rem 1.5rem;
    display: flex;
    justify-content: flex-end;
    gap: 0.5rem;
}

.custom-button {
    border-radius: 6px;
    padding: 0.5rem 1rem;
    font-weight: 500;
    transition: background-color 0.2s, transform 0.1s;
}

.custom-button:hover {
    transform: translateY(-1px);
}

:deep(.p-button-secondary) {
    color: #6b7280;
}

:deep(.p-button-secondary:hover) {
    background: #f3f4f6;
}

:deep(.p-button-danger) {
    background: #dc3545;
    border-color: #dc3545;
}

:deep(.p-button-danger:hover) {
    background: #b91c1c;
    border-color: #b91c1c;
}
</style>