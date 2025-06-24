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
      <DataTable :value="filteredRequests" paginator :rows="10" tableStyle="min-width: 50rem" :showGridlines="true">
        <Column field="startDate" header="Start Date" sortable style="max-width: 8rem;">
          <template #body="{ data }">
            {{ formatDate(data.startDate) }}
          </template>
        </Column>
        <Column field="endDate" header="End Date" sortable style="max-width: 8rem;">
          <template #body="{ data }">
            {{ formatDate(data.endDate) }}
          </template>
        </Column>
        <Column field="numberOfDays" header="Days" sortable style="max-width: 8rem;"/>
        <Column field="status" header="Status" sortable style="max-width: 8rem;">
          <template #body="{ data }">
            {{ statusDisplay(data.status) }}
          </template>
        </Column>
        <Column field="type" header="Type" sortable style="max-width: 8rem;"/>
        <Column field="reason" header="Reason" sortable style="max-width: 8rem;"/>
        <Column :exportable="false" style="max-width: 5rem" header="Actions">
          <template #body="slotProps">
            <Button icon="pi pi-pencil" class="add-button"
              :disabled="slotProps.data.status === 1 || slotProps.data.status === 2"
              @click="openEditDialog(slotProps.data)" />  
            <Button icon="pi pi-trash" class="add-button1"
              :disabled="slotProps.data.status === 1 || slotProps.data.status === 2"
              @click="confirmDelete(slotProps.data)" />
          </template>
        </Column>
        <template #empty>
          <div class="text-center text-muted">No leave requests found</div>
        </template>
      </DataTable>
    </div>

    <!-- Add/Edit Dialog -->
    <LeaveRequestDialog 
      :visible="dialogVisible" 
      :isEdit="isEdit" 
      :request="currentRequest" 
      :userId="userId"
      @update:visible="dialogVisible = $event"
      @save="handleSave"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, computed, inject } from 'vue'
import api from '@/api'
import Button from 'primevue/button'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import InputText from 'primevue/inputtext'
import LeaveRequestDialog from './Componant/LeaveRequestDialog.vue'

const leaveRequests = ref([])
const dialogVisible = ref(false)
const isEdit = ref(false)
const currentId = ref(null)
const searchQuery = ref('')
const userId = ref(null)
const currentRequest = ref(null)
const deleteDialogRef = inject('deleteDialog')

const filteredRequests = computed(() => {
    if (!searchQuery.value) return leaveRequests.value
    const query = searchQuery.value.toLowerCase()
    return leaveRequests.value.filter(request =>
        (request.type && request.type.toLowerCase().includes(query)) ||
        (request.reason && request.reason.toLowerCase().includes(query)) ||
        (request.status && request.status.toLowerCase().includes(query))
    )
})

const statusDisplay = (status) => {
    switch (status) {
        case 0: return 'Pending'
        case 1: return 'Approved'
        case 2: return 'Rejected'
        default: return 'Unknown'
    }
}

const getAccountId = async (email) => {
    try {
      console.log("email", email)
        const response = await api.post('/api/UserTenants/get-id-by-email', email)
        console.log('Account ID fetched:', response.data)
        return response.data
    } catch (error) {
        console.error('Error fetching account ID:', error)
        throw error
    }
}

onMounted(async () => {
    console.log('ListDemandeEmploye mounted')
    // Assume email is stored in localStorage or user object
    const userEmail = localStorage.getItem('email') // Replace with your email source
    if (userEmail) {
        try {
            userId.value = await getAccountId(`"${userEmail}"`) // Send as raw string with quotes
            await fetchLeaveRequests()
        } catch (error) {
            console.error('User ID not fetched:', error)
        }
    } else {
        console.error('User email not found')
    }
})

const fetchLeaveRequests = async () => {
    try {
        if (!userId.value) {
            throw new Error('User ID is not available')
        }
        const accessToken = localStorage.getItem('accessToken')
        console.log("bla bla bla", userId.value)
        localStorage.setItem("Id", userId.value) // Retrieve the token
        const { data } = await api.get(`/api/LeaveRequests/user/${userId.value}`, {
            headers: {
                Authorization: `Bearer ${accessToken}` // Add token to Authorization header
            }
        })
        leaveRequests.value = data
        console.log('Leave requests fetched:', data)
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
    console.log('Opening edit dialog for request:', request)
    currentRequest.value = request
    currentId.value = request.id
    isEdit.value = true
    dialogVisible.value = true
}

const openAddDialog = () => {
    console.log('Opening add dialog')
    isEdit.value = false
    currentRequest.value = null
    dialogVisible.value = true
}

const closeDialog = () => {
    console.log('Closing dialog')
    dialogVisible.value = false
    currentRequest.value = null
}

const handleSave = async () => {
    console.log('Handling save from dialog')
    await fetchLeaveRequests()
    closeDialog()
}

const confirmDelete = (request) => {
    console.log('Confirming delete for request:', request)
    if (deleteDialogRef?.value?.showDeleteDialog) {
        deleteDialogRef.value.showDeleteDialog({
            item: request,
            type: 'leave request',
            name: request.type || `ID ${request.id}`,
            onConfirm: async () => {
                try {
                    console.log('Deleting request:', request)
                    await api.delete(`/api/LeaveRequests/${request.id}`)
                    await fetchLeaveRequests()
                } catch (error) {
                    console.error('Error deleting leave request:', error)
                }
            }
        })
    } else {
        console.error('DeleteDialog not injected or showDeleteDialog not exposed')
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