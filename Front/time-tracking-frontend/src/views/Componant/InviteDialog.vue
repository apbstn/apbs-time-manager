<template>
    <div>
        <!-- Add Invitation Dialog -->
        <Dialog :visible="visible" modal header="Add Invitation"
            class="invite-dialog" :draggable="false" :style="{ maxWidth: '650px' }" @update:visible="onClose">
            <div class="p-fluid form-container">
                <div class="field mb-4">
                    <label for="email" class="field-label">Email</label>
                    <InputText v-model="form.email" id="email" :class="{ 'p-invalid': v$.email.$error }" />
                    <small v-if="v$.email.$error" class="p-error">
                        <i class="pi pi-exclamation-circle mr-1"></i>
                        {{ v$.email.$errors[0].$message }}
                    </small>
                </div>
                <div class="field mb-4">
                    <label for="username" class="field-label">Username</label>
                    <InputText v-model="form.username" id="username" :class="{ 'p-invalid': v$.username.$error }" />
                    <small v-if="v$.username.$error" class="p-error">
                        <i class="pi pi-exclamation-circle mr-1"></i>
                        {{ v$.username.$errors[0].$message }}
                    </small>
                </div>
                <div class="field mb-4">
                    <label for="phoneNumber" class="field-label">Phone Number</label>
                    <InputText v-model="form.phoneNumber" id="phoneNumber" :class="{ 'p-invalid': v$.phoneNumber.$error }" />
                    <small v-if="v$.phoneNumber.$error" class="p-error">
                        <i class="pi pi-exclamation-circle mr-1"></i>
                        {{ v$.phoneNumber.$errors[0].$message }}
                    </small>
                </div>
            </div>

            <template #footer>
                <Button label="Cancel" icon="pi pi-times" class="p-button-text p-button-secondary custom-button" @click="closeDialog" />
                <Button label="Send" icon="pi pi-check" class="p-button-primary custom-button" :loading="isSending" @click="sendInvite" />
            </template>
        </Dialog>
    </div>
</template>

<script setup>
import { ref, watch } from 'vue'
import api from '@/api'
import { useVuelidate } from '@vuelidate/core'
import { required, helpers } from '@vuelidate/validators'
import InputText from 'primevue/inputtext'
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'

const props = defineProps({
    visible: Boolean,
    userId: String
})

const emit = defineEmits(['update:visible', 'save'])

const form = ref({
    email: '',
    username: '',
    phoneNumber: ''
})

const isSending = ref(false)

// Vuelidate rules
const rules = {
    email: { 
        required: helpers.withMessage('Email is required.', required)
    },
    username: { 
        required: helpers.withMessage('Username is required.', required)
    },
    phoneNumber: { 
        required: helpers.withMessage('Phone number is required.', required)
    }
}

const v$ = useVuelidate(rules, form)

watch(() => props.visible, (newVisible) => {
    console.log('Invite dialog visibility changed:', newVisible)
    if (!newVisible) {
        closeDialog()
    }
})

const closeDialog = () => {
    console.log('Emitting close invite dialog')
    emit('update:visible', false)
    form.value = { email: '', username: '', phoneNumber: '' }
    v$.value.$reset()
}

const sendInvite = async () => {
    console.log('Sending invite with payload:', form.value)
    await v$.value.$validate()
    
    if (v$.value.$invalid) {
        console.log('Validation errors:', v$.value.$errors)
        return
    }

    try {
        isSending.value = true
        if (!props.userId) {
            throw new Error('User ID is not available. Please ensure you are logged in.')
        }

        const payload = {
            email: form.value.email,
            username: form.value.username,
            phoneNumber: form.value.phoneNumber
        }

        const response = await api.post('http://localhost:58167/api/auth/invite', payload)
        console.log('Invite sent:', response.data)
        emit('save', { ...payload, status: 1, id: response.data.id || Date.now() })
        closeDialog()
    } catch (error) {
        console.error('Error sending invite:', error)
    } finally {
        isSending.value = false
    }
}

const onClose = (value) => {
    console.log('Invite dialog close event:', value)
    emit('update:visible', value)
    closeDialog()
}
</script>

<style scoped>
.invite-dialog {
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

:deep(.p-inputtext) {
    border-radius: 6px;
    border: 1px solid #ced4da;
    transition: border-color 0.2s, box-shadow 0.2s;
}

:deep(.p-inputtext:focus) {
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

:deep(.p-button-secondary) {
    color: #6b7280;
}

:deep(.p-button-secondary:hover) {
    background: #f3f4f6;
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