<template>
  <Dialog :visible="showDialog" @update:visible="emit('update:showDialog', $event)" header="Add New Tenant"
    :modal="true" class="p-fluid stunning-dialog" style="min-width: 650px;">

    <div class="dialog-content">
      <p class="dialog-subtitle">Enter tenant details below to create a new tenant</p>
      <Divider class="-dialog-divider" />
      <Message v-if="validationError" severity="error" :closable="true" class="error-message"
        @close="validationError = ''">
        {{ validationError }}
      </Message>
      <div class="p-field">
        <label for="add-tenantname" class="field-label">Tenant Name</label>
        <InputText id="add-tenantname" v-model.trim="localTenant.tenantname" placeholder="Enter tenant name"
          class="stunning-input" :class="{ 'p-invalid': validationError && !localTenant.tenantname }" />
      </div>
      <br />
      <div class="p-field">
        <label for="add-user" class="field-label">Owner</label>
        <Dropdown id="add-user" v-model="localTenant.userId" :options="filteredUsersList" optionLabel="username"
          optionValue="id" placeholder="Select owner" class="stunning-input"
          :class="{ 'p-invalid': validationError && !localTenant.userId }" filter />
      </div>
    </div>

    <Divider class="-dialog-divider" />
    <div class="footer-buttons">
      <Button label="Cancel" icon="pi pi-times" @click="emitCancel"
        class="p-button-text stunning-button stunning-button-cancel" v-tooltip="{
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
                value: 'Add Tenant',
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
import Dropdown from 'primevue/dropdown';
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
      userId: null
    })
  },
  usersList: {
    type: Array,
    default: () => []
  }
});

// Emits
const emit = defineEmits(['update:showDialog', 'save']);

// Local reactive tenant object to manage form data
const localTenant = ref({ ...props.tenant });

// Validation error message
const validationError = ref('');

// Computed property to filter out admin@example.com from usersList
const filteredUsersList = computed(() => {
  // Return empty array if usersList is not an array or is empty
  if (!Array.isArray(props.usersList) || props.usersList.length === 0) {
    return [];
  }
  // Filter out users where email (or username as fallback) is admin@example.com
  return props.usersList.filter(user => {
    // Check email field first, then fallback to username or other fields if needed
    const email = user.email || user.emailAddress || user.username || '';
    return email.toLowerCase() !== 'admin@example.com';
  });
});

// Computed property to check if all fields are filled
const isFormValid = computed(() => {
  return localTenant.value.tenantname.trim() && localTenant.value.userId;
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

// Watch showDialog to reset validation error and admin userId when dialog opens
watch(
  () => props.showDialog,
  (newValue) => {
    if (newValue) {
      validationError.value = ''; // Reset error when dialog opens
      // Reset userId if it corresponds to admin@example.com
      const adminUser = props.usersList.find(user => {
        const email = user.email || user.emailAddress || user.username || '';
        return email.toLowerCase() === 'admin@example.com';
      });
      if (adminUser && localTenant.value.userId === adminUser.id) {
        localTenant.value.userId = null;
      }
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
  emit('save', localTenant.value);
  validationError.value = ''; // Reset error after successful save
};
</script>

<style scoped>
.stunning-dialog {
  width: 32rem;
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
  border-color: #35D300 !important;
  box-shadow: 0 0 0 3px #35D300 !important;
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
  color: #ff0000 !important;
  border: 1px solid #ff0000 !important;
}

.stunning-button-cancel:hover {
  background: #ff0000 !important;
  color: white !important;
  border-color: #ffffff !important;
}

.stunning-button-save {
  background: #3b83f600;
  color: #35D300 !important;
  border: 1px solid #35D300 !important;
}

.stunning-button-save:hover:not(:disabled) {
  background: #35D300 !important;
  color: white !important;
  border-color: white !important;
  transform: translateY(-1px);
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

:deep(.p-dropdown) {
  width: 100%;
  border-radius: 8px;
}

:deep(.p-dropdown:focus) {
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.2);
}
</style>