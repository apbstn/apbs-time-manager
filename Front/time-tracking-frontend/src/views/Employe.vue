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
        <Column field="username" header="Username" sortable style="max-width: 6rem;" />
        <Column field="email" header="Email" sortable style="max-width: 6rem;" />
        <Column field="phoneNumber" header="Phone Number" sortable style="max-width: 6rem;" />
        <Column field="teamId" header="Teams" sortable style="max-width: 6rem;">
          <template #body="slotProps">
            {{ getTeamName(slotProps.data.teamId) || 'No team' }}
          </template>
        </Column>
        <Column :exportable="false" style="max-width: 4rem" header="Actions">
          <template #body="slotProps">
            <Button icon="pi pi-pencil" class="add-button" @click="openEditDialog(slotProps.data)" />  
            <Button icon="pi pi-trash" class="add-button1" @click="confirmDelete(slotProps.data)" />  
            <Button icon="pi pi-plus-circle" class="allocate-button" @click="showAllocateDialog(slotProps.data)" />
          </template>
        </Column>
        <template #empty>
          <div class="text-center text-muted">No users found</div>
        </template>
      </DataTable>
    </div>

    <!-- Add/Edit Dialog -->
    <UserDialog :visible="dialogVisible" :isEdit="isEdit" :user="selectedUser" :currentId="currentId" :teams="availableTeams"
      @update:visible="dialogVisible = $event" @refresh="fetchUsers" @close="closeDialog" />

    <!-- Allocate Leave Balance Dialog -->
    <AllocateLeaveDialog v-model:visible="allocateDialogVisible" :user="selectedUserForAllocation" @allocate="allocateLeaveBalance" />
  </div>
</template>

<script setup>
import { ref, onMounted, computed, inject } from 'vue'
import api from '@/api'
import Button from 'primevue/button'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import InputText from 'primevue/inputtext'
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
const availableTeams = ref([]) // Store available teams for the dropdown
const allocateDialogVisible = ref(false)
const selectedUserForAllocation = ref(null)

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
      id: user.email // Use email as a temporary id since id is missing
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
  currentId.value = user.email // Use email as currentId since id is missing
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
    await api.delete(`/api/UserTenants/accounts/${user.email}`)
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

    // Step 1: Retrieve the email from the selected user data
    const email = user.email
    console.log('Retrieved email:', email)

    // Step 2: Use get-id-by-email to get the userId
    const idResponse = await api.post('/api/UserTenants/get-id-by-email',email , {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('accessToken')}`
      }
    })
    const userId = idResponse.data
    console.log('Retrieved userId:', userId)

    // Step 3: Use POST /api/LeaveRequests/balance/allocate/{id} with the entered days
    const allocationResponse = await api.put(`/api/LeaveRequests/balance/${userId}`, days , {
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

.allocate-button:hover {
  transform: translateY(-1px);
  background-color: #ff8000 !important;
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
</style>