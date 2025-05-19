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
        <Column field="username" header="Requested By" sortable />
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
        <Column field="status" header="Status" sortable>
          <template #body="{ data }">
            {{ statusDisplay(data.status) }}
          </template>
        </Column>
        <Column field="type" header="Type" sortable />
        <Column field="reason" header="Reason" sortable />
        <Column :exportable="false" style="min-width: 12rem" header="Actions">
          <template #body="slotProps">
            <Button icon="pi pi-check-circle" class="p-button-success p-button-sm mr-2" @click="acceptRequest(slotProps.data)" />
            <Button icon="pi pi-times-circle" class="p-button-warning p-button-sm" @click="denyRequest(slotProps.data)" />
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
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import api from '@/api'
import Button from 'primevue/button'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import InputText from 'primevue/inputtext'
import LeaveRequestDialog from './LeaveRequestDialog.vue'

const leaveRequests = ref([])
const dialogVisible = ref(false)
const searchQuery = ref('')
const userId = ref(null)

const filteredRequests = computed(() => {
  // First, filter by userId to show only the logged-in user's leave requests
  let userRequests = leaveRequests.value.filter(request => request.userId === userId.value)
  
  // Then apply the search filter
  if (!searchQuery.value) return userRequests
  const query = searchQuery.value.toLowerCase()
  return userRequests.filter(request =>
    (request.type && request.type.toLowerCase().includes(query)) ||
    (request.reason && request.reason.toLowerCase().includes(query)) ||
    (request.status && request.status.toLowerCase().includes(query)) ||
    (request.username && request.username.toLowerCase().includes(query))
  )
})

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