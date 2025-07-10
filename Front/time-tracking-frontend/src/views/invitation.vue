<template>
  <div>
    <div class="header-container">
      <div class="flex justify-content-between align-items-center mb-2">
        <h2 style="font-size: 22px; color: #6B7280;">User Invitations</h2>
        <Button label="Add" icon="pi pi-plus" class="add-button" @click="openAddDialog" />
      </div>
    </div>

    <InputText v-model="searchQuery" placeholder="Search invitations..." class="search-input" />

    <div class="card">
      <DataTable :value="filteredInvitations" paginator :rows="10" tableStyle="min-width: 50rem" :showGridlines="true">
        <Column field="email" header="Email" sortable style="max-width: 5rem">
            <template #body="{ data }">
                {{ data.email || 'N/A' }}
            </template>
        </Column>
        <Column field="phone_Number" header="Phone Number" sortable style="max-width: 5rem">
            <template #body="{ data }">
                {{ console.log('Phone Number Data:', data.phone_Number) }}
                {{ data.phone_Number || 'N/A' }}
            </template>
        </Column>
        <Column field="status" header="Status" sortable style="max-width: 5rem">
          <template #body="{ data }">
            {{ statusDisplay(data.status) }}
          </template>
        </Column>
        <Column :exportable="false" style="max-width: 1rem" header="Actions">
          <template #body="slotProps">
            <Button icon="pi pi-trash" class="add-button1"
              :disabled="slotProps.data.status === 1"
              @click="confirmDelete(slotProps.data)" />
          </template>
        </Column>
        <template #empty>
          <div class="text-center text-muted">No invitations found</div>
        </template>
      </DataTable>
    </div>

    <!-- Invite Dialog -->
    <InviteDialog 
      :visible="dialogVisible" 
      :userId="userId"
      @update:visible="dialogVisible = $event"
      @save="handleSave"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, computed, inject } from 'vue'
import api from '@/api'
import Button from 'primevue/button'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import InputText from 'primevue/inputtext'
import InviteDialog from './Componant/InviteDialog.vue'

const invitations = ref([])
const dialogVisible = ref(false)
const searchQuery = ref('')
const userId = ref(null)
const tenantId = ref(null)
const deleteDialogRef = inject('deleteDialog')

const filteredInvitations = computed(() => {
    if (!searchQuery.value) return invitations.value
    const query = searchQuery.value.toLowerCase()
    return invitations.value.filter(invite =>
        (invite.email && invite.email.toLowerCase().includes(query)) ||
        (invite.phone_Number && invite.phone_Number.toLowerCase().includes(query))
    )
})

const statusDisplay = (status) => {
    switch (status) {
        case 0: return 'Pending'
        case 1: return 'Used'
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

onMounted(async () => {
    console.log('UserInvitations mounted')
    const accessToken = localStorage.getItem('accessToken')
    if (accessToken) {
        const decoded = decodeJwt(accessToken)
        if (decoded && decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']) {
            userId.value = decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']
            tenantId.value = decoded['tenant_id']
            await fetchInvitations()
        } else {
            console.error('User ID not found in token')
        }
    } else {
        console.error('Access token not found')
    }
})

const fetchInvitations = async () => {
    try {
        if (!userId.value) {
            throw new Error('User ID is not available')
        }
        const { data } = await api.get(`/api/auth/invite/${tenantId.value}`)
        console.log('Raw API response:', data) // Debug
        invitations.value = data.map(invite => ({ ...invite, status: invite.isUsed ? 1 : 0 }))
        console.log('Mapped invitations:', invitations.value) // Debug
    } catch (error) {
        console.error('Error fetching invitations:', error)
    }
}

const openAddDialog = () => {
    console.log('Opening add dialog')
    dialogVisible.value = true
}

const handleSave = (newInvite) => {
    console.log('Handling save from dialog:', newInvite)
    invitations.value.push(newInvite)
    dialogVisible.value = false
}

const confirmDelete = (invite) => {
    console.log('Confirming delete for invite:', invite)
    if (deleteDialogRef?.value?.showDeleteDialog) {
        deleteDialogRef.value.showDeleteDialog({
            item: invite,
            type: 'invitation',
            name: invite.email || `ID ${invite.id}`,
            onConfirm: async () => {
                try {
                    console.log('Deleting invite:', invite)
                    await api.delete(`/api/invitations/${invite.id}`)
                    invitations.value = invitations.value.filter(i => i.id !== invite.id)
                } catch (error) {
                    console.error('Error deleting invitation:', error)
                }
            }
        })
    } else {
        console.error('DeleteDialog not injected or showDeleteDialog not exposed')
    }
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
}

.text-center {
    text-align: center;
}

.text-muted {
    color: #6c757d;
}

:deep(.p-datatable .p-datatable-tbody td) {
    white-space: nowrap;
    overflow: visible;
}
</style>