<template>
  <div>
    <div class="header-container">
      <div class="flex justify-content-between align-items-center mb-2">
        <h2>Teams</h2>
        <Button label="Add" icon="pi pi-plus" class="add-button" @click="openAddDialog" />
      </div>
    </div>
    
    <InputText v-model="searchQuery" placeholder="Search teams..." class="search-input" />

    <div class="card">
      <DataTable :value="filteredTeams" paginator :rows="10" tableStyle="min-width: 50rem" :showGridlines="true">
        <Column field="name" header="Name" sortable />
        <Column field="description" header="Description" sortable />
        <Column :exportable="false" style="min-width: 12rem" header="Actions">
          <template #body="slotProps">
            <Button icon="pi pi-pencil" class="p-button-warning p-button-sm mr-2"
              @click="openEditDialog(slotProps.data)" />
            <Button icon="pi pi-trash" class="p-button-danger p-button-sm" @click="deleteTeam(slotProps.data)" />
          </template>
        </Column>
        <template #empty>
          <div class="text-center text-muted">No teams found</div>
        </template>
      </DataTable>
    </div>

    <!-- Add/Edit Dialog -->
    <Dialog v-model:visible="dialogVisible" modal :header="isEdit ? 'Edit Team' : 'Add Team'" class="w-[30rem]">
      <div class="p-fluid">
        <div class="field">
          <label for="name">Team Name</label>
          <InputText v-model="form.name" id="name" />
        </div>
        <div class="field">
          <label for="description">Description</label>
          <InputText v-model="form.description" id="description" />
        </div>
      </div>

      <template #footer>
        <Button label="Cancel" icon="pi pi-times" class="p-button-text" @click="closeDialog" />
        <Button label="Save" icon="pi pi-check" @click="saveTeam" />
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
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'

const teams = ref([])
const dialogVisible = ref(false)
const isEdit = ref(false)
const currentId = ref(null)
const searchQuery = ref('')

const form = ref({
  name: '',
  description: ''
})

const filteredTeams = computed(() => {
  if (!searchQuery.value) return teams.value
  const query = searchQuery.value.toLowerCase()
  return teams.value.filter(team => 
    team.name.toLowerCase().includes(query) || 
    (team.description && team.description.toLowerCase().includes(query))
  )
})

const fetchTeams = async () => {
  try {
    const response = await api.get('/api/teams')
    teams.value = response.data
  } catch (error) {
    console.error('Error fetching teams:', error)
  }
}

const openEditDialog = (team) => {
  form.value = { ...team }
  currentId.value = team.id
  isEdit.value = true
  dialogVisible.value = true
}

const openAddDialog = () => {
  isEdit.value = false
  form.value = { name: '', description: '' }
  dialogVisible.value = true
}

const closeDialog = () => {
  dialogVisible.value = false
}

const saveTeam = async () => {
  try {
    if (isEdit.value) {
      await api.put(`/api/teams/${currentId.value}`, form.value)
    } else {
      await api.post('/api/teams', form.value)
    }
    await fetchTeams()
    closeDialog()
  } catch (error) {
    console.error('Error saving team:', error)
  }
}

const deleteTeam = async (team) => {
  try {
    await api.delete(`/api/teams/${team.id}`)
    await fetchTeams()
  } catch (error) {
    console.error('Error deleting team:', error)
  }
}

onMounted(fetchTeams)
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
</style>