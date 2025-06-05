<template>
    <div>
        <div class="header-container">
            <div class="flex justify-content-between align-items-center mb-2">
                <h2>Tenants</h2>
              
            </div>
        </div>

        <InputText v-model="searchQuery" placeholder="Search tenants..." class="search-input" />

        <div class="card">
            <div v-if="isLoading" class="text-center">Loading tenants...</div>
            <div v-else-if="error" class="text-center text-red-500">{{ error }}</div>
            <DataTable v-else :value="filteredTenants" paginator :rows="10" tableStyle="min-width: 50rem"
                :showGridlines="true" @row-click="onRowClick" class="clickable-rows">
                <Column field="name" header="Name" sortable>
                    <template #body="{ data }">
                        <span class="clickable-name">{{ data.name || 'Unnamed Tenant' }}</span>
                    </template>
                </Column>
                <template #empty>
                    <div class="text-center text-muted">No tenant names found</div>
                </template>
            </DataTable>
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import InputText from 'primevue/inputtext'

import api from '@/api'

const tenants = ref([])
const searchQuery = ref('')
const isLoading = ref(false)
const error = ref(null)

// Filter tenants based on search query
const filteredTenants = computed(() => {
    if (!searchQuery.value) return tenants.value
    const query = searchQuery.value.toLowerCase()
    return tenants.value.filter(tenant =>
        tenant.name && tenant.name.toLowerCase().includes(query)
    )
})

// Handle row click to switch tenant
const onRowClick = async (event) => {
    const tenant = event.data
    if (!tenant || !tenant.id) {
        console.error('No tenant ID found for the clicked row')
        error.value = 'Unable to switch tenant: Missing tenant ID.'
        return
    }

    try {
        console.log('Switching to tenant:', tenant.id)

        // Step 1: Get the token
        const oldToken = localStorage.getItem('accessToken')
        console.log('Step 1: Retrieved old token:', oldToken)

        if (!oldToken) {
            throw new Error('No access token found in localStorage')
        }

        // Step 2: Call the API
        const response = await api.post(`/api/auth/switch/${tenant.id}`, {}, {
            headers: {
                'Authorization': `Bearer ${oldToken}`
            }
        })
        console.log('Step 2: Switch API response:', response.data)

        // Step 3: Store the new token in a new const
        const newToken = response.data.accessToken
        console.log('Step 3: New token stored in const:', newToken)

        if (!newToken) {
            throw new Error('No access token found in switch response')
        }

        // Step 4: Delete the old token
        localStorage.removeItem('accessToken')
        console.log('Step 4: Old token deleted from localStorage')

        // Step 5: Store the new token in the place of the old token
        localStorage.setItem('accessToken', newToken)
        console.log('Step 5: New token stored in localStorage:', newToken)

        // Step 6: Refresh
        console.log('Step 6: Refreshing the page')
        window.location.href = '/home'; // Redirect to home page
        
     
    } catch (err) {
        console.error('Error switching tenant:', err)
        error.value = 'Failed to switch tenant: ' + (err.response?.data?.message || err.message)
        localStorage.removeItem('accessToken') // Ensure token is cleared on error
        console.log('Cleared accessToken due to error')
    }
}

// Placeholder function (to be implemented if needed)

// Load tenants from localStorage on mount
onMounted(() => {
    console.log('Component mounted')
    isLoading.value = true
    error.value = null

    try {
        const accessToken = localStorage.getItem('accessToken')
        console.log('Access token:', accessToken ? 'Present' : 'Missing')

        if (!accessToken) {
            error.value = 'No access token found. Please log in.'
            return
        }

        const storedTenants = localStorage.getItem('tenants')
        console.log('Stored tenants:', storedTenants)

        if (storedTenants) {
            tenants.value = JSON.parse(storedTenants)
            if (!Array.isArray(tenants.value)) {
                tenants.value = [tenants.value] // Ensure it's an array
            }
            if (!tenants.value.length) {
                console.log('No tenants found in localStorage')
            } else {
                console.log('Tenant objects:', tenants.value)
            }
        } else {
            error.value = 'No tenants found in localStorage. Please log in again.'
        }
    } catch (err) {
        console.error('Error loading tenants from localStorage:', err)
        error.value = 'Failed to load tenants from localStorage. Check console for details.'
    } finally {
        isLoading.value = false
    }
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

.text-red-500 {
    color: #ef4444;
}

.field {
    margin-bottom: 1.5rem;
}

.clickable-rows {
    cursor: pointer;
}

.clickable-name {
    color: #007bff;
    text-decoration: underline;
}

.clickable-name:hover {
    color: #0056b3;
}
</style>