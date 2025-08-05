<template>
  <Dialog v-model:visible="localVisible" header="Update Leave Balance" :modal="true" :style="{ width: '650px' }">
    <Divider class="dialog-divider" />
    <div class="p-fluid">
        <p class="dialog-subtitle">{{ 'Enter the New leave balance (Max is 25 days per month)' }}</p>
      <div class="p-field">
        <label for="days" class="field-label">Number of Days</label>
        <InputNumber id="days" style="width: 600px;" v-model="allocationDays" :min="0" :max="25" :step="0.5" />
      </div>
    </div>
    <template #footer>
      <Button label="Cancel" icon="pi pi-times" class="add-button1" @click="cancel" v-tooltip="{
                value: 'Cancel',
                pt: {
                  arrow: {
                    style: {
                      borderBottomColor: '#000000',
                    },
                  },
                  text: '!bg-black !text-white !font-medium',
                }
              }"/>
      <Button label="Allocate" icon="pi pi-check-square" class="add-button" @click="confirm" :disabled="!allocationDays" v-tooltip="{
                value: 'Allocate',
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
    <Divider class="dialog-divider" />
  </Dialog>
</template>

<script setup>
import { ref, defineProps, defineEmits, watch } from 'vue'
import Button from 'primevue/button'
import InputNumber from 'primevue/inputnumber'
import Dialog from 'primevue/dialog'

// Props
const props = defineProps({
  visible: Boolean,
  user: Object
})

// Emits
const emit = defineEmits(['update:visible', 'allocate'])

// Local state
const localVisible = ref(props.visible)
const allocationDays = ref(null)

// Sync visible prop with local state
watch(() => props.visible, (newVal) => {
  localVisible.value = newVal
})

watch(localVisible, (newVal) => {
  if (!newVal) {
    emit('update:visible', false)
  }
})

// Methods
const cancel = () => {
  allocationDays.value = null
  emit('update:visible', false)
}

const confirm = () => {
  if (allocationDays.value) {
    emit('allocate', { user: props.user, days: allocationDays.value })
    allocationDays.value = null
    emit('update:visible', false)
  }
}
</script>

<style scoped>
.dialog-subtitle {
  margin: 0 0 1rem 0;
  color: #4b5563;
  font-size: 0.95rem;
  font-weight: 400;
  text-align: center;
}

.add-button {
  border-radius: 6px;
  padding: 0.5rem 1rem;
  font-weight: 500;
  transition: background-color 0.2s, transform 0.1s;
  color: #ff8000 !important;
  border-color: #ff8000 !important;
  background-color: white;
}

.add-button:hover {
  transform: translateY(-1px);
  background-color: #ff8000 !important;
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

.field-label {
  font-weight: 600;
  font-size: 1rem;
  color: #1f2a44;
  margin-bottom: 0.5rem;
  display: block;
}
.p-fluid .p-field {
  margin-bottom: 1rem;
}

.p-field label {
  display: block;
  margin-bottom: 0.5rem;
}
</style>