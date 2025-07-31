<template>
  <Dialog :visible="showDialog" @update:visible="emit('update:showDialog', $event)" :style="{ width: '650px' }" header="Edit Tenant"
    :modal="true" class="p-fluid stunning-dialog">
    <Divider class="dialog-divider" />
    <div class="dialog-content">
      <p class="dialog-subtitle">Update the tenant details below</p>
      <Message v-if="validationError" severity="error" :closable="true" class="error-message"
        @close="validationError = ''">
        {{ validationError }}
      </Message>
      <div class="p-field">
        <label for="tenantname" class="field-label">Tenant Name</label>
        <InputText id="tenantname" v-model.trim="localTenant.tenantname" placeholder="Enter tenant name" class="stunning-input"
          :class="{ 'p-invalid': validationError && !localTenant.tenantname }" />
      </div>
      <br />
      <div class="p-field">
        <label for="email" class="field-label">Email (Read-only)</label>
        <InputText id="email" v-model.trim="localTenant.email" placeholder="Email" class="stunning-input" disabled />
      </div>
      <br />
      <div class="p-field">
        <label for="username" class="field-label">Username (Read-only)</label>
        <InputText id="username" v-model.trim="localTenant.username" placeholder="Username" class="stunning-input" disabled />
      </div>
      <br />
      <div class="p-field">
        <label for="phonenumber" class="field-label">Phone Number (Read-only)</label>
        <InputText id="phonenumber" v-model.trim="localTenant.phonenumber" placeholder="Phone number" class="stunning-input" disabled />
      </div>
    </div>
    <Divider class="dialog-divider" />
    <div class="footer-buttons">
      <Button label="Cancel" icon="pi pi-times" @click="emitCancel" class="p-button-text stunning-button stunning-button-cancel" v-tooltip="{
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
      <Button label="Save" icon="pi pi-check" @click="validateAndSave" class="stunning-button stunning-button-save" 
        :disabled="!isFormValid" v-tooltip="{
                value: 'Save the new details',
                pt: {
                  arrow: {
                    style: {
                      borderBottomColor: '#000000',
                    },
                  },
                  text: '!bg-black !text-white !font-medium',
                }
              }"/>
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
  tenant: {
    type: Object,
    default: () => ({
      tenantname: '',
      email: '',
      username: '',
      phonenumber: ''
    })
  }
});

// Emits
const emit = defineEmits(['update:showDialog', 'save']);

// Local reactive tenant object to manage form data
const localTenant = ref({ ...props.tenant });

// Validation error message
const validationError = ref('');

// Computed property to check if tenantname is filled
const isFormValid = computed(() => {
  return localTenant.value.tenantname.trim();
});

// Sync localTenant with props.tenant when it changes
watch(
  () => props.tenant,
  (newTenant) => {
    localTenant.value = { ...newTenant };
    validationError.value = ''; // Reset error when tenant prop changes
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
    validationError.value = 'Please fill in the tenant name.';
    return;
  }
  emit('save', localTenant.value);
  validationError.value = ''; // Reset error after successful save
};
</script>

<style scoped>
.stunning-dialog {
  max-width: 650px;
  border-radius: 12px;
  background: linear-gradient(145deg, #ffffff, #f8fafc);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
  transition: all 0.3s ease;
}

.dialog-content {
  padding: 0.5rem;
}

.dialog-subtitle {
  margin: 0 0 1rem 0;
  color: #4b5563;
  font-size: 0.95rem;
  font-weight: 400;
  text-align: center;
}

.dialog-divider {
  margin: 2px 0;
  margin-top: 2px 0;
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

.stunning-input:disabled {
  background-color: #f3f4f6;
  color: #6b7280;
}

.stunning-input:focus:not(:disabled) {
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
  border-color: #FF0000 !important;
}
</style>