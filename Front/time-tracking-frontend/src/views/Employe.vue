<template>
  <div>
    <div class="header-container">
      <div class="flex justify-content-between align-items-center mb-2">
        <h2>Employee Management</h2>
        <Button label="Add an employee" icon="pi pi-plus" class="add-button" @click="openAddDialog" />
      </div>
    </div>

    <InputText v-model="searchQuery" placeholder="Search employees..." class="search-input" />

    <div class="card">
      <DataTable :value="filteredEmployers" paginator :rows="10" tableStyle="min-width: 50rem" :showGridlines="true">
        <Column field="name" header="Name" sortable style="max-width: 6rem;" />
        <Column field="email" header="Email" sortable style="max-width: 6rem;" />
        <Column field="role" header="Role" sortable style="max-width: 6rem;" />
        <Column :exportable="false" style="max-width: 1rem" header="Actions">
          <template #body="slotProps">
            <Button icon="pi pi-pencil" class="add-button" @click="openEditDialog(slotProps.data)" />&nbsp;
            <Button icon="pi pi-trash" class="add-button1" @click="deleteEmployer(slotProps.data)" />
          </template>
        </Column>
        <template #empty>
          <div class="text-center text-muted">No employees found</div>
        </template>
      </DataTable>
    </div>

    <!-- Add/Edit Dialog -->
    <EmployeeDialog :visible="dialogVisible" :isEdit="isEdit" :employee="selectedEmployee" 
                    @update:visible="dialogVisible = $event" @save="handleSave" @close="closeDialog" />
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import Button from 'primevue/button'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import InputText from 'primevue/inputtext'
import EmployeeDialog from './Componant/EmployeeDialog.vue'

const employers = ref([
  { id: 1, name: 'Jean Dupont', email: 'jean@entreprise.com', role: 'Développeur' },
  { id: 2, name: 'Alice Martin', email: 'alice@entreprise.com', role: 'Designer' }
])

const dialogVisible = ref(false)
const isEdit = ref(false)
const selectedEmployee = ref(null)
const searchQuery = ref('')

const filteredEmployers = computed(() => {
  if (!searchQuery.value) return employers.value
  const query = searchQuery.value.toLowerCase()
  return employers.value.filter(employer =>
    employer.name.toLowerCase().includes(query) ||
    employer.email.toLowerCase().includes(query) ||
    employer.role.toLowerCase().includes(query)
  )
})

const openEditDialog = (employer) => {
  selectedEmployee.value = { ...employer }
  isEdit.value = true
  dialogVisible.value = true
}

const openAddDialog = () => {
  isEdit.value = false
  selectedEmployee.value = { name: '', email: '', role: '' }
  dialogVisible.value = true
}

const closeDialog = () => {
  dialogVisible.value = false
  selectedEmployee.value = null
}

const handleSave = (employee) => {
  if (isEdit.value) {
    const index = employers.value.findIndex(e => e.id === employee.id)
    if (index !== -1) {
      employers.value[index] = { ...employee }
    }
  } else {
    const newId = Math.max(0, ...employers.value.map(e => e.id)) + 1
    employers.value.push({ id: newId, ...employee })
  }
  closeDialog()
}

const deleteEmployer = (employer) => {
  employers.value = employers.value.filter(e => e.id !== employer.id)
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