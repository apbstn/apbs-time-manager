<template>
  <div>
    <div class="header-container">
      <div class="flex justify-content-between align-items-center mb-2">
        <h2 style="font-size: 22px; color: #6B7280;">Teams</h2>
        <Button label="Add" icon="pi pi-plus" class="add-button" @click="openAddDialog" />
      </div>
    </div>

    <InputText v-model="searchQuery" placeholder="Search teams..." class="search-input" />

    <div class="card">
      <DataTable responsiveLayout="stack" breakpoint="960px" :value="filteredTeams" paginator :rows="10"
        tableStyle="min-width: 50rem" :showGridlines="true">
        <Column field="name" header="Name" sortable style="max-width: 6rem;" />
        <Column field="description" header="Description" sortable style="max-width: 6rem;" />
        <Column :exportable="false" style="max-width: 1rem" header="Actions">
          <template #body="slotProps">
            <Button icon="pi pi-pencil" class="add-button" @click="openEditDialog(slotProps.data)" /> &nbsp;
            <Button icon="pi pi-trash" class="add-button1" @click="confirmDelete(slotProps.data)" />
          </template>
        </Column>
        <template #empty>
          <div class="text-center text-muted">No teams found</div>
        </template>
      </DataTable>
    </div>

    <!-- Add/Edit Dialog -->
    <TeamDialog :visible="dialogVisible" :isEdit="isEdit" :team="selectedTeam" :currentId="currentId"
      @update:visible="dialogVisible = $event" @refresh="fetchTeams" @close="closeDialog" />
  </div>
</template>

<script setup>
import { ref, onMounted, computed, inject, watch } from 'vue'
import api from '@/api'
import Button from 'primevue/button'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import InputText from 'primevue/inputtext'
import TeamDialog from './Componant/TeamDialog.vue'

// Inject the DeleteDialog instance
const deleteDialogRef = inject('deleteDialog')

const teams = ref([])
const dialogVisible = ref(false)
const isEdit = ref(false)
const currentId = ref(null)
const searchQuery = ref('')
const selectedTeam = ref(null)

watch(deleteDialogRef, (newVal) => {
  console.log('teams.vue: DeleteDialog ref updated:', newVal?.value)
})

onMounted(() => {
  console.log('teams.vue: Initial DeleteDialog ref:', deleteDialogRef?.value)
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
    console.log('Teams fetched:', response.data)
  } catch (error) {
    console.error('Error fetching teams:', error)
  }
}

const openEditDialog = (team) => {
  console.log('Opening edit dialog for team:', team)
  selectedTeam.value = { ...team }
  currentId.value = team.id
  isEdit.value = true
  dialogVisible.value = true
}

const openAddDialog = () => {
  console.log('Opening add dialog')
  isEdit.value = false
  selectedTeam.value = null
  currentId.value = null
  dialogVisible.value = true
}

const closeDialog = () => {
  console.log('Closing add/edit dialog')
  dialogVisible.value = false
  selectedTeam.value = null
}

const confirmDelete = (team) => {
  console.log('teams.vue: deleteDialogRef:', deleteDialogRef?.value)
  console.log('teams.vue: showDeleteDialog available:', !!deleteDialogRef?.value?.showDeleteDialog)
  if (deleteDialogRef?.value && deleteDialogRef.value.showDeleteDialog) {
    deleteDialogRef.value.showDeleteDialog({
      item: team,
      type: 'team',
      name: team.name || `ID ${team.id}`,
      onConfirm: () => deleteTeam(team)
    })
  } else {
    console.error('DeleteDialog instance not found or showDeleteDialog not exposed')
  }
}

const deleteTeam = async (team) => {
  try {
    console.log('Deleting team:', team)
    await api.delete(`/api/teams/${team.id}`)
    await fetchTeams()
  } catch (error) {
    console.error('Error deleting team:', error)
  }
}

fetchTeams()
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
  background-color: rgba(255, 255, 255, 0);
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