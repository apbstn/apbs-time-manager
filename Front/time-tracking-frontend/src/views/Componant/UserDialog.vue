<template>
  <Dialog :visible="visible" @update:visible="emit('update:visible', $event)" :header="props.isEdit ? 'Edit User Team' : 'Add New User'" :modal="true" class="p-fluid stunning-dialog">
    <div class="dialog-content">
      <p class="dialog-subtitle">{{ props.isEdit ? 'Update the user\'s team below' : 'Enter user details below to create a new account' }}</p>
      <Divider class="dialog-divider" />
      <Message v-if="validationError" severity="error" :closable="true" class="error-message" @close="validationError = ''">
        {{ validationError }}
      </Message>
      <div class="p-field" v-if="!props.isEdit">
        <label for="email" class="field-label">Email</label>
        <InputText id="email" v-model.trim="localUser.email" placeholder="Enter email address" class="stunning-input" :class="{ 'p-invalid': validationError && !localUser.email }" />
      </div>
      <br v-if="!props.isEdit" />
      <div class="p-field" v-if="!props.isEdit">
        <label for="username" class="field-label">Username</label>
        <InputText id="username" v-model.trim="localUser.username" placeholder="Enter username" class="stunning-input" :class="{ 'p-invalid': validationError && !localUser.username }" />
      </div>
      <br v-if="!props.isEdit" />
      <div class="p-field" v-if="!props.isEdit">
        <label for="phoneNumber" class="field-label">Phone Number</label>
        <InputText id="phoneNumber" v-model.trim="localUser.phoneNumber" placeholder="Enter phone number" class="stunning-input" :class="{ 'p-invalid': validationError && !localUser.phoneNumber }" />
      </div>
      <div class="p-field">
        <label for="team" class="field-label">Team</label>
        <Dropdown id="team" v-model="selectedTeamName" :options="props.teams" optionLabel="name" optionValue="name" placeholder="Select a team" class="stunning-input" :disabled="!props.isEdit || !props.teams.length" />
      </div>
    </div>
    <Divider class="dialog-divider" />
    <div class="footer-buttons">
      <Button label="Cancel" icon="pi pi-times" @click="closeDialog" class="p-button-text stunning-button stunning-button-cancel" />
      <Button label="Save" icon="pi pi-check" @click="handleSave" class="stunning-button stunning-button-save" :disabled="!isSavable" />
    </div>
  </Dialog>
</template>

<script setup>
import { ref, defineProps, defineEmits, watch, computed } from 'vue'
import api from '@/api'
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Dropdown from 'primevue/dropdown'
import Divider from 'primevue/divider'
import Message from 'primevue/message'

const props = defineProps({
  visible: Boolean,
  isEdit: Boolean,
  user: Object,
  currentId: [String, null],
  teams: Array
})

const emit = defineEmits(['update:visible', 'refresh', 'close'])

const visible = ref(props.visible)
const localUser = ref({ ...props.user })
const selectedTeamName = ref(props.user?.team?.name || null)
const Id = ref(null)
const validationError = ref('')

watch(() => props.visible, (newVal) => {
  visible.value = newVal
  if (newVal && props.user) {
    localUser.value = { ...props.user }
    selectedTeamName.value = props.user.team?.name || null
    validationError.value = '' // Reset error on open
  }
})

watch(() => props.user, (newUser) => {
  localUser.value = { ...newUser }
  selectedTeamName.value = newUser?.team?.name || null
  validationError.value = '' // Reset error on user change
}, { deep: true })

const isSavable = computed(() => {
  if (props.isEdit) return !!selectedTeamName.value && !!props.currentId && props.teams.some(t => t.name === selectedTeamName.value)
  return !!localUser.value?.email && !!localUser.value?.username && !!localUser.value?.phoneNumber
})

const closeDialog = () => {
  emit('update:visible', false)
  emit('close')
  validationError.value = '' // Reset error on close
}

const handleSave = async () => {
  if (!isSavable.value) {
    validationError.value = 'Please fill in all required fields.'
    return
  }
  try {
    const response = await api.post('/api/UserTenants/get-id-by-email', props.currentId)
    Id.value = response.data
    console.log('Id received:', Id.value)
  } catch (error) {
    console.error('Error getting Id:', error)
    validationError.value = 'Failed to retrieve user ID.'
    return
  }
  if (props.isEdit && selectedTeamName.value && props.currentId) {
    try {
      console.log('Id sending:', Id.value)
      console.log('Team name sending:', selectedTeamName.value)
      await api.put(`/api/UserTenants/${Id.value}/team`, selectedTeamName.value)
      emit('refresh')
      closeDialog()
    } catch (error) {
      console.error('Error updating team:', error)
      validationError.value = 'Failed to update team.'
    }
  } else if (!props.isEdit) {
    try {
      // Placeholder for add logic - implement POST request if needed
      console.log('Adding user:', localUser.value)
      emit('refresh')
      closeDialog()
    } catch (error) {
      console.error('Error adding user:', error)
      validationError.value = 'Failed to add user.'
    }
  }
}
</script>

<style scoped>
.stunning-dialog {
  width: 100rem;
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
  border-radius: 8px;
  transition: all 0.2s ease;
}

.stunning-input:focus {
  border-color: #35D300;
  box-shadow: 0 0 0 3px rgba(53, 211, 0, 0.2);
  outline: none;
}

.stunning-input::placeholder {
  color: #9ca3af;
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
  box-shadow: 0 2px 4px rgba(53, 211, 0, 0.3);
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