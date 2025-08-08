<template>
  <div>
    <div class="header-container">
      <div class="flex justify-content-between align-items-center mb-2">
        <h2 style="font-size: 22px; color: #6B7280;">User Accounts</h2>
      </div>
    </div>

    <!-- Error Message -->
    <Message v-if="error" severity="error" :closable="true" class="error-message" @close="error = ''">
      {{ error }}
    </Message>

    <!-- Success Message -->
    <Message v-if="successMessage" severity="success" :closable="true" class="success-message" @close="successMessage = null">
      {{ successMessage }}
    </Message>

    <InputText v-model="searchQuery" placeholder="Search users..." class="search-input" />

    <div class="card">
      <DataTable :value="filteredUsers" paginator :rows="10" tableStyle="min-width: 50rem" :showGridlines="true">
        <Column field="username" header="Username" sortable style="width: 25%;">
          <template #body="slotProps">
            <span class="username-link" @click="showEmailDialog(slotProps.data)" v-tooltip="{
              value: 'Monitor Employee Work Hours',
              pt: {
                arrow: { style: { borderBottomColor: '#000000' } },
                text: '!bg-black !text-white !font-medium'
              }
            }">
              {{ slotProps.data.username }}
            </span>
          </template>
        </Column>
        <Column field="email" header="Email" sortable style="width: 25%;" />
        <Column field="phoneNumber" header="Phone Number" sortable style="width: 25%;" />
        <Column field="teamId" header="Teams" sortable style="width: 25%;">
          <template #body="slotProps">
            {{ getTeamName(slotProps.data.teamId) || 'No team' }}
          </template>
        </Column>
        <Column :exportable="false" style="width: 1%;" header="Actions">
          <template #body="slotProps">
            <div class="actions-container">
              <Button icon="pi pi-user-plus" class="add-button" :disabled="availableTeams.length === 0" v-tooltip="{
                value: availableTeams.length === 0 ? 'No teams available' : 'Add Employee to a team',
                pt: {
                  arrow: { style: { borderBottomColor: '#000000' } },
                  text: '!bg-black !text-white !font-medium'
                }
              }" @click="openAddToTeamDialog(slotProps.data)" />
              <Button icon="pi pi-minus" class="removeteam-button" v-tooltip="{
                value: 'Remove the User from the team',
                pt: {
                  arrow: { style: { borderBottomColor: '#000000' } },
                  text: '!bg-black !text-white !font-medium'
                }
              }" @click="showRemoveDialog(slotProps.data)" />
              <Button icon="pi pi-times" class="add-button1" v-tooltip="{
                value: 'Fire User',
                pt: {
                  arrow: { style: { borderBottomColor: '#000000' } },
                  text: '!bg-black !text-white !font-medium'
                }
              }" @click="confirmDelete(slotProps.data)" />
              <Button icon="pi pi-plus-circle" class="allocate-button" v-tooltip="{
                value: 'Allocate Leave Balance',
                pt: {
                  arrow: { style: { borderBottomColor: '#000000' } },
                  text: '!bg-black !text-white !font-medium'
                }
              }" @click="showAllocateDialog(slotProps.data)" />
            </div>
          </template>
        </Column>
        <template #empty>
          <div class="text-center text-muted">No users found</div>
        </template>
      </DataTable>
    </div>

    <!-- Email Display Dialog -->
    <Dialog :visible="emailDialogVisible" @update:visible="emailDialogVisible = $event"
      header="Employee Details" :modal="true" class="p-fluid stunning-dialog" :draggable="false" :style="{ width: '650px' }">
      <Divider class="dialog-divider" />
      <div class="dialog-content">
        <p class="dialog-subtitle">Hours Worked Today : {{ hhours || 'No track for today' }}</p>
        <p class="dialog-subtitle">Hours Worked This Week : {{ hhhours || 'No track for this Week' }}</p>
        <p class="dialog-subtitle">Hours Worked This Month : {{ hhhhours || 'No track for this Month' }}</p>
      </div>
      <Divider class="dialog-divider" />
      <div class="footer-buttons">
        <Button label="Close" icon="pi pi-times" @click="emailDialogVisible = false"
          class="p-button-text stunning-button stunning-button-cancel" v-tooltip="{
            value: 'Close',
            pt: {
              arrow: { style: { borderBottomColor: '#000000' } },
              text: '!bg-black !text-white !font-medium'
            }
          }"/>
      </div>
    </Dialog>

    <!-- Add/Edit Dialog -->
    <UserDialog :visible="dialogVisible" :isEdit="isEdit" :user="selectedUser" :currentId="currentId"
      :teams="availableTeams" @update:visible="dialogVisible = $event" @refresh="onUserDialogSave" @close="closeDialog" />

    <!-- Allocate Leave Balance Dialog -->
    <AllocateLeaveDialog v-model:visible="allocateDialogVisible" :user="selectedUserForAllocation"
      @allocate="allocateLeaveBalance" />

    <!-- Remove from Team Dialog -->
    <Dialog :visible="removeDialogVisible" @update:visible="removeDialogVisible = $event"
      header="Remove User from Team" :modal="true" class="p-fluid stunning-dialog" :draggable="false" :style="{ width: '650px' }">
      <Divider class="dialog-divider" />
      <div class="dialog-content">
        <p class="dialog-subtitle">Are you sure you want to remove {{ selectedUserForRemoval?.username || selectedUserForRemoval?.email }} from their team?</p>
        <Message v-if="removeError" severity="error" :closable="true" class="error-message" @close="removeError = ''">
          {{ removeError }}
        </Message>
      </div>
      <Divider class="dialog-divider" />
      <div class="footer-buttons">
        <Button label="Cancel" icon="pi pi-times" @click="removeDialogVisible = false"
          class="p-button-text stunning-button stunning-button-cancel" v-tooltip="{
            value: 'Cancel',
            pt: {
              arrow: { style: { borderBottomColor: '#000000' } },
              text: '!bg-black !text-white !font-medium'
            }
          }"/>
        <Button label="Confirm" icon="pi pi-check" @click="removeUserFromTeam"
          class="stunning-button stunning-button-remove" v-tooltip="{
            value: 'Confirm Removal',
            pt: {
              arrow: { style: { borderBottomColor: '#000000' } },
              text: '!bg-black !text-white !font-medium'
            }
          }"/>
      </div>
    </Dialog>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, inject } from 'vue'
import api from '@/api'
import Button from 'primevue/button'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import InputText from 'primevue/inputtext'
import Dialog from 'primevue/dialog'
import Divider from 'primevue/divider'
import Message from 'primevue/message'
import UserDialog from './Componant/UserDialog.vue'
import AllocateLeaveDialog from './Componant/AllocateLeaveDialog.vue'

// Inject the DeleteDialog instance
const deleteDialogRef = inject('deleteDialog')

const users = ref([])
const dialogVisible = ref(false)
const isEdit = ref(false)
const currentId = ref(null)
const searchQuery = ref('')
const selectedUser = ref(null)
const availableTeams = ref([])
const allocateDialogVisible = ref(false)
const selectedUserForAllocation = ref(null)
const removeDialogVisible = ref(false)
const selectedUserForRemoval = ref(null)
const removeError = ref('')
const emailDialogVisible = ref(false)
const selectedUserForEmail = ref(null)
const hhours = ref(null)
const hhhours = ref(null)
const hhhhours = ref(null)
const error = ref('')
const successMessage = ref(null)

const filteredUsers = computed(() => {
  if (!searchQuery.value) return users.value
  const query = searchQuery.value.toLowerCase()
  return users.value.filter(user =>
    user.username.toLowerCase().includes(query) ||
    user.email.toLowerCase().includes(query) ||
    (user.phoneNumber && user.phoneNumber.toLowerCase().includes(query))
  )
})

const fetchUsers = async () => {
  try {
    const response = await api.get('/api/UserTenants/accounts')
    users.value = response.data.map(user => ({
      ...user,
      id: user.email
    }))
    console.log('Raw users response:', users.value)
    console.log('Users fetched:', users.value)
    error.value = ''
  } catch (error) {
    console.error('Error fetching users:', error)
    error.value = 'Failed to fetch users: ' + (error.response?.data?.Message || error.message || 'Unknown error')
  }
}

const fetchTeams = async () => {
  try {
    const response = await api.get('/api/teams')
    console.log('Raw teams response:', response.data)
    availableTeams.value = response.data.map(team => ({
      id: team.id,
      name: team.name
    }))
    if (availableTeams.value.length === 0) {
      console.warn('No teams fetched from /api/teams')
      error.value = 'No teams available. Please create a team first.'
    } else {
      console.log('Teams fetched:', availableTeams.value)
      error.value = ''
    }
  } catch (error) {
    console.error('Error fetching teams:', error)
    error.value = 'Failed to fetch teams: ' + (error.response?.data?.Message || error.message || 'Unknown error')
  }
}

const getTeamName = (teamId) => {
  if (!teamId) return null
  const team = availableTeams.value.find(t => t.id === teamId)
  return team ? team.name : 'Unknown team'
}

const showEmailDialog = async (user) => {
  console.log('Opening email dialog for user:', user)
  selectedUserForEmail.value = user
  emailDialogVisible.value = true
  try {
    const response = await api.post('/api/UserTenants/get-id-by-email', user.email, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('accessToken')}`
      }
    })
    const accountId = response.data
    console.log('Account ID:', accountId)
    const hoursResponse = await api.get(`/api/timelog/today/${accountId}`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('accessToken')}`
      }
    })
    if (hoursResponse.data === 'No PE tracking data found for today.') {
      hhours.value = '0h 00m'
    } else {
      hhours.value = hoursResponse.data
    }
    const WeekResponse = await api.get(`/api/timelog/weekly/${accountId}`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('accessToken')}`
      }
    })
    const weeklyData = WeekResponse.data
    const totalHours = Object.values(weeklyData).reduce((sum, hours) => sum + hours, 0)
    const hours = Math.floor(totalHours)
    const minutes = Math.floor((totalHours - hours) * 60)
    hhhours.value = `${hours}h ${minutes === 0 ? 0 : minutes}m`

    const monthlyhours = await api.get(`/api/timelog/monthly/${accountId}`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('accessToken')}`
      }
    })
    hhhhours.value = monthlyhours.data
    console.log('Hours:', hhours.value)
    error.value = ''
  } catch (error) {
    console.error('Error fetching hours:', error)
    hhours.value = null
    error.value = 'Failed to fetch hours: ' + (error.response?.data?.Message || error.message || 'Unknown error')
  }
}

const openAddToTeamDialog = (user) => {
  console.log('Opening add to team dialog for user:', user)
  if (availableTeams.value.length === 0) {
    error.value = 'Cannot add user to team: No teams available.'
    return
  }
  try {
    selectedUser.value = { ...user }
    currentId.value = user.email
    isEdit.value = true // Set to true assuming UserDialog handles team assignment as an edit
    dialogVisible.value = true
  } catch (err) {
    console.error('Error opening add to team dialog:', err)
    error.value = 'Failed to open team assignment dialog: ' + err.message
  }
}

const openAddDialog = () => {
  console.log('Opening add dialog')
  if (availableTeams.value.length === 0) {
    error.value = 'Cannot add user: No teams available.'
    return
  }
  isEdit.value = false
  selectedUser.value = null
  currentId.value = null
  dialogVisible.value = true
}

const closeDialog = () => {
  console.log('Closing add/edit dialog')
  dialogVisible.value = false
  selectedUser.value = null
  currentId.value = null
}

const onUserDialogSave = async () => {
  try {
    await fetchUsers()
    successMessage.value = `Team assignment updated successfully`
    setTimeout(() => {
      successMessage.value = null
    }, 7000)
  } catch (error) {
    console.error('Error refreshing users after dialog save:', error)
    error.value = 'Failed to refresh users: ' + (error.response?.data?.Message || error.message || 'Unknown error')
  }
}

const showRemoveDialog = (user) => {
  console.log('Opening remove dialog for user:', user)
  selectedUserForRemoval.value = user
  removeDialogVisible.value = true
  removeError.value = ''
}

const removeUserFromTeam = async () => {
  if (!selectedUserForRemoval.value?.email) {
    removeError.value = 'User email is required.'
    return
  }
  try {
    console.log('Removing user from team:', selectedUserForRemoval.value)
    const idResponse = await api.post('/api/UserTenants/get-id-by-email', selectedUserForRemoval.value.email, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('accessToken')}`
      }
    })
    const userId = idResponse.data
    console.log('Retrieved userId:', userId)
    await api.put(`/api/UserTenants/remove-team/${userId}`, null, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('accessToken')}`
      }
    })
    await fetchUsers()
    removeDialogVisible.value = false
    successMessage.value = `${selectedUserForRemoval.value.username || selectedUserForRemoval.value.email} has been removed from the team successfully`
    setTimeout(() => {
      successMessage.value = null
    }, 7000)
  } catch (error) {
    console.error('Error removing user from team:', error.response ? error.response.data : error)
    removeError.value = error.response?.data?.Message || 'Failed to remove user from team.'
    error.value = 'Failed to remove user from team: ' + (error.response?.data?.Message || error.message || 'Unknown error')
  }
}

const confirmDelete = (user) => {
  console.log('UserAccounts.vue: deleteDialogRef:', deleteDialogRef?.value)
  console.log('UserAccounts.vue: showDeleteDialog available:', !!deleteDialogRef?.value?.showDeleteDialog)
  if (deleteDialogRef?.value && deleteDialogRef.value.showDeleteDialog) {
    deleteDialogRef.value.showDeleteDialog({
      item: user,
      type: 'user',
      name: user.username || `Email ${user.email}`,
      onConfirm: () => deleteUser(user)
    })
  } else {
    console.error('DeleteDialog instance not found or showDeleteDialog not exposed')
    error.value = 'Failed to open delete dialog: DeleteDialog instance not found'
  }
}

const deleteUser = async (user) => {
  try {
    console.log('Deleting user:', user)
    const id = await api.post('/api/UserTenants/get-id-by-email', user.email, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('accessToken')}`
      }
    })
    console.log('User ID:', id.data)
    const idd = id.data
    await api.delete(`/api/UserTenants/delete/${idd}`, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('accessToken')}`
      }
    })
    await fetchUsers()
    successMessage.value = `${user.username || user.email} has been Fired successfully`
    setTimeout(() => {
      successMessage.value = null
    }, 7000)
  } catch (error) {
    console.error('Error deleting user:', error)
    error.value = 'Failed to delete user: ' + (error.response?.data?.Message || error.message || 'Unknown error')
  }
}

const showAllocateDialog = (user) => {
  selectedUserForAllocation.value = user
  allocateDialogVisible.value = true
}

const allocateLeaveBalance = async ({ user, days }) => {
  try {
    console.log('Allocating leave balance for user:', user)
    const email = user.email
    console.log('Retrieved email:', email)
    const idResponse = await api.post('/api/UserTenants/get-id-by-email', email, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('accessToken')}`
      }
    })
    const userId = idResponse.data
    console.log('Retrieved userId:', userId)
    const allocationResponse = await api.put(`/api/LeaveRequests/balance/${userId}`, days, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('accessToken')}`
      }
    })
    console.log('Allocation response:', allocationResponse.data)
    successMessage.value = `Leave balance allocated successfully for ${user.username || user.email}`
    setTimeout(() => {
      successMessage.value = null
    }, 7000)
  } catch (error) {
    console.error('Error allocating leave balance:', error)
    error.value = 'Failed to allocate leave balance: ' + (error.response?.data?.Message || error.message || 'Unknown error')
  }
}

onMounted(() => {
  fetchUsers()
  fetchTeams()
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
  border: 1px solid #ced4da;
  border-radius: 6px;
  width: 300px;
  margin-bottom: 1rem;
  padding: 0.5rem;
  transition: border-color 0.2s;
}

.search-input:focus {
  border-color: #6366f1;
  box-shadow: 0 0 0 3px rgba(99, 102, 241, 0.2);
}

h2 {
  font-size: 2rem;
  margin: 0;
}

.username-link {
  color: rgb(0, 0, 0);
  cursor: pointer;
  text-decoration: underline;
}

.username-link:hover {
  color: #35D300;
  text-decoration: underline;
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
  border-color: white;
}

.add-button:disabled {
  color: #d1d5db !important;
  border-color: #d1d5db !important;
  cursor: not-allowed;
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
  border-color: white;
}

.allocate-button {
  border-radius: 6px;
  padding: 0.5rem 1rem;
  font-weight: 500;
  transition: background-color 0.2s, transform 0.1s;
  color: #ff8000 !important;
  border-color: #ff8000 !important;
  background-color: white;
}

.removeteam-button {
  border-radius: 6px;
  padding: 0.5rem 1rem;
  font-weight: 500;
  transition: background-color 0.2s, transform 0.1s;
  color: #007BFF !important;
  border-color: #007BFF !important;
  background-color: white;
}

.allocate-button:hover {
  transform: translateY(-1px);
  background-color: #ff8000 !important;
  color: white !important;
  border-color: white;
}

.removeteam-button:hover {
  transform: translateY(-1px);
  background-color: #007BFF !important;
  color: white !important;
  border-color: white;
}

.card {
  margin-top: 1rem;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  background: #ffffff;
}

.text-center {
  text-align: center;
}

.text-muted {
  color: #6b7280;
}

.actions-container {
  display: flex;
  gap: 0.5rem;
  white-space: nowrap;
}

.stunning-dialog {
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
  color: #000000;
  font-size: 1.3rem;
  font-weight: bold;
  text-align: center;
}

.dialog-divider {
  margin: 1rem 0;
}

.error-message {
  margin-bottom: 1rem;
  border-radius: 8px;
  border-left: 4px solid #ef4444;
  background-color: #fef2f2;
}

.success-message {
  margin: 1rem 0;
  border-radius: 8px;
  border-left: 4px solid #22c55e;
  background-color: #d1fae5;
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
  background: #ff0000;
  color: #ffffff;
  border-color: #ff0000;
}

.stunning-button-remove {
  background: transparent !important;
  color: #007BFF !important;
  border: 1px solid #007BFF !important;
}

.stunning-button-remove:hover:not(:disabled) {
  background: #007BFF !important;
  color: #ffffff !important;
  border-color: #007BFF !important;
  transform: translateY(-1px);
  box-shadow: 0 2px 4px rgba(0, 123, 255, 0.3);
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

:deep(.p-button) {
  transition: all 0.2s ease;
}

:deep(.p-button.p-button-text.stunning-button-cancel:hover) {
  background-color: #ff0000 !important;
  color: #ffffff !important;
  border-color: #ff0000 !important;
}

:deep(.p-button.p-button-text.stunning-button-remove:hover) {
  background-color: #007BFF !important;
  color: #ffffff !important;
  border-color: #007BFF !important;
}

/* Tooltip styles */
:deep(.p-tooltip) {
  background-color: #000000 !important;
  color: #ffffff !important;
  font-size: 0.9rem;
  padding: 0.5rem 0.75rem;
  border-radius: 4px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
  max-width: 200px;
  line-height: 1.2;
}

:deep(.p-tooltip-arrow) {
  border-bottom-color: #000000 !important;
}

:deep(.p-tooltip.add-button-tooltip) {
  background-color: #35D300 !important;
}

:deep(.p-tooltip.add-button1-tooltip) {
  background-color: #ff0000 !important;
}

:deep(.p-tooltip.allocate-button-tooltip) {
  background-color: #ff8000 !important;
}

:deep(.p-tooltip.removeteam-button-tooltip) {
  background-color: #007BFF !important;
}
</style>