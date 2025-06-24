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
                <Button label="Cancel" icon="pi pi-times" class="add-button1" @click="closeDialog" />
                <Button label="Save" icon="pi pi-check" class="add-button" :loading="isSaving" @click="saveRequest" />
            </template>
        </Dialog>
    </div>
</template>

<script setup>
import { ref, watch } from 'vue';
import api from '@/api';
import { useVuelidate } from '@vuelidate/core';
import { required, helpers } from '@vuelidate/validators';

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
        required: helpers.withMessage('Leave type is required.', required)
    },
    reason: { 
        required: helpers.withMessage('Reason is required.', required)
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
        return;
    }

    try {
        isSaving.value = true;
        const userId = localStorage.getItem('Id'); // Fetch userId from localStorage
        if (!userId) {
            throw new Error('User ID is not available. Please ensure you are logged in.');
        }

        // Helper function to format date as YYYY-MM-DD in local timezone
        const formatDate = (date) => {
            if (!date) return null;
            const year = date.getFullYear();
            const month = String(date.getMonth() + 1).padStart(2, '0');
            const day = String(date.getDate()).padStart(2, '0');
            return `${year}-${month}-${day}`;
        };

        const payload = {
            startDate: formatDate(form.value.startDate),
            endDate: formatDate(form.value.endDate),
            type: form.value.type,
            reason: form.value.reason,
            UserId: userId // Use userId directly from localStorage
        };

        let response;
        if (props.isEdit) {
            console.log('Editing request:', props.request?.id);
            response = await api.put(`/api/LeaveRequests/${props.request?.id}`, payload);
        } else {
            console.log('Creating new request');
            console.log('Payload:', payload);
            response = await api.post('/api/LeaveRequests', payload);
        }
        console.log('Save response:', response.data);
        emit('save', response.data);
        closeDialog();
    } catch (error) {
        console.error('Error saving leave request:', error);
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
.leave-request-dialog {
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

:deep(.p-inputtext), :deep(.p-calendar), :deep(.p-dropdown) {
    border-radius: 6px;
    border: 1px solid #ced4da;
    transition: border-color 0.2s, box-shadow 0.2s;
}

:deep(.p-inputtext:focus), :deep(.p-calendar:focus), :deep(.p-dropdown:focus) {
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

.custom-button {
    border-radius: 6px;
    padding: 0.5rem 1rem;
    font-weight: 500;
    transition: background-color 0.2s, transform 0.1s;
}

.custom-button:hover {
    transform: translateY(-1px);
}

:deep(.p-button-primary) {
    background: #6366f1;
    border-color: #6366f1;
}

:deep(.p-button-primary:hover) {
    background: #4f46e5;
    border-color: #4f46e5;
}
</style>
