<template>
  <div>
    <div class="header-container">
      <div class="flex justify-content-between align-items-center mb-2">
        <h2 style="font-size: 22px; color: #6B7280;">User Accounts</h2>
      </div>
    </div>

    <InputText v-model="searchQuery" placeholder="Search users..." class="search-input" />

    <div class="card">
      <DataTable :value="filteredUsers" paginator :rows="10" tableStyle="min-width: 50rem" :showGridlines="true">
        <Column field="username" header="Username" sortable style="width: 25%;" />
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
              <Button icon="pi pi-user-plus" class="add-button" v-tooltip="{
                value: 'Add Employee to a team',
                pt: {
                  arrow: {
                    style: {
                      borderBottomColor: '#000000',
                    },
                  },
                  text: '!bg-black !text-white !font-medium',
                }
              }" @click="openEditDialog(slotProps.data)" />

              <Button icon="pi pi-minus-circle" class="removeteam-button" v-tooltip="{
                value: 'Remove the User from the team',
                pt: {
                  arrow: {
                    style: {
                      borderBottomColor: '#000000',
                    },
                  },
                  text: '!bg-black !text-white !font-medium',
                }
              }" @click="showRemoveDialog(slotProps.data)" />

              <Button icon="pi pi-times" class="add-button1" v-tooltip="{
                value: 'Fire User',
                pt: {
                  arrow: {
                    style: {
                      borderBottomColor: '#000000',
                    },
                  },
                  text: '!bg-black !text-white !font-medium',
                }
              }" @click="confirmDelete(slotProps.data)" />
              <Button icon="pi pi-plus-circle" class="allocate-button" v-tooltip="{
                value: 'Allocate Leave Balance',
                pt: {
                  arrow: {
                    style: {
                      borderBottomColor: '#000000',
                    },
                  },
                  text: '!bg-black !text-white !font-medium',
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

    <!-- Add/Edit Dialog -->
    <UserDialog :visible="dialogVisible" :isEdit="isEdit" :user="selectedUser" :currentId="currentId"
      :teams="availableTeams" @update:visible="dialogVisible = $event" @refresh="fetchUsers" @close="closeDialog" />

    <!-- Allocate Leave Balance Dialog -->
    <AllocateLeaveDialog v-model:visible="allocateDialogVisible" :user="selectedUserForAllocation"
      @allocate="allocateLeaveBalance" />

    <!-- Remove from Team Dialog -->
    <Dialog :visible="removeDialogVisible" @update:visible="removeDialogVisible = $event"
      header="Remove User from Team" :modal="true" class="p-fluid stunning-dialog" :draggable="false" :style="{ width: '650px' }">
      <Divider class="dialog-divider" />
      <div class="dialog-content">
        <p class="dialog-subtitle">Are you sure you want to remove {{ selectedUserForRemoval.value?.username || selectedUserForRemoval.value?.email }} from their team?</p>
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
                    arrow: {
                      style: {
                        borderBottomColor: '#000000',
                      },
                    },
                    text: '!bg-black !text-white !font-medium',
                  }
                }"/>
        <Button label="Confirm" icon="pi pi-check" @click="removeUserFromTeam"
          class="stunning-button stunning-button-remove" v-tooltip="{
                  value: 'Confirm Removal',
                  pt: {
                    arrow: {
                      style: {
                        borderBottomColor: '#000000',
                      },
                    },
                    text: '!bg-black !text-white !font-medium',
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
  } catch (error) {
    console.error('Error fetching users:', error)
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
    } else {
      console.log('Teams fetched:', availableTeams.value)
    }
  } catch (error) {
    console.error('Error fetching teams:', error)
  }
}

const getTeamName = (teamId) => {
  if (!teamId) return null
  const team = availableTeams.value.find(t => t.id === teamId)
  return team ? team.name : 'Unknown team'
}

const openEditDialog = (user) => {
  console.log('Opening edit dialog for user:', user)
  selectedUser.value = { ...user }
  currentId.value = user.email
  isEdit.value = true
  dialogVisible.value = true
}

const openAddDialog = () => {
  console.log('Opening add dialog')
  isEdit.value = false
  selectedUser.value = null
  currentId.value = null
  dialogVisible.value = true
}

const closeDialog = () => {
  console.log('Closing add/edit dialog')
  dialogVisible.value = false
  selectedUser.value = null
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
    const response = await api.put(`/api/UserTenants/remove-team/${userId}`, null, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('accessToken')}`
      }
    })
    console.log('Remove response:', response.data)

    await fetchUsers()
    window.location.reload()
    removeDialogVisible.value = false
  } catch (error) {
    console.error('Error removing user from team:', error.response ? error.response.data : error)
    removeError.value = error.response?.data?.Message || 'Failed to remove user from team.'
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
  }
}

const deleteUser = async (user) => {
  try {
    console.log('Deleting user:', user)
    const id = await api.post('api/UserTenants/get-id-by-email/', user.email)
    console.log('User ID:', id.data)
    const idd = id.data
    await api.delete(`/api/UserTenants/delete/${idd}`)
    await fetchUsers()
  } catch (error) {
    console.error('Error deleting user:', error)
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
    alert('Leave balance allocated successfully!')
  } catch (error) {
    console.error('Error allocating leave balance:', error)
    alert('Failed to allocate leave balance. Check console for details.')
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
  border-color: white !important;
}

.removeteam-button:hover {
  transform: translateY(-1px);
  background-color: #007BFF !important;
  color: white !important;
  border-color: white !important;
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
  color: #4b5563;
  font-size: 0.95rem;
  font-weight: 400;
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