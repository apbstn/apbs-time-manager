<template>
  <Dialog :visible="visible" modal :header="isEdit ? 'Edit Team' : 'Add Team'" class="stunning-dialog" :draggable="false" :style="{ width: '650px' }" @update:visible="onClose">
    <Divider class="dialog-divider" />
    <div class="dialog-content">
      <p class="dialog-subtitle">{{ isEdit ? 'Update the team details below' : 'Enter team details to create a new team' }}</p>
      
      <Message v-if="validationError" severity="error" :closable="true" class="error-message" @close="validationError = ''">
        {{ validationError }}
      </Message>
      <div class="p-field">
        <label for="name" class="field-label">Team Name</label>
        <InputText id="name" v-model="form.name" :class="{ 'p-invalid': v$.name.$error || validationError }" class="stunning-input" />
        <small v-if="v$.name.$error" class="p-error">
          <i class="pi pi-exclamation-circle mr-1"></i>
          {{ v$.name.$errors[0].$message }}
        </small>
      </div>
      <br />
      <div class="p-field">
        <FloatLabel variant="on" for="description" class="field-label">Description</FloatLabel>
        <Textarea style="resize: none" cols="30" rows="5" id="description" v-model="form.description" :class="{ 'p-invalid': v$.description.$error || validationError }" class="stunning-input" />
        <small v-if="v$.description.$error" class="p-error">
          <i class="pi pi-exclamation-circle mr-1"></i>
          {{ v$.description.$errors[0].$message }}
        </small>
      </div>
    </div>
    <Divider class="dialog-divider" />
    <div class="footer-buttons">
      <Button label="Cancel" icon="pi pi-times" @click="closeDialog" class="p-button-text stunning-button stunning-button-cancel" />
      <Button label="Save" icon="pi pi-check" :loading="isSaving" @click="saveTeam" class="stunning-button stunning-button-save" :disabled="!isSavable || isSaving" />
    </div>
  </Dialog>
</template>

<script setup>
import { ref, watch, computed } from 'vue'
import { useVuelidate } from '@vuelidate/core'
import { required, helpers } from '@vuelidate/validators'
import api from '@/api'
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Divider from 'primevue/divider'
import Message from 'primevue/message'

const props = defineProps({
  visible: Boolean,
  isEdit: Boolean,
  team: Object,
  currentId: [String, Number]
})

const emit = defineEmits(['update:visible', 'refresh', 'close'])

const form = ref({
  name: '',
  description: ''
})

const isSaving = ref(false)
const validationError = ref('')

// Vuelidate rules
const rules = {
  name: { 
    required: helpers.withMessage('Team name is required.', required)
  },
  description: { 
    required: helpers.withMessage('Description is required.', required)
  }
}

const v$ = useVuelidate(rules, form)

// Computed property to enable Save button only when both fields are valid
const isSavable = computed(() => {
  return !v$.value.$invalid && form.value.name.trim() && form.value.description.trim()
})

watch(() => props.team, (newTeam) => {
  console.log('Team prop changed:', newTeam)
  if (newTeam) {
    form.value = {
      name: newTeam.name || '',
      description: newTeam.description || ''
    }
  } else {
    form.value = { name: '', description: '' }
  }
  v$.value.$reset()
  validationError.value = '' // Reset error on team change
}, { immediate: true })

watch(() => props.visible, (newVisible) => {
  console.log('Add/Edit dialog visibility changed:', newVisible)
  if (!newVisible) {
    closeDialog()
  }
})

const closeDialog = () => {
  console.log('Emitting close dialog')
  emit('update:visible', false)
  if (!props.isEdit) {
    form.value = { name: '', description: '' }
    v$.value.$reset()
    validationError.value = '' // Reset error on close
  }
}

const saveTeam = async () => {
  console.log('Saving team with payload:', form.value)
  const isValid = await v$.value.$validate()
  
  if (!isValid) {
    validationError.value = 'Please fix the validation errors.'
    console.log('Validation errors:', v$.value.$errors)
    return
  }

  try {
    isSaving.value = true
    if (props.isEdit) {
      console.log('Editing team:', props.currentId, form.value)
      await api.put(`/api/teams/${props.currentId}`, form.value)
    } else {
      console.log('Creating new team:', form.value)
      await api.post(`/api/teams`, form.value)
    }
    emit('refresh')
    closeDialog()
  } catch (error) {
    console.error('Error saving team:', error)
    validationError.value = 'Failed to save team. Please try again.'
  } finally {
    isSaving.value = false
  }
}

const onClose = (value) => {
  console.log('Add/Edit dialog close event:', value)
  emit('update:visible', value)
  closeDialog()
}
</script>

<style scoped>
.stunning-dialog {
  width: 100%;
  max-width: 550px;
  border-radius: 12px;
  background: linear-gradient(145deg, #ffffff, #f8fafc);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
  transition: all 0.3s ease;
}

.dialog-content {
  padding: 1.5rem;
}

.dialog-subtitle {
  margin: 0 0 1rem 0;
  color: #4b5563;
  font-size: 0.95rem;
  font-weight: 400;
  text-align: center;
}

.dialog-divider {
  margin: 1rem 0;
}

.field-label {
  font-weight: 600;
  color: #494949;
  margin-bottom: 0.5rem;
  display: block;
  transition: color 0.2s ease;
}

.stunning-input {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #d1d5db;
  border-bottom-color: #35D300 !important;
  /* border-bottom-width: 2px !important; */
  border-radius: 8px;
  transition: all 0.2s ease;
}

.stunning-input:focus {
  border-color: #35D300;
  box-shadow: 0 0 0 3px rgba(53, 211, 0, 0.2);
  outline: none;
}

.stunning-input::placeholder {
  color: #35D300 !important;
}

.stunning-input.p-invalid {
  border-color: #ef4444;
  background: #fef2f2;
}

.error-message {
  margin-bottom: 1rem;
  border-radius: 8px;
  border-left: 4px solid #ef4444;
  background-color: #fef2f2;
}

.footer-buttons {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  padding: 0.5rem 1rem;
}

.stunning-button {
  border-radius: 8px;
  padding: 0.5rem 1.25rem;
  font-weight: 500;
  transition: all 0.2s ease;
}

.stunning-button-cancel {
  color: #ff0000;
  border: 1px solid #ff0000;
}

.stunning-button-cancel:hover {
  background: #FF0000;
  color: #ffffff;
  border-color: #FF0000;
}

.stunning-button-save {
  background: #35d30000 !important;
  color: #35D300 !important;
  border-color: #35D300 !important;
}

.stunning-button-save:hover:not(:disabled) {
  background: #35D300;
  transform: translateY(-1px);
  box-shadow: 0 2px 4px #35d300;
}

.stunning-button-save:disabled {
  background: #d1d5db;
  cursor: not-allowed;
}

:deep(.p-dialog-header) {
  background: #ffffff;
  border-bottom: 1px solid #e5e7eb;
  padding: 1rem 1.5rem;
  font-size: 1.25rem;
  font-weight: 600;
  color: #1f2937;
}

:deep(.p-dialog-content) {
  padding: 0;
}

:deep(.p-dialog-footer) {
  padding: 0;
  border-top: 1px solid #e5e7eb;
}

:deep(.p-button) {
  transition: all 0.2s ease;
}

:deep(.p-button:hover) {
  background-color: #35D300 !important;
  box-shadow: 0 3px 6px rgba(53, 211, 0, 0.2) !important;
  color: #ffffff !important;
}

:deep(.p-button.p-button-text:hover) {
  background-color: transparent !important;
  color: #35D300 !important;
  box-shadow: none !important;
}

:deep(.p-button.p-button-text.stunning-button-cancel:hover) {
  background-color: #FF0000 !important;
  color: #ffffff !important;
  border-color: #ffffff !important;
}
</style>