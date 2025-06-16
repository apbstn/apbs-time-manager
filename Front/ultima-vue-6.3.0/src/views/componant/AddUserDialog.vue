<template>
  <Dialog :visible="showDialog" @update:visible="emit('update:showDialog', $event)" header="Add New User" :modal="true"
    class="p-fluid stunning-dialog">
    <div class="dialog-content">
      <p class="dialog-subtitle">Enter user details below to create a new account</p>
      <Divider class="dialog-divider" />
      <Message v-if="validationError" severity="error" :closable="true" class="error-message"
        @close="validationError = ''">
        {{ validationError }}
      </Message>
      <div class="p-field">
        <label for="email" class="field-label">Email</label>
        <InputText id="email" v-model.trim="localUser.email" placeholder="Enter email address" class="stunning-input"
          :class="{ 'p-invalid': validationError && !localUser.email }" />
      </div>
      <br />
      <div class="p-field">
        <label for="username" class="field-label">Username</label>
        <InputText id="username" v-model.trim="localUser.username" placeholder="Enter username" class="stunning-input"
          :class="{ 'p-invalid': validationError && !localUser.username }" />
      </div>
      <br />
      <div class="p-field">
        <label for="phoneNumber" class="field-label">Phone Number</label>
        <InputText id="phoneNumber" v-model.trim="localUser.phoneNumber" placeholder="Enter phone number"
          class="stunning-input" :class="{ 'p-invalid': validationError && !localUser.phoneNumber }" />
      </div>
    </div>
    <Divider class="dialog-divider" />
    
      
      <div class="footer-buttons">
        <Button label="Cancel" icon="pi pi-times" @click="emitCancel"
          class="p-button-text stunning-button stunning-button-cancel" />
        <Button label="Save" icon="pi pi-check" @click="validateAndSave" class="stunning-button stunning-button-save"
          :disabled="!isFormValid" />
      </div>
    
  </Dialog>
</template>

<script setup>
import { ref, watch, computed } from 'vue';
import Dialog from 'primevue/dialog';
import InputText from 'primevue/inputtext';
import Button from 'primevue/button';
import Divider from 'primevue/divider';
import Message from 'primevue/message';

// Props
const props = defineProps({
  showDialog: {
    type: Boolean,
    required: true
  },
  user: {
    type: Object,
    default: () => ({
      email: '',
      username: '',
      phoneNumber: ''
    })
  }
});

// Emits
const emit = defineEmits(['update:showDialog', 'save']);

// Local reactive user object to manage form data
const localUser = ref({ ...props.user });

// Validation error message
const validationError = ref('');

// Computed property to check if all fields are filled
const isFormValid = computed(() => {
  return (
    localUser.value.email.trim() &&
    localUser.value.username.trim() &&
    localUser.value.phoneNumber.trim()
  );
});

// Sync localUser with props.user when it changes
watch(
  () => props.user,
  (newUser) => {
    localUser.value = { ...newUser };
    validationError.value = ''; // Reset error when user prop changes
  },
  { deep: true }
);

// Watch showDialog to reset validation error when dialog opens
watch(
  () => props.showDialog,
  (newValue) => {
    if (newValue) {
      validationError.value = ''; // Reset error when dialog opens
    }
  }
);

// Emit cancel event to close dialog
const emitCancel = () => {
  emit('update:showDialog', false);
  validationError.value = ''; // Reset error on cancel
};

// Validate and emit save event
const validateAndSave = () => {
  if (!isFormValid.value) {
    validationError.value = 'Please fill in all fields.';
    return;
  }
  emit('save', localUser.value);
  validationError.value = ''; // Reset error after successful save
};
</script>

<style scoped>
.stunning-dialog {
  width: 100rem;
  /* Increased from 32rem to make dialog wider */
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
  /* Changed from #3b82f6 (blue) to green */
  box-shadow: 0 0 0 3px rgba(53, 211, 0, 0.2);
  /* Changed from rgba(59, 130, 246, 0.2) to green */
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
  /* Red for cancel hover */
  color: #ffffff;
  /* White text for contrast */
  border-color: #FF0000;
  /* Match border to hover */
}

.stunning-button-save {
  background: #35d30000 !important;
  /* Green for save */
  color: #35D300 !important;
  border-color: #35D300 !important;
}

.stunning-button-save:hover:not(:disabled) {
  background: #35D300;
  /* Ensure green hover */
  transform: translateY(-1px);
  box-shadow: 0 2px 4px rgba(53, 211, 0, 0.3);
  /* Green shadow */
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
  /* Override blue hover for all buttons */
  box-shadow: 0 3px 6px rgba(53, 211, 0, 0.2) !important;
  /* Green shadow */
  color: #ffffff !important;
  /* White text on hover for contrast */
}

:deep(.p-button.p-button-text:hover) {
  background-color: transparent !important;
  /* Keep text buttons transparent */
  color: #35D300 !important;
  /* Green text on hover for text buttons */
  box-shadow: none !important;
  /* No shadow for text buttons */
}

:deep(.p-button.p-button-text.stunning-button-cancel:hover) {
  background-color: #FF0000 !important;
  /* Red hover for cancel */
  color: #ffffff !important;
  /* White text */
  border-color: #ffffff !important;
  /* Match border */
}
</style>