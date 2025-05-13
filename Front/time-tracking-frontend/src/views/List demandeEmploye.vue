<template>
    <div>
        <div class="header-container">
            <div class="flex justify-content-between align-items-center mb-2">
                <h2>Leave Requests</h2>
                <Button label="Add" icon="pi pi-plus" class="add-button" @click="openAddDialog" />
            </div>
        </div>

        <InputText v-model="searchQuery" placeholder="Search leave requests..." class="search-input" />

        <div class="card">
            <DataTable :value="filteredRequests" paginator :rows="10" tableStyle="min-width: 50rem"
                :showGridlines="true">
                <Column field="startDate" header="Start Date" sortable>
                    <template #body="{ data }">
                        {{ formatDate(data.startDate) }}
                    </template>
                </Column>
                <Column field="endDate" header="End Date" sortable>
                    <template #body="{ data }">
                        {{ formatDate(data.endDate) }}
                    </template>
                </Column>
                <Column field="numberOfDays" header="Days" sortable />
                <Column field="status" header="Status" sortable />
                <Column field="type" header="Type" sortable />
                <Column field="reason" header="Reason" sortable />
                <Column :exportable="false" style="min-width: 12rem" header="Actions">
                    <template #body="slotProps">
                        <Button icon="pi pi-pencil" class="p-button-warning p-button-sm mr-2"
                            @click="openEditDialog(slotProps.data)" />
                        <Button icon="pi pi-trash" class="p-button-danger p-button-sm"
                            @click="confirmDelete(slotProps.data)" />
                    </template>
                </Column>
                <template #empty>
                    <div class="text-center text-muted">No leave requests found</div>
                </template>
            </DataTable>
        </div>

        <!-- Add/Edit Dialog -->
        <Dialog v-model:visible="dialogVisible" modal :header="isEdit ? 'Edit Leave Request' : 'Add Leave Request'"
            class="w-[30rem]">
            <div class="p-fluid">
                <div class="field">
                    <label for="startDate">Start Date</label>
                    <Calendar v-model="form.startDate" id="startDate" dateFormat="yy-mm-dd" showIcon />
                </div>
                <div class="field">
                    <label for="endDate">End Date</label>
                    <Calendar v-model="form.endDate" id="endDate" dateFormat="yy-mm-dd" showIcon />
                </div>
                <div class="field">
                    <label for="type">Type</label>
                    <Dropdown v-model="form.type" :options="leaveTypes" optionLabel="label" optionValue="value"
                        placeholder="Select leave type" id="type" />
                </div>
                <div class="field">
                    <label for="reason">Reason</label>
                    <InputText v-model="form.reason" id="reason" />
                </div>
            </div>

            <template #footer>
                <Button label="Cancel" icon="pi pi-times" class="p-button-text" @click="closeDialog" />
                <Button label="Save" icon="pi pi-check" @click="saveRequest" />
            </template>
        </Dialog>

        <!-- Delete Confirmation Dialog -->
        <Dialog v-model:visible="deleteDialogVisible" modal header="Confirm Delete" :style="{ width: '25rem' }">
            <span>Are you sure you want to delete this leave request?</span>
            <template #footer>
                <Button label="No" icon="pi pi-times" class="p-button-text" @click="deleteDialogVisible = false" />
                <Button label="Yes" icon="pi pi-check" class="p-button-danger" @click="deleteRequest" />
            </template>
        </Dialog>
    </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import api from '@/api'

const leaveRequests = ref([])
const dialogVisible = ref(false)
const deleteDialogVisible = ref(false)
const isEdit = ref(false)
const currentId = ref(null)
const searchQuery = ref('')
const leaveToDelete = ref(null)
const userId = ref(null) // Store the current user's ID temporarily

const leaveTypes = ref([
    { label: 'Vacation', value: 'Vacation' },
    { label: 'Sick Leave', value: 'Sick Leave' },
    { label: 'Personal', value: 'Personal' },
    { label: 'Bereavement', value: 'Bereavement' },
    { label: 'Other', value: 'Other' }
])

const form = ref({
    startDate: null,
    endDate: null,
    type: null,
    reason: ''
})

const filteredRequests = computed(() => {
    if (!searchQuery.value) return leaveRequests.value
    const query = searchQuery.value.toLowerCase()
    return leaveRequests.value.filter(request =>
        (request.type && request.type.toLowerCase().includes(query)) ||
        (request.reason && request.reason.toLowerCase().includes(query)) ||
        (request.status && request.status.toLowerCase().includes(query))
    )
})

onMounted(async () => {
    // Fetch the user ID from stored login response (adjust based on your setup)
    // Example: From Vuex store, Pinia, or auth service
    // Replace with your actual method to get the user ID
    // Example with Vuex: import { useStore } from 'vuex'; const store = useStore(); userId.value = store.state.auth.userId
    // Example with Pinia: import { useAuthStore } from '@/stores/auth'; const authStore = useAuthStore(); userId.value = authStore.userId
    userId.value = 'your-user-id' // Placeholder: Replace with actual retrieval (e.g., authStore.userId)
    
    if (!userId.value) {
        console.error('User ID not found. Please ensure you are logged in.')
    }
    await fetchLeaveRequests()
})

const fetchLeaveRequests = async () => {
    try {
        const { data } = await api.get('/api/LeaveRequests')
        leaveRequests.value = data
    } catch (error) {
        console.error('Error fetching leave requests:', error)
    }
}

const formatDate = (dateString) => {
    if (!dateString) return ''
    const options = { year: 'numeric', month: 'short', day: 'numeric' }
    return new Date(dateString).toLocaleDateString(undefined, options)
}

const openEditDialog = (request) => {
    form.value = {
        startDate: new Date(request.startDate),
        endDate: new Date(request.endDate),
        type: request.type,
        reason: request.reason
    }
    currentId.value = request.id
    isEdit.value = true
    dialogVisible.value = true
}

const openAddDialog = () => {
    isEdit.value = false
    form.value = { startDate: null, endDate: null, type: null, reason: '' }
    dialogVisible.value = true
}

const closeDialog = () => {
    dialogVisible.value = false
}

const saveRequest = async () => {
    try {
        if (!userId.value) {
            throw new Error('User ID is not available. Please ensure you are logged in.')
        }
        const payload = {
            startDate: form.value.startDate.toISOString().split('T')[0],
            endDate: form.value.endDate.toISOString().split('T')[0],
            type: form.value.type,
            reason: form.value.reason,
            L_USER_ID: userId.value // Include user ID from login response
        }

        if (isEdit.value) {
            await api.put(`/api/LeaveRequests/${currentId.value}`, payload)
        } else {
            await api.post('/api/LeaveRequests', payload)
        }
        await fetchLeaveRequests()
        closeDialog()
    } catch (error) {
        console.error('Error saving leave request:', error)
    }
}

const confirmDelete = (request) => {
    leaveToDelete.value = request
    deleteDialogVisible.value = true
}

const deleteRequest = async () => {
    try {
        await api.delete(`/api/LeaveRequests/${leaveToDelete.value.id}`)
        await fetchLeaveRequests()
    } catch (error) {
        console.error('Error deleting leave request:', error)
    } finally {
        deleteDialogVisible.value = false
        leaveToDelete.value = null
    }
}
</script>

<style scoped>
.header-container {
    margin-bottom: 1.5rem;
}

.flex {
    display: flex;
}

.justify-content-between {
    justify-content: space-between;
}

.align-items-center {
    align-items: center;
}

.search-input {
    border: 1px solid #ced4da !important;
    border-radius: 4px !important;
    width: 300px;
    margin-bottom: 1rem;
}

h2 {
    font-size: 2rem;
    margin: 0;
}

.add-button {
    align-items: right;
}

.card {
    margin-top: 1rem;
}

.text-center {
    text-align: center;
}

.text-muted {
    color: #6c757d;
}

.field {
    margin-bottom: 1.5rem;
}
</style>