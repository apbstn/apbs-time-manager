<template>
    <Dialog :visible="visible" modal :header="isEdit ? 'Edit Team' : 'Add Team'"
        class="team-dialog" :draggable="false" :style="{ maxWidth: '550px' }" @update:visible="onClose">
        <div class="p-fluid form-container">
            <div class="field mb-4">
                <label for="name" class="field-label">Team Name</label>
                <InputText v-model="form.name" id="name" :class="{ 'p-invalid': v$.name.$error }" />
                <small v-if="v$.name.$error" class="p-error">
                    <i class="pi pi-exclamation-circle mr-1"></i>
                    {{ v$.name.$errors[0].$message }}
                </small>
            </div>
            <div class="field mb-4">
                <label for="description" class="field-label">Description</label>
                <InputText v-model="form.description" id="description" :class="{ 'p-invalid': v$.description.$error }" />
                <small v-if="v$.description.$error" class="p-error">
                    <i class="pi pi-exclamation-circle mr-1"></i>
                    {{ v$.description.$errors[0].$message }}
                </small>
            </div>
        </div>

        <template #footer>
            <Button label="Cancel" icon="pi pi-times" class="add-button1" @click="closeDialog" />
            <Button label="Save" icon="pi pi-check" class="add-button" :loading="isSaving" @click="saveTeam" />
        </template>
    </Dialog>
</template>

<script setup>
import { ref, watch } from 'vue'
import { useVuelidate } from '@vuelidate/core'
import { required, helpers } from '@vuelidate/validators'
import api from '@/api'
import Dialog from 'primevue/dialog'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'

const props = defineProps({
    visible: Boolean,
    isEdit: Boolean,
    team: Object,
    currentId: [String, Number]
})

const emit = defineEmits(['update:visible', 'refresh', 'close'])

const form = ref({
    name: '',
    description: ''
})

const isSaving = ref(false)

// Vuelidate rules
const rules = {
    name: { 
        required: helpers.withMessage('Team name is required.', required)
    },
    description: { 
        required: helpers.withMessage('Description is required.', required)
    }
}

const v$ = useVuelidate(rules, form)

watch(() => props.team, (newTeam) => {
    console.log('Team prop changed:', newTeam)
    if (newTeam) {
        form.value = {
            name: newTeam.name || '',
            description: newTeam.description || ''
        }
    } else {
        form.value = { name: '', description: '' }
    }
    v$.value.$reset()
}, { immediate: true })

watch(() => props.visible, (newVisible) => {
    console.log('Add/Edit dialog visibility changed:', newVisible)
    if (!newVisible) {
        closeDialog()
    }
})

const closeDialog = () => {
    console.log('Emitting close dialog')
    emit('update:visible', false)
    if (!props.isEdit) {
        form.value = { name: '', description: '' }
        v$.value.$reset()
    }
}

const saveTeam = async () => {
    console.log('Saving team with payload:', form.value)
    await v$.value.$validate()
    
    if (v$.value.$invalid) {
        console.log('Validation errors:', v$.value.$errors)
        return
    }

    try {
        isSaving.value = true
        if (props.isEdit) {
            console.log('Editing team:', props.currentId, form.value)
            await api.put(`/api/teams/${props.currentId}`, form.value)
        } else {
            console.log('Creating new team:', form.value)
            await api.post(`/api/teams`, form.value)
        }
        emit('refresh')
        closeDialog()
    } catch (error) {
        console.error('Error saving team:', error)
    } finally {
        isSaving.value = false
    }
}

const onClose = (value) => {
    console.log('Add/Edit dialog close event:', value)
    emit('update:visible', value)
    closeDialog()
}
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
.team-dialog {
    width: 90%;
    max-width: 550px;
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