<template>
  <div class="card">
    <h2>Employee management</h2>

    <!-- Add Employer Button -->
    <Button label="Add an employee" icon="pi pi-plus" class="mb-4" @click="openAddDialog" />

    <!-- Employers Table -->
    <DataTable :value="employers" tableStyle="min-width: 50rem">
      <Column field="name" header="Name" />
      <Column field="email" header="Email" />
      <Column field="role" header="Role" />
      <Column :exportable="false" style="min-width: 12rem">
        <template #body="slotProps">
          <Button icon="pi pi-pencil" outlined rounded class="mr-2"
                  @click="openEditDialog(slotProps.data)" />
          <Button icon="pi pi-trash" outlined rounded severity="danger"
                  @click="deleteEmployer(slotProps.data)" />
        </template>
      </Column>
    </DataTable>

    <!-- Add/Edit Dialog -->
    <Dialog v-model:visible="dialogVisible" modal :header="isEdit ? 'Edit an employee' : 'Add an employee'"
            class="w-[30rem]">
      <div class="p-fluid">
        <div class="field">
          <label for="name">Name</label>
          <InputText v-model="form.name" id="name" />
        </div>
        <div class="field">
          <label for="email">Email</label>
          <InputText v-model="form.email" id="email" />
        </div>
        <div class="field">
          <label for="role">Role</label>
          <Dropdown v-model="form.role" :options="roles" placeholder="Sélectionner un rôle" />
        </div>
      </div>

      <template #footer>
        <Button label="Cancel" icon="pi pi-times" class="p-button-text" @click="closeDialog" />
        <Button label="Save" icon="pi pi-check" @click="saveEmployer" />
      </template>
    </Dialog>
  </div>
</template>
<script setup>
import { ref } from 'vue'

// ✅ PrimeVue component imports (no destructuring)
import Button from 'primevue/button'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import Dropdown from 'primevue/dropdown'

// Sample employer data
const employers = ref([
  { id: 1, name: 'Jean Dupont', email: 'jean@entreprise.com', role: 'Développeur' },
  { id: 2, name: 'Alice Martin', email: 'alice@entreprise.com', role: 'Designer' }
])

const dialogVisible = ref(false)
const isEdit = ref(false)
const currentId = ref(null)

const form = ref({
  name: '',
  email: '',
  role: ''
})

// Role dropdown options
const roles = ['Développeur', 'Designer', 'Manager', 'RH']

const openEditDialog = (employer) => {
  form.value = { ...employer }
  currentId.value = employer.id
  isEdit.value = true
  dialogVisible.value = true
}

const openAddDialog = () => {
  isEdit.value = false
  form.value = { name: '', email: '', role: '' }
  dialogVisible.value = true
}

const closeDialog = () => {
  dialogVisible.value = false
}

const saveEmployer = () => {
  if (isEdit.value) {
    const index = employers.value.findIndex(e => e.id === currentId.value)
    if (index !== -1) {
      employers.value[index] = { id: currentId.value, ...form.value }
    }
  } else {
    const newId = Math.max(0, ...employers.value.map(e => e.id)) + 1
    employers.value.push({ id: newId, ...form.value })
  }

  dialogVisible.value = false
  isEdit.value = false
  form.value = { name: '', email: '', role: '' }
}

const deleteEmployer = (employer) => {
  employers.value = employers.value.filter(e => e.id !== employer.id)
}
</script>

<style scoped>
.field {
  margin-bottom: 1rem;
}
</style>
