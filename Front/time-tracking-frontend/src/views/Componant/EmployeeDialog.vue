<template>
  <div>
    <!-- Add/Edit Dialog -->
    <Dialog :visible="visible" modal :header="isEdit ? 'Edit Leave Request' : 'Add Leave Request'"
      class="leave-request-dialog" :draggable="false" :style="{ maxWidth: '650px' }" @update:visible="onClose">
      <div class="p-fluid form-container">
        <div class="field mb-4">
          <label for="startDate" class="field-label">Start Date</label>
          <Calendar v-model="form.startDate" id="startDate" dateFormat="yy-mm-dd" showIcon
            :class="{ 'p-invalid': v$.startDate.$error }" />
          <small v-if="v$.startDate.$error" class="p-error">
            <i class="pi pi-exclamation-circle mr-1"></i>
            {{ v$.startDate.$errors[0].$message }}
          </small>
        </div>
        <div class="field mb-4">
          <label for="endDate" class="field-label">End Date</label>
          <Calendar v-model="form.endDate" id="endDate" dateFormat="yy-mm-dd" showIcon
            :class="{ 'p-invalid': v$.endDate.$error }" />
          <small v-if="v$.endDate.$error" class="p-error">
            <i class="pi pi-exclamation-circle mr-1"></i>
            {{ v$.endDate.$errors[0].$message }}
          </small>
        </div>
        <div class="field mb-4">
          <label for="type" class="field-label">Type</label>
          <Dropdown v-model="form.type" :options="leaveTypes" optionLabel="label" optionValue="value"
            placeholder="Select leave type" id="type" :class="{ 'p-invalid': v$.type.$error }" />
          <small v-if="v$.type.$error" class="p-error">
            <i class="pi pi-exclamation-circle mr-1"></i>
            {{ v$.type.$errors[0].$message }}
          </small>
        </div>
        <div class="field mb-4">
          <label for="reason" class="field-label">Reason</label>
          <InputText v-model="form.reason" id="reason" :class="{ 'p-invalid': v$.reason.$error }" />
          <small v-if="v$.reason.$error" class="p-error">
            <i class="pi pi-exclamation-circle mr-1"></i>
            {{ v$.reason.$errors[0].$message }}
          </small>
        </div>
      </div>

      <template #footer>
        <Button label="Cancel" icon="pi pi-times" class="add-button1" @click="closeDialog" v-tooltip="{
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
        <Button label="Save" icon="pi pi-check" class="add-button" :loading="isSaving" @click="saveRequest" v-tooltip="{
                value: 'Save',
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

    <!-- Toast for notifications -->

  </div>
</template>

<script setup>
import { ref, watch } from 'vue';
import api from '@/api';
import { useVuelidate } from '@vuelidate/core';
import { required, helpers, maxLength } from '@vuelidate/validators';
import { useToast } from 'primevue/usetoast';

const toast = useToast();

const props = defineProps({
  visible: Boolean,
  isEdit: Boolean,
  request: Object,
  showDelete: Boolean,
  deleteRequest: Object
});

const emit = defineEmits(['update:visible', 'save', 'update:showDelete', 'delete']);

const leaveTypes = ref([
  { label: 'Vacation', value: 'Vacation' },
  { label: 'Sick Leave', value: 'Sick Leave' },
  { label: 'Personal', value: 'Personal' },
  { label: 'Bereavement', value: 'Bereavement' },
  { label: 'Other', value: 'Other' }
]);

const form = ref({
  startDate: null,
  endDate: null,
  type: null,
  reason: ''
});

const isSaving = ref(false);

// Vuelidate rules
const rules = {
  startDate: {
    required: helpers.withMessage('Start date is required.', required)
  },
  endDate: {
    required: helpers.withMessage('End date is required.', required),
    validDate: helpers.withMessage(
      'End date must be on or after start date.',
      (value, { startDate }) => !startDate || !value || value >= startDate
    )
  },
  type: {
    required: helpers.withMessage('Leave type is required.', required),
    maxLength: helpers.withMessage('Type must be 50 characters or less.', maxLength(50))
  },
  reason: {
    required: helpers.withMessage('Reason is required.', required),
    maxLength: helpers.withMessage('Reason must be 500 characters or less.', maxLength(500))
  }
};

const v$ = useVuelidate(rules, form);

watch(() => props.request, (newRequest) => {
  console.log('Request prop changed:', newRequest);
  if (newRequest) {
    form.value = {
      status: newRequest.status,
      startDate: newRequest.startDate ? new Date(newRequest.startDate) : null,
      endDate: newRequest.endDate ? new Date(newRequest.endDate) : null,
      type: newRequest.type,
      reason: newRequest.reason
    };
  } else {
    form.value = { startDate: null, endDate: null, type: null, reason: '' };
  }
  v$.value.$reset();
}, { immediate: true });

watch(() => props.visible, (newVisible) => {
  console.log('Add/Edit dialog visibility changed:', newVisible);
  if (!newVisible) {
    closeDialog();
  }
});

watch(() => props.showDelete, (newShowDelete) => {
  console.log('Delete dialog visibility changed:', newShowDelete);
});

const closeDialog = () => {
  console.log('Emitting close add/edit dialog');
  emit('update:visible', false);
  if (!props.isEdit) {
    form.value = { startDate: null, endDate: null, type: null, reason: '' };
    v$.value.$reset();
  }
};

const saveRequest = async () => {
  console.log('Saving request with payload:', form.value);
  await v$.value.$validate();

  if (v$.value.$invalid) {
    console.log('Validation errors:', v$.value.$errors);
    toast.add({
      severity: 'error',
      summary: 'Validation Error',
      detail: 'Please correct the errors in the form.',
      life: 3000
    });
    return;
  }

  try {
    isSaving.value = true;
    const userId = localStorage.getItem('Id');
    if (!userId) {
      throw new Error('User ID is not available. Please ensure you are logged in.');
    }

    // Ensure token is set for this request
    const token = localStorage.getItem('accessToken');
    if (!token) {
      throw new Error('Authentication token is missing. Please log in again.');
    }

    // Format date as ISO 8601 for consistency with backend UTC conversion
    const formatDate = (date) => {
      if (!date) return null;
      return date.toISOString(); // e.g., "2025-07-01T00:00:00.000Z"
    };

    // Payload for CreateLeaveRequestDto
    const payload = {
      userId: userId, // Matches CreateLeaveRequestDto
      startDate: formatDate(form.value.startDate),
      endDate: formatDate(form.value.endDate),
      type: form.value.type,
      reason: form.value.reason
    };

    // Alternative payload if backend sets userId from token
    // const payload = {
    //   startDate: formatDate(form.value.startDate),
    //   endDate: formatDate(form.value.endDate),
    //   type: form.value.type,
    //   reason: form.value.reason
    // };

    let response;
    if (props.isEdit) {
      console.log('Editing request:', props.request?.id);
      // Payload for UpdateLeaveRequestDto
      const updatePayload = {
        startDate: formatDate(form.value.startDate),
        endDate: formatDate(form.value.endDate),
        type: form.value.type,
        reason: form.value.reason
      };
      response = await api.put(`/api/LeaveRequests/${props.request?.id}`, updatePayload);
    } else {
      console.log('Creating new request');
      response = await api.post('/api/LeaveRequests', payload);
    }

    console.log('Save response:', response.data);
    toast.add({
      severity: 'success',
      summary: 'Success',
      detail: props.isEdit ? 'Leave request updated successfully.' : 'Leave request created successfully.',
      life: 3000
    });
    emit('save', response.data);
    closeDialog();
  } catch (error) {
    console.error('Error saving leave request:', error);
    let errorMessage = 'An error occurred while saving the leave request.';
    if (error.response) {
      if (error.response.status === 401) {
        errorMessage = 'Unauthorized: Please log in again.';
        // Optionally redirect to login
        // localStorage.removeItem('accessToken');
        // localStorage.removeItem('Id');
        // window.location.href = '/login';
      } else if (error.response.status === 400) {
        errorMessage = error.response.data.errors
          ? Object.values(error.response.data.errors).flat().join(' ')
          : 'Invalid input. Please check your data.';
      }
    } else if (error.request) {
      errorMessage = 'No response from server. Please check your network connection.';
    }
    toast.add({
      severity: 'error',
      summary: 'Error',
      detail: errorMessage,
      life: 3000
    });
  } finally {
    isSaving.value = false;
  }
};

const onClose = (value) => {
  console.log('Add/Edit dialog close event:', value);
  emit('update:visible', value);
  closeDialog();
};
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