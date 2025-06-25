<template>
  <div>
    <div class="header-container">
      <div class="flex justify-content-between align-items-center mb-2">
        <h2>User Accounts</h2>
        <Button label="Add" icon="pi pi-plus" class="add-button" @click="openAddDialog" />
      </div>
    </div>

    <InputText v-model="searchQuery" placeholder="Search users..." class="search-input" />

    <div class="card">
      <DataTable :value="filteredUsers" paginator :rows="10" tableStyle="min-width: 50rem" :showGridlines="true">
        <Column field="username" header="Username" sortable style="max-width: 6rem;" />
        <Column field="email" header="Email" sortable style="max-width: 6rem;" />
        <Column field="phoneNumber" header="Phone Number" sortable style="max-width: 6rem;" />
        <Column field="team" header="Teams" sortable style="max-width: 6rem;"/>
        <Column :exportable="false" style="max-width: 3rem" header="Actions">
          <template #body="slotProps">
            <Button icon="pi pi-pencil" class="add-button" @click="openEditDialog(slotProps.data)" /> 
            <Button icon="pi pi-trash" class="add-button1" @click="confirmDelete(slotProps.data)" />
          </template>
        </Column>
        <template #empty>
          <div class="text-center text-muted">No users found</div>
        </template>
      </DataTable>
    </div>

    <!-- Add/Edit Dialog -->
    <UserDialog :visible="dialogVisible" :isEdit="isEdit" :user="selectedUser" :currentId="currentId"
      @update:visible="dialogVisible = $event" @refresh="fetchUsers" @close="closeDialog" />
  </div>
</template>

<script setup>
import { ref, onMounted, computed, inject } from 'vue'
import api from '@/api'
import Button from 'primevue/button'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import InputText from 'primevue/inputtext'


// Inject the DeleteDialog instance
const deleteDialogRef = inject('deleteDialog')

const users = ref([])
const dialogVisible = ref(false)
const isEdit = ref(false)
const currentId = ref(null)
const searchQuery = ref('')
const selectedUser = ref(null)

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
    users.value = response.data
    console.log('Users fetched:', response.data)
  } catch (error) {
    console.error('Error fetching users:', error)
  }
}

const openEditDialog = (user) => {
  console.log('Opening edit dialog for user:', user)
  selectedUser.value = { ...user }
  currentId.value = user.email // Assuming email is unique for users
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

fetchUsers()
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