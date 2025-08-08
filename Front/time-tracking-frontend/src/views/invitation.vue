<template>
  <div>
    <div class="header-container">
      <div class="flex justify-content-between align-items-center mb-2">
        <h2 style="font-size: 22px; color: #6B7280;">Employee Invitations</h2>
        <Button label="Add" icon="pi pi-plus" class="add-button" @click="openAddDialog" v-tooltip.bottom="{
                value: 'Invite an employee',
                pt: {
                  arrow: {
                    style: {
                      borderBottomColor: '#000000',
                    },
                  },
                  text: '!bg-black !text-white !font-medium',
                }
              }" />
      </div>
    </div>

    <InputText v-model="searchQuery" placeholder="Search invitations..." class="search-input" />

    <div class="card">
      <DataTable :value="filteredInvitations" paginator :rows="10" tableStyle="min-width: 50rem" :showGridlines="true">
        <Column field="email" header="Email" sortable style="width: 33.3333%;">
          <template #body="{ data }">
            {{ formatEmail(data.email) || 'N/A' }}
          </template>
        </Column>
        <Column field="phone_Number" header="Phone Number" sortable style="width: 33.3333%;">
          <template #body="{ data }">
            {{ console.log('Phone Number Data:', data.phone_Number) }}
            {{ data.phone_Number || 'N/A' }}
          </template>
        </Column>
        <Column field="status" header="Status" sortable style="width: 33.3333%;">
          <template #body="{ data }">
            {{ statusDisplay(data.status) }}
          </template>
        </Column>
        <Column :exportable="false" style="width: 1%;" header="Actions">
          <template #body="slotProps">
            <div class="actions-container">
              <Button icon="pi pi-trash" class="add-button1"
                
                @click="confirmDelete(slotProps.data)" v-tooltip.top="{
                value: 'Delete the invitation',
                pt: {
                  arrow: {
                    style: {
                      borderBottomColor: '#000000',
                    },
                  },
                  text: '!bg-black !text-white !font-medium',
                }
              }" />
            </div>
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

const formatEmail = (email) => {
    if (!email) return null
    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/
    return emailRegex.test(email) ? email : null
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
        console.log('Raw API response:', data)
        invitations.value = data.map(invite => ({ ...invite, status: invite.isUsed ? 1 : 0 }))
        console.log('Mapped invitations:', invitations.value)
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
    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/
    if (newInvite.email && emailRegex.test(newInvite.email)) {
        invitations.value.push(newInvite)
        dialogVisible.value = false
    } else {
        console.error('Invalid email format:', newInvite.email)
        // Optionally, you could trigger a UI notification here
    }
}

const confirmDelete = (invite) => {
    console.log('Confirming delete for invite:', invite)
    if (deleteDialogRef?.value?.showDeleteDialog) {
        deleteDialogRef.value.showDeleteDialog({
            item: invite,
            type: 'invitation',
            name: invite.email,
            onConfirm: async () => {
                try {
                    console.log('Deleting invite:', invite.email);
                    await api.delete(`/api/auth/invite/delete/${invite.email}`)
                    window.location.reload();
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

.actions-container {
  display: flex;
  gap: 0.5rem;
  white-space: nowrap;
}
</style>