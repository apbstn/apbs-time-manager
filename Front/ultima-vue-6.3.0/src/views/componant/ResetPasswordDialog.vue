<template>
  <Dialog :visible="showDialog" @update:visible="emit('update:showDialog', $event)" :style="{ width: '650px' }" header="Reset Password"
    :modal="true" class="p-fluid stunning-dialog">
    <Divider class="dialog-divider" />
    <div class="dialog-content">
      <p class="dialog-subtitle">
        Are you sure you want to reset the password for
        <strong>{{ localUser.email }}</strong>?
      </p>
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
      <Button label="Confirm" icon="pi pi-check" @click="save" class="stunning-button stunning-button-save" v-tooltip="{
                value: 'Confirme Reseting the password',
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
import { ref, watch } from 'vue';
import Dialog from 'primevue/dialog';
import Button from 'primevue/button';
import Divider from 'primevue/divider';
import axios from 'axios';

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

// Sync localUser with props.user when it changes
watch(
  () => props.user,
  (newUser) => {
    localUser.value = { ...newUser };
  },
  { deep: true }
);

// Watch showDialog to reset state when dialog opens
watch(
  () => props.showDialog,
  (newValue) => {
    if (newValue) {
      localUser.value = { ...props.user }; // Reset to initial user data
    }
  }
);

// Emit cancel event to close dialog
const emitCancel = () => {
  emit('update:showDialog', false);
};

// Save (confirm) password reset
const save = async () => {
  try {
    const token = localStorage.getItem('accessToken');
    if (!token) {
      throw new Error('No access token found');
    }

    const payload = {
      email: localUser.value.email,
      username: localUser.value.username,
      phoneNumber: localUser.value.phoneNumber || ''
    };

    console.log('Resetting password for:', payload);
    const response = await axios.patch('http://localhost:58169/api/user/resetPass', payload, {
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
        Accept: 'application/json'
      }
    });

    console.log('Reset password response:', response.status, response.data);
    emit('save', { user: localUser.value, response: response.data });
    emit('update:showDialog', false);
  } catch (error) {
    console.error('Error resetting password:', error.response ? error.response.data : error.message);
    emit('save', { user: localUser.value, error: error.response?.data?.message || error.message });
    emit('update:showDialog', false);
  }
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
  padding: 1.5rem;
}

.dialog-subtitle {
  margin: 0 0 1rem 0;
  color: #4b5563;
  font-size: 20px;
  font-weight: 400;
  text-align: center;
}

.dialog-subtitle strong {
  color: #1f2937;
  font-weight: 600;
}

.dialog-divider {
  margin: 1rem 0;
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
  color: #FF8000 !important;
  border-color: #FF8000 !important;
}

.stunning-button-save:hover:not(:disabled) {
  background: #FF8000;
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
  background-color: #FF8000 !important;
  /* Override blue hover for all buttons */
  box-shadow: 0 3px 6px rgba(53, 211, 0, 0.2) !important;
  /* Green shadow */
  color: #ffffff !important;
  /* White text on hover for contrast */
}

:deep(.p-button.p-button-text:hover) {
  background-color: transparent !important;
  /* Keep text buttons transparent */
  color: #FF8000 !important;
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