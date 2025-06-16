<template>
    <div class="users-page">
        <!-- Error Message -->
        <Message v-if="error" severity="error" :closable="true" class="error-message">
            {{ error }}
        </Message>

        <!-- Add Tenant Button and Search -->
        <div class="header-container">
            <Toolbar class="header-toolbar">
                <template #start>
                    <div class="flex align-items-center">
                        <h2>Tenants</h2>
                        <span class="p-input-icon-left search-container">
                            <i class="pi pi-search" />
                            <InputText v-model="searchQuery" placeholder="Search tenants..." class="search-input" />
                        </span>
                    </div>
                </template>
                <template #end>
                    <Button label="Add Tenant" icon="pi pi-plus" class="add-button" @click="openAddPopup" outlined
                        style="border-color: #35D300; color: #35D300;" />
                </template>
            </Toolbar>
        </div>

        <AddTenantDialog :showDialog="isAdding" :tenant="newTenantData" :usersList="usersList"
            @update:showDialog="isAdding = $event" @save="saveNewTenant" />

        <div class="card">
            <DataTable :value="filteredTenants" :loading="loading" tableStyle="min-width: 100%" :showGridlines="true"
                responsiveLayout="scroll" class="p-datatable-sm">
                <template #empty>
                    <div class="text-center text-muted">No tenants found</div>
                </template>
                <Column field="tenantname" header="Tenant Name" sortable style="min-width: 200px"></Column>
                <Column field="email" header="Email" sortable style="min-width: 200px"></Column>
                <Column field="username" header="Username" sortable style="min-width: 150px"></Column>
                <Column field="phonenumber" header="Phone Number" sortable style="min-width: 150px">
                    <template #body="{ data }">
                        {{ data.phonenumber || 'N/A' }}
                    </template>
                </Column>
            </DataTable>
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import api from '../api';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import Button from 'primevue/button';
import Message from 'primevue/message';
import Toolbar from 'primevue/toolbar';
import InputText from 'primevue/inputtext';
import AddTenantDialog from './componant/AddTenantDialog.vue';

const users = ref([]);
const loading = ref(false);
const error = ref(null);
const isEditing = ref(false);
const isAdding = ref(false);
const searchQuery = ref('');
const editUserData = ref({});
const newTenantData = ref({
    tenantname: '',
    userId: null
});
const usersList = ref([]);

const filteredTenants = computed(() => {
    if (!searchQuery.value) return users.value;
    const query = searchQuery.value.toLowerCase();
    return users.value.filter(
        (tenant) =>
            tenant.tenantname?.toLowerCase().includes(query) ||
            tenant.email?.toLowerCase().includes(query) ||
            tenant.username?.toLowerCase().includes(query)
    );
});

const fetchTenants = async () => {
    try {
        loading.value = true;
        error.value = null;
        const token = localStorage.getItem('accessToken');

        if (!token) {
            throw new Error('No access token found');
        }

        const response = await api.get('/api/tenant', {
            headers: {
                Authorization: `Bearer ${token}`,
                Accept: 'application/json'
            }
        });

        users.value = response.data.items.map(item => ({
            code: item.code,
            tenantname: item.tenantName || 'N/A',
            email: item.email,
            username: item.username,
            phonenumber: item.phoneNumber
        }));
    } catch (err) {
        console.error('Error fetching tenants:', err);
        error.value =
            'Failed to fetch tenants: ' +
            (err.response?.data?.message || err.message || 'Unknown error');
    } finally {
        loading.value = false;
    }
};

const fetchUsers = async () => {
    try {
        loading.value = true;
        error.value = null;
        const token = localStorage.getItem('accessToken');

        if (!token) {
            throw new Error('No access token found');
        }

        const response = await api.get('/api/user', {
            headers: {
                Authorization: `Bearer ${token}`,
                Accept: 'application/json'
            }
        });

        usersList.value = response.data.map(user => ({
            id: user.id,
            username: user.username
        }));
    } catch (err) {
        console.error('Error fetching users:', err);
        error.value =
            'Failed to fetch users: ' +
            (err.response?.data?.message || err.message || 'Unknown error');
    } finally {
        loading.value = false;
    }
};

// --- Add logic ---
const openAddPopup = () => {
    isAdding.value = true;
    newTenantData.value = {
        tenantname: '',
        userId: null
    };
};

const saveNewTenant = async (tenantData) => {
    try {
        loading.value = true;
        error.value = null;
        const token = localStorage.getItem('accessToken');

        if (!token) {
            throw new Error('No access token found');
        }

        const payload = {
            tenantName: tenantData.tenantname,
            user: tenantData.userId
        };

        const response = await api.post('/api/tenant', payload, {
            headers: {
                Authorization: `Bearer ${token}`,
                'Content-Type': 'application/json',
                Accept: 'application/json'
            }
        });

        users.value.push({
            code: response.data.code || response.data.T_CODE,
            tenantname: response.data.tenantName || response.data.T_NAME || 'N/A',
            email: response.data.email || 'N/A',
            username: response.data.username || 'N/A',
            phonenumber: response.data.phoneNumber || 'N/A'
        });

        isAdding.value = false;
    } catch (error) {
        console.error('Error adding tenant:', error.response?.data || error.message);
        error.value = 'Failed to add tenant: ' + (error.response?.data?.message || error.message || 'Unknown error');
    } finally {
        loading.value = false;
    }
};

onMounted(() => {
    fetchTenants();
    fetchUsers();
});
</script>

<style scoped>
.users-page {
    width: 100%;
    min-height: 100vh;
    padding: 2rem;
    background-color: #f9fafb;
    box-sizing: border-box;
}

.header-container {
    margin-bottom: 2rem;
}

.header-toolbar {
    background: linear-gradient(145deg, #ffffff, #f1f5f9);
    border: none;
    border-radius: 10px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
    padding: 1rem;
    transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.header-toolbar:hover {
    transform: translateY(-2px);
    box-shadow: 0 6px 16px rgba(0, 0, 0, 0.1);
}

.flex {
    display: flex;
}

.align-items-center {
    align-items: center;
    gap: 1.5rem;
}

.search-container {
    display: flex;
    align-items: center;
    position: relative;
    /* Ensure proper positioning context */
}

.search-input {
    width: 250px;
    border-radius: 6px;
    padding: 0.5rem 0.5rem 0.5rem 2.5rem;
    /* Increased left padding to accommodate icon */
    border: 1px solid #d1d5db;
    transition: border-color 0.2s ease, box-shadow 0.2s ease;
}

.search-input:focus {
    border-color: #35D300;
    /* Changed from #3b82f6 (blue) to green */
    box-shadow: 0 0 0 3px rgba(53, 211, 0, 0.2);
    /* Changed from rgba(59, 130, 246, 0.2) to green */
    outline: none;
    /* Remove default blue outline */
}

:deep(.p-input-icon-left > i) {
    position: absolute;
    left: 0.75rem;
    /* Position icon inside input */
    top: 50%;
    transform: translateY(-50%);
    color: #6b7280;
    /* Match placeholder color */
    pointer-events: none;
    /* Prevent icon from interfering with input */
}

.add-button {
    border-radius: 6px;
    padding: 0.5rem 1rem;
    font-weight: 500;
    transition: background-color 0.2s, transform 0.1s;
    color: #35D300;
    /* Changed from #3b82f6 (blue) to green */
    border-color: #35D300;
    /* Changed from #3b82f6 (blue) to green */
}

.add-button:hover {
    transform: translateY(-1px);
    background-color: #35D300 !important;
    color: #ffffff !important;
    /* Changed from blue to green */
    /* Ensure no blue hover from PrimeVue */
    box-shadow: none !important;
    /* Override any blue shadow */
}

.card {
    width: 100%;
    padding: 1.5rem;
    background: linear-gradient(145deg, #ffffff, #f1f5f9);
    border-radius: 10px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
    transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.card:hover {
    transform: translateY(-2px);
    box-shadow: 0 6px 16px rgba(0, 0, 0, 0.1);
}

:deep(.p-datatable .p-datatable-thead > tr > th) {
    background: #f1f5f9;
    color: #1f2937;
    font-weight: 600;
    padding: 1rem 1.5rem;
    line-height: 2.5;
}

:deep(.p-datatable .p-datatable-tbody > tr) {
    transition: background-color 0.2s ease;
}

:deep(.p-datatable .p-datatable-tbody > tr:hover) {
    background-color: #f9fafb;
}

:deep(.p-datatable .p-datatable-tbody > tr > td) {
    padding: 2rem 2.5rem;
    color: #1f2937;
    line-height: 3.5;
}

h2 {
    font-size: 2.25rem;
    font-weight: 700;
    margin: 0;
    color: #1f2937;
    letter-spacing: -0.025rem;
}

.text-center {
    text-align: center;
}

.text-muted {
    color: #6b7280;
    font-style: italic;
    font-size: 0.95rem;
}

.error-message {
    margin: 1rem 0;
    border-radius: 8px;
    border-left: 4px solid #ef4444;
    background-color: #fef2f2;
}

:deep(.p-button.p-button-text) {
    margin: 0 0.25rem;
}

/* Specific override for InputText focus to remove blue */
:deep(.p-inputtext:focus) {
    border-color: #35D300 !important;
    /* Ensure green border */
    box-shadow: 0 0 0 3px rgba(53, 211, 0, 0.2) !important;
    /* Green shadow */
    outline: none !important;
    /* Remove any default blue outline */
    outline-offset: 0 !important;
    /* Ensure no offset adds blue */
}
</style>