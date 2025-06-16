<template>
  <Dialog :visible="showDialog" @update:visible="emit('update:showDialog', $event)" header="Reset Password"
    :modal="true" class="p-fluid stunning-dialog">
    <div class="dialog-content">
      <p class="dialog-subtitle">
        Are you sure you want to reset the password for
        <strong>{{ user.email }}</strong>?
      </p>
    </div>
    <template #footer>
      <Divider class="dialog-divider" />
      <div class="footer-buttons">
        <Button label="Cancel" icon="pi pi-times" @click="emitCancel"
          class="p-button-text stunning-button stunning-button-cancel" />
        <Button label="Confirm" icon="pi pi-check" @click="emitConfirm"
          class="stunning-button stunning-button-confirm" />
      </div>
    </template>
  </Dialog>
</template>

<script setup>
import Dialog from 'primevue/dialog';
import Button from 'primevue/button';
import Divider from 'primevue/divider';

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
const emit = defineEmits(['update:showDialog', 'confirm']);

// Emit cancel event to close dialog
const emitCancel = () => {
  emit('update:showDialog', false);
};

// Emit confirm event to trigger password reset
const emitConfirm = () => {
  emit('confirm', props.user);
  emit('update:showDialog', false);
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
  font-size: 1.2rem;
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
  background: #ff0000 !important; /* Ensure red background on hover */
  border-color: #ff0000 !important;
  color: #ffffff !important; /* Ensure white text on hover */
}

.stunning-button-confirm:hover {
  background: #35D300 !important;
  color: #ffffff !important;
  border: none;
}

.stunning-button-confirm {
  background: #35d30000 !important;
  color: #35D300 !important;
  border-color: #35D300 !important;
  transform: translateY(-1px);
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
</style>