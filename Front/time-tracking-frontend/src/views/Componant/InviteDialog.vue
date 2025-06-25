<template>
    <div>
        <!-- Add Invitation Dialog -->
        <Dialog :visible="visible" modal header="Add Invitation" class="stunning-dialog" :draggable="false" @update:visible="onClose">
            <div class="dialog-content">
                <p class="dialog-subtitle">Enter invitation details below</p>
                <Divider class="dialog-divider" />
                <Message v-if="validationError" severity="error" :closable="true" class="error-message" @close="validationError = ''">
                    {{ validationError }}
                </Message>
                <div class="p-field">
                    <label for="email" class="field-label">Email</label>
                    <InputText v-model="form.email" id="email" :class="{ 'p-invalid': v$.email.$error || validationError }" class="stunning-input" />
                    <small v-if="v$.email.$error" class="p-error">
                        <i class="pi pi-exclamation-circle mr-1"></i>
                        {{ v$.email.$errors[0].$message }}
                    </small>
                </div>
                <br />
                <div class="p-field">
                    <label for="username" class="field-label">Username</label>
                    <InputText v-model="form.username" id="username" :class="{ 'p-invalid': v$.username.$error || validationError }" class="stunning-input" />
                    <small v-if="v$.username.$error" class="p-error">
                        <i class="pi pi-exclamation-circle mr-1"></i>
                        {{ v$.username.$errors[0].$message }}
                    </small>
                </div>
                <br />
                <div class="p-field">
                    <label for="phoneNumber" class="field-label">Phone Number</label>
                    <InputText v-model="form.phoneNumber" id="phoneNumber" :class="{ 'p-invalid': v$.phoneNumber.$error || validationError }" class="stunning-input" />
                    <small v-if="v$.phoneNumber.$error" class="p-error">
                        <i class="pi pi-exclamation-circle mr-1"></i>
                        {{ v$.phoneNumber.$errors[0].$message }}
                    </small>
                </div>
            </div>
            <Divider class="dialog-divider" />
            <div class="footer-buttons">
                <Button label="Cancel" icon="pi pi-times" @click="closeDialog" class="p-button-text stunning-button stunning-button-cancel" />
                <Button label="Send" icon="pi pi-check" :loading="isSending" @click="sendInvite" class="stunning-button stunning-button-save" :disabled="!isSavable || isSending" />
            </div>
        </Dialog>
    </div>
</template>

<script setup>
import { ref, watch, computed } from 'vue'
import api from '@/api'
import { useVuelidate } from '@vuelidate/core'
import { required, helpers } from '@vuelidate/validators'
import InputText from 'primevue/inputtext'
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'
import Divider from 'primevue/divider'
import Message from 'primevue/message'

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
const validationError = ref('')

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

// Computed property to enable Send button only when all fields are valid
const isSavable = computed(() => {
    console.log('isSavable check:', { email: form.value.email, username: form.value.username, phoneNumber: form.value.phoneNumber, invalid: v$.value.$invalid })
    return !v$.value.$invalid && form.value.email.trim() && form.value.username.trim() && form.value.phoneNumber.trim()
})

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
    validationError.value = '' // Reset error on close
}

const sendInvite = async () => {
    console.log('Sending invite with payload:', form.value)
    await v$.value.$validate()
    
    if (v$.value.$invalid) {
        validationError.value = 'Please fix the validation errors.'
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
        validationError.value = 'Failed to send invite. Please try again.'
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
.stunning-dialog {
    width: 90%;
    max-width: 800px; /* Increased from 650px to 800px */
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
    border-top: 1px solid #e5e7eb;
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