<template>
    <h2 style="font-size: 22px; color: #6B7280;">List Of Tenants</h2>
    <div class="users-page">
        <!-- Error Message -->
        <Message v-if="error" severity="error" :closable="true" class="error-message">
            {{ error }}
        </Message>

        <!-- Success Message -->
        <Message v-if="successMessage" severity="success" :closable="true" class="success-message"
            @close="successMessage = null">
            {{ successMessage }}
        </Message>

        <!-- Add Tenant Button and Search -->
        <div class="header-container">
            <Toolbar class="header-toolbar">
                <template #start>
                    <div class="flex align-items-center">
                        <h3>Search Tenant : </h3>
                        <span class="p-input-icon-left search-container">
                            <i class="pi pi-search" />
                            <InputText v-model="searchQuery" class="search-input" />
                        </span>
                    </div>
                </template>
                <template #end>
                    <Button label="Add Tenant" icon="pi pi-plus" class="add-button" @click="openAddPopup" outlined
                        style="border-color: #35D300; color: #35D300;" v-tooltip.top="{
                value: 'Add Tenant',
                pt: {
                  arrow: {
                    style: {
                      borderBottomColor: '#000000',
                    },
                  },
                  text: '!bg-black !text-white !font-medium',
                }
              }"/>
                </template>
            </Toolbar>
        </div>

        <!-- Add Tenant Dialog -->
        <AddTenantDialog :showDialog="isAdding" :tenant="newTenantData" :usersList="usersList"
            @update:showDialog="isAdding = $event" @save="saveNewTenant" />

        <!-- Edit Tenant Dialog -->
        <EditTenantDialog :showDialog="showEditDialog" :tenant="selectedTenant"
            @update:showDialog="showEditDialog = $event" @save="editTenant" />

        <!-- Delete Tenant Dialog -->
        <DeleteTenantDialog :showDialog="showDeleteDialog" :tenant="selectedTenant"
            @update:showDialog="showDeleteDialog = $event" @save="deleteTenant" />

        <div class="card">
            <DataTable :value="filteredTenants" :loading="loading" tableStyle="min-width: 100%" :showGridlines="true"
                responsiveLayout="scroll" class="p-datatable-sm">
                <template #empty>
                    <div class="text-center text-muted">No tenants found</div>
                </template>
                <Column field="tenantname" header="Tenant Name" sortable style="width: 24.75%;"></Column>
                <Column field="email" header="Email" sortable style="width: 24.75%;"></Column>
                <Column field="username" header="Username" sortable style="width: 24.75%;"></Column>
                <Column field="phonenumber" header="Phone Number" sortable style="width: 24.75%;">
                    <template #body="{ data }">
                        {{ data.phonenumber || 'N/A' }}
                    </template>
                </Column>
                <Column :exportable="false" header="Actions" style="width: 1%; text-align: center">
                    <template #body="slotProps">
                        <div class="actions-container">
                            <Button icon="pi pi-pencil" class="add-button"
                                @click="openEditDialog(slotProps.data)" v-tooltip.top="{
                value: 'Edit Tenant',
                pt: {
                  arrow: {
                    style: {
                      borderBottomColor: '#000000',
                    },
                  },
                  text: '!bg-black !text-white !font-medium',
                }
              }"/>
                            <Button icon="pi pi-trash" class="add-button1"
                                @click="openDeleteDialog(slotProps.data)" v-tooltip.top="{
                value: 'Delete Tenant',
                pt: {
                  arrow: {
                    style: {
                      borderBottomColor: '#000000',
                    },
                  },
                  text: '!bg-black !text-white !font-medium',
                }
              }"/>
                        </div>
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
import EditTenantDialog from './componant/EditTenantDialog.vue';
import DeleteTenantDialog from './componant/DeleteTenantDialog.vue';

const tenants = ref([]);
const loading = ref(false);
const error = ref(null);
const isAdding = ref(false);
const searchQuery = ref('');
const newTenantData = ref({
    tenantname: '',
    userId: null
});
const usersList = ref([]);
const showEditDialog = ref(false);
const showDeleteDialog = ref(false);
const selectedTenant = ref(null);
const successMessage = ref(null);

const filteredTenants = computed(() => {
    if (!searchQuery.value) return tenants.value;
    const query = searchQuery.value.toLowerCase();
    return tenants.value.filter(
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

        tenants.value = response.data.items.map(item => ({
            id: item.id,
            tenantname: item.tenantName || 'N/A',
            email: item.email || 'N/A',
            username: item.username || 'N/A',
            phonenumber: item.phoneNumber || 'N/A'
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

        tenants.value.push({
            id: response.data.id,
            tenantname: response.data.tenantName || 'N/A',
            email: response.data.email || 'N/A',
            username: response.data.username || 'N/A',
            phonenumber: response.data.phoneNumber || 'N/A'
        });

        isAdding.value = false;
        successMessage.value = `Tenant ${tenantData.tenantname} added successfully`;
        setTimeout(() => (successMessage.value = null), 7000);
    } catch (error) {
        console.error('Error adding tenant:', error.response || error);
        error.value = 'Failed to add tenant: ' + (error.response?.data?.message || error.message || 'Unknown error');
    } finally {
        loading.value = false;
    }
};

const openEditDialog = (tenant) => {
    selectedTenant.value = { ...tenant };
    showEditDialog.value = true;
};

const editTenant = async (tenantData) => {
    try {
        loading.value = true;
        error.value = null;
        const token = localStorage.getItem('accessToken');

        if (!token) {
            throw new Error('No access token found');
        }

        const tenantId = selectedTenant.value.id;
        console.log('Editing tenant with ID:', tenantId);
        const payload = {
            Name: tenantData.tenantname,
            Email: selectedTenant.value.email || 'N/A',
            Username: selectedTenant.value.username || 'N/A',
            PhoneNumber: selectedTenant.value.phonenumber || 'N/A'
        };

        const response = await api.put(`/api/tenant/${tenantId}`, payload, {
            headers: {
                Authorization: `Bearer ${token}`,
                'Content-Type': 'application/json',
                Accept: 'application/json'
            }
        });

        const index = tenants.value.findIndex(tenant => tenant.id === tenantId);
        if (index !== -1) {
            tenants.value[index] = {
                ...tenants.value[index],
                tenantname: response.data.tenantName || tenantData.tenantname,
                email: response.data.email || selectedTenant.value.email,
                username: response.data.username || selectedTenant.value.username,
                phonenumber: response.data.phoneNumber || selectedTenant.value.phonenumber
            };
        }

        showEditDialog.value = false;
        successMessage.value = `Tenant ${tenantData.tenantname} updated successfully`;
        setTimeout(() => (successMessage.value = null), 7000);
    } catch (err) {
        console.error('Error editing tenant:', err.response || err);
        error.value =
            'Failed to edit tenant: ' +
            (err.response?.data?.message || err.message || 'Unknown error');
    } finally {
        loading.value = false;
    }
};

const openDeleteDialog = (tenant) => {
    selectedTenant.value = { ...tenant };
    showDeleteDialog.value = true;
};

const deleteTenant = async () => {
    try {
        loading.value = true;
        error.value = null;
        const token = localStorage.getItem('accessToken');

        if (!token) {
            throw new Error('No access token found');
        }

        const tenantId = selectedTenant.value.id;
        console.log("Tenant ID:", tenantId);
        await api.delete(`/api/tenant/${tenantId}`, {
            headers: {
                Authorization: `Bearer ${token}`,
                Accept: 'application/json'
            }
        });

        const index = tenants.value.findIndex(tenant => tenant.id === tenantId);
        if (index !== -1) {
            tenants.value.splice(index, 1);
        }

        showDeleteDialog.value = false;
        successMessage.value = `Tenant ${selectedTenant.value.tenantname} deleted successfully`;
        setTimeout(() => (successMessage.value = null), 7000);
    } catch (err) {
        console.error('Error deleting tenant:', err.response || err);
        error.value =
            'Failed to delete tenant: ' +
            (err.response?.data?.message || err.message || 'Unknown error');
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
    min-height: auto !important;
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
    border-color: #35D300 !important;
}

.search-input {
    width: 250px;
    border-radius: 6px;
    padding: 0.5rem 0.5rem 0.5rem 2rem;
    border-bottom-color: #35D300 !important;
}

.search-input:focus {
    border-color: #35D300 !important;
}

:deep(.p-input-icon-left > i) {
    position: absolute;
    left: 0.75rem;
    top: 50%;
    transform: translateY(-50%);
    color: #6b7280;
    pointer-events: none;
}

.add-button {
    border-radius: 6px;
    padding: 0.5rem 1rem;
    font-weight: 500;
    transition: background-color 0.2s, transform 0.1s;
    color: #35D300 !important;
    border: 1px solid #35D300 !important;
    background-color: rgba(255, 255, 255, 0);
    line-height: 1.5;
}

.add-button:hover {
    transform: translateY(-1px);
    background-color: #35D300 !important;
    color: white !important;
    border: 1px solid white !important;
}

.add-button1 {
    border-radius: 6px;
    padding: 0.5rem 1rem;
    font-weight: 500;
    transition: background-color 0.2s, transform 0.1s;
    color: #FF0000 !important;
    border: 1px solid #FF0000 !important;
    background-color: rgba(255, 255, 255, 0);
    line-height: 1.5;
}

.add-button1:hover {
    transform: translateY(-1px);
    background-color: #FF0000 !important;
    color: white !important;
    border: 1px solid white !important;
}

:deep(.p-button.add-button.pi-pencil) {
    padding: 0.5rem;
    border: 1px solid #35D300 !important;
    line-height: 1.5;
}

:deep(.p-button.add-button.pi-pencil:hover) {
    background-color: #35D300 !important;
    color: white !important;
    border: 1px solid white !important;
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
    font-size: 2.1rem;
    font-weight: 450;
    margin: 0;
    color: #000000;
    letter-spacing: -0.025rem;
}

h3 {
    font-size: 1.25rem;
    font-weight: 150;
    margin: 0;
    color: #000000;
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

.success-message {
    margin: 1rem 0;
    border-radius: 8px;
    border-left: 4px solid #22c55e;
    background-color: #d1fae5;
}

:deep(.p-button.p-button-text.delete-button) {
    color: #35D300 !important;
    margin-left: 0.25rem;
}

:deep(.p-button.p-button-text.delete-button:hover) {
    color: #ffffff !important;
    background-color: #35D300 !important;
    box-shadow: 0 2px 4px rgba(239, 68, 68, 0.3) !important;
}

.actions-container {
    display: flex;
    gap: 0.5rem;
    white-space: nowrap;
    justify-content: center;
}
</style>