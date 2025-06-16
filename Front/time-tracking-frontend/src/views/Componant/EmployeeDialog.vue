<template>
  <div>
    <Dialog :visible="visible" modal :header="isEdit ? 'Edit an employee' : 'Add an employee'"
            class="employee-dialog" :draggable="false" :style="{ maxWidth: '650px' }" @update:visible="onClose">
      <div class="p-fluid form-container">
        <div class="field mb-4">
          <label for="name" class="field-label">Name</label>
          <InputText v-model="form.name" id="name" :class="{ 'p-invalid': v$.name.$error }" />
          <small v-if="v$.name.$error" class="p-error">
            <i class="pi pi-exclamation-circle mr-1"></i>
            {{ v$.name.$errors[0].$message }}
          </small>
        </div>
        <Divider />
        <div class="field mb-4">
          <label for="email" class="field-label">Email</label>
          <InputText v-model="form.email" id="email" :class="{ 'p-invalid': v$.email.$error }" />
          <small v-if="v$.email.$error" class="p-error">
            <i class="pi pi-exclamation-circle mr-1"></i>
            {{ v$.email.$errors[0].$message }}
          </small>
        </div>
        <Divider />
        <div class="field mb-4">
          <label for="role" class="field-label">Role</label>
          <Dropdown v-model="form.role" :options="roles" optionLabel="label" optionValue="value"
                    placeholder="Select a role" id="role" :class="{ 'p-invalid': v$.role.$error }" />
          <small v-if="v$.role.$error" class="p-error">
            <i class="pi pi-exclamation-circle mr-1"></i>
            {{ v$.role.$errors[0].$message }}
          </small>
        </div>
      </div>

      <template #footer>
        <Button label="Cancel" icon="pi pi-times" class="add-button1" @click="closeDialog" />
        <Button label="Save" icon="pi pi-check" class="add-button" :loading="isSaving" @click="saveEmployee" />
      </template>
    </Dialog>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'
import { useVuelidate } from '@vuelidate/core'
import { required, email, helpers } from '@vuelidate/validators'
import InputText from 'primevue/inputtext'
import Dropdown from 'primevue/dropdown'
import Dialog from 'primevue/dialog'
import Divider from 'primevue/divider'
import Button from 'primevue/button'

const props = defineProps({
  visible: Boolean,
  isEdit: Boolean,
  employee: Object
})

const emit = defineEmits(['update:visible', 'save', 'close'])

const roles = ref([
  { label: 'Développeur', value: 'Développeur' },
  { label: 'Designer', value: 'Designer' },
  { label: 'Manager', value: 'Manager' },
  { label: 'RH', value: 'RH' }
])

const form = ref({
  id: null,
  name: '',
  email: '',
  role: null
})

const isSaving = ref(false)

const rules = {
  name: { 
    required: helpers.withMessage('Name is required.', required)
  },
  email: { 
    required: helpers.withMessage('Email is required.', required),
    email: helpers.withMessage('Invalid email format.', email)
  },
  role: { 
    required: helpers.withMessage('Role is required.', required)
  }
}

const v$ = useVuelidate(rules, form)

watch(() => props.employee, (newEmployee) => {
  if (newEmployee) {
    form.value = {
      id: newEmployee.id || null,
      name: newEmployee.name || '',
      email: newEmployee.email || '',
      role: newEmployee.role || null
    }
  } else {
    form.value = { id: null, name: '', email: '', role: null }
  }
  v$.value.$reset()
}, { immediate: true })

watch(() => props.visible, (newVisible) => {
  if (!newVisible) {
    closeDialog()
  }
})

const closeDialog = () => {
  emit('update:visible', false)
  if (!props.isEdit) {
    form.value = { id: null, name: '', email: '', role: null }
    v$.value.$reset()
  }
}

const saveEmployee = async () => {
  await v$.value.$validate()
  if (v$.value.$invalid) {
    return
  }
  try {
    isSaving.value = true
    emit('save', { ...form.value })
    closeDialog()
  } catch (error) {
    console.error('Error saving employee:', error)
  } finally {
    isSaving.value = false
  }
}

const onClose = (value) => {
  emit('update:visible', value)
  closeDialog()
}
</script>

<style scoped>
.employee-dialog {
  width: 90%;
  max-width: 650px;
  border-radius: 12px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.2);
  background: #ffffff;
}

.form-container {
  padding: 1.5rem;
  background: #f9fafb;
  border-radius: 8px;
}

.field {
  margin-bottom: 1.5rem;
}

.field-label {
  font-weight: 600;
  font-size: 1rem;
  color: #1f2a44;
  margin-bottom: 0.5rem;
  display: block;
}

:deep(.p-inputtext), :deep(.p-dropdown) {
  border-radius: 6px;
  border: 1px solid #ced4da;
  transition: border-color 0.2s, box-shadow 0.2s;
}

:deep(.p-inputtext:focus), :deep(.p-dropdown:focus) {
  border-color: #6366f1;
  box-shadow: 0 0 0 3px rgba(99, 102, 241, 0.2);
}

:deep(.p-invalid) {
  border-color: #dc3545 !important;
  animation: shake 0.3s ease;
}

.p-error {
  color: #dc3545;
  font-size: 0.875rem;
  margin-top: 0.25rem;
  display: flex;
  align-items: center;
}

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

@keyframes shake {
  0%, 100% { transform: translateX(0); }
  25% { transform: translateX(-5px); }
  75% { transform: translateX(5px); }
}
</style>