<template>
  <div>
    <div class="header-container">
      <div class="flex justify-content-between align-items-center mb-2">
        <h2>Leave Requests</h2>
      </div>
    </div>

    <InputText v-model="searchQuery" placeholder="Search leave requests..." class="search-input" />

    <div class="card">
      <DataTable :value="filteredRequests" paginator :rows="10" tableStyle="min-width: 50rem" :showGridlines="true">
        <!-- User Name Column -->
        <Column field="username" header="Requested By" sortable style="max-width: 8rem;"/>
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
        <Column field="numberOfDays" header="Days" sortable style="max-width: 8rem;" />
        <Column field="status" header="Status" sortable style="max-width: 8rem;">
          <template #body="{ data }">
            {{ statusDisplay(data.status) }}
          </template>
        </Column>
        <Column field="type" header="Type" sortable style="max-width: 8rem;"/>
        <Column field="reason" header="Reason" sortable style="max-width: 8rem;" />
        <Column :exportable="false" style="max-width: 6rem" header="Actions">
          <template #body="slotProps">
            <Button icon="pi pi-check-circle" class="add-button" @click="showConfirmDialog(slotProps.data, 'accept')" /> &nbsp;
            <Button icon="pi pi-times-circle" class="add-button1" @click="showConfirmDialog(slotProps.data, 'deny')" />
          </template>
        </Column>
        <template #empty>
          <div class="text-center text-muted">No leave requests found</div>
        </template>
      </DataTable>
    </div>

    <LeaveRequestDialog 
      :visible="dialogVisible" 
      :userId="userId"
      @update:visible="dialogVisible = $event"
      @save="handleSave"
    />

    <!-- Confirmation Dialog -->
    <Dialog 
      v-model:visible="confirmDialogVisible" 
      header="Confirm Action" 
      :modal="true" 
      :closable="false"
      :style="{ width: '400px' }"
    >
      <div class="confirmation-content">
        <p>Are you sure you want to {{ confirmAction === 'accept' ? 'approve' : 'deny' }} this leave request?</p>
      </div>
      <template #footer>
        <Button label="Cancel" icon="pi pi-times" class="add-button1" @click="confirmDialogVisible = false" />
        <Button class="add-button"
          :label="confirmAction === 'accept' ? 'Approve' : 'Deny'" 
          :icon="confirmAction === 'accept' ? 'pi pi-check' : 'pi pi-times'" 
          :class="confirmAction === 'accept' ? 'p-button-success' : 'p-button-danger'" 
          @click="confirmActionHandler" 
        />
      </template>
    </Dialog>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import api from '@/api'
import Button from 'primevue/button'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import InputText from 'primevue/inputtext'
import Dialog from 'primevue/dialog'
import LeaveRequestDialog from './Componant/LeaveRequestDialog.vue'

const leaveRequests = ref([])
const dialogVisible = ref(false)
const searchQuery = ref('')
const userId = ref(null)
const confirmDialogVisible = ref(false)
const confirmAction = ref('')
const selectedRequest = ref(null)

const filteredRequests = computed(() => {
  if (!searchQuery.value) return leaveRequests.value;
  const query = searchQuery.value.toLowerCase();
  return leaveRequests.value.filter(request =>
    (request.type && request.type.toLowerCase().includes(query)) ||
    (request.reason && request.reason.toLowerCase().includes(query)) ||
    (request.status && request.status.toLowerCase().includes(query)) ||
    (request.username && request.username.toLowerCase().includes(query))
  );
});

const statusDisplay = (status) => {
  switch (status) {
    case 0: return 'Pending'
    case 1: return 'Accepted'
    case 2: return 'Denied'
    default: return 'Unknown'
  }
}

const decodeJwt = (token) => {
  try {
    const base64Url = token.split('.')[1]
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
    const jsonPayload = decodeURIComponent(atob(base64).split('').map(c => {
      return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)
    }).join(''))
    return JSON.parse(jsonPayload)
  } catch (error) {
    console.error('Error decoding JWT:', error)
    return null
  }
}

const fetchLeaveRequests = async () => {
  try {
    if (!userId.value) {
      throw new Error('User ID is not available')
    }
    console.log(`Fetching all leave requests`)
    const { data } = await api.get(`/api/LeaveRequests`)
    console.log('Raw leave requests data:', data)
    leaveRequests.value = data
    console.log('Processed leave requests:', leaveRequests.value)
  } catch (error) {
    console.error('Error fetching leave requests:', error)
  }
}

const formatDate = (dateString) => {
  if (!dateString) return ''
  const options = { year: 'numeric', month: 'short', day: 'numeric' }
  return new Date(dateString).toLocaleDateString(undefined, options)
}

const openAddDialog = () => {
  console.log('Opening add dialog')
  dialogVisible.value = true
}

const closeDialog = () => {
  console.log('Closing dialog')
  dialogVisible.value = false
}

const handleSave = async () => {
  console.log('Handling save from dialog')
  await fetchLeaveRequests()
  closeDialog()
}

const showConfirmDialog = (request, action) => {
  selectedRequest.value = request
  confirmAction.value = action
  confirmDialogVisible.value = true
}

const confirmActionHandler = async () => {
  if (confirmAction.value === 'accept') {
    await acceptRequest(selectedRequest.value)
  } else if (confirmAction.value === 'deny') {
    await denyRequest(selectedRequest.value)
  }
  confirmDialogVisible.value = false
  selectedRequest.value = null
  confirmAction.value = ''
}

const acceptRequest = async (request) => {
  console.log('Accepting request:', request)
  try {
    await api.put(`/api/LeaveRequests/${request.id}`, { status: 1 })
    await fetchLeaveRequests()
  } catch (error) {
    console.error('Error accepting leave request:', error)
  }
}

const denyRequest = async (request) => {
  console.log('Denying request:', request)
  try {
    await api.put(`/api/LeaveRequests/${request.id}`, { status: 2 })
    await fetchLeaveRequests()
  } catch (error) {
    console.error('Error denying leave request:', error)
  }
}

onMounted(async () => {
  console.log('ListDemandeEmploye mounted')
  const accessToken = localStorage.getItem('accessToken')
  if (accessToken) {
    console.log('Access token found:', accessToken)
    const decoded = decodeJwt(accessToken)
    console.log('Decoded JWT:', decoded)
    if (decoded && decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']) {
      userId.value = decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']
      console.log('Decoded userId:', userId.value)
      await fetchLeaveRequests()
    } else {
      console.error('User ID not found in token')
    }
  } else {
    console.error('Access token not found')
  }
})
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

.confirmation-content {
  padding: 1rem;
}
</style>