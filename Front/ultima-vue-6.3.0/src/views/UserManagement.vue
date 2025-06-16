<template>
  <div class="users-page">
    <div class="header-container">
      <Toolbar class="header-toolbar">
        <template #start>
          <div class="flex align-items-center">
            <h2>Users</h2>
            <span class="p-input-icon-left search-container">
              <i class="pi pi-search" />
              <InputText v-model="searchQuery" placeholder="Search Users..." class="search-input" />
            </span>
          </div>
        </template>
        <template #end>
          <Button label="Add User" icon="pi pi-plus" class="add-button" @click="showAddDialog = true" outlined />
        </template>
      </Toolbar>
    </div>

    <Message v-if="error" severity="error" :closable="true" class="error-message">
      {{ error }}
    </Message>

    <Message v-if="successMessage" severity="success" :closable="true" class="success-message"
      @close="successMessage = null">
      Password reset done with success
    </Message>

    <AddUserDialog 
      :showDialog="showAddDialog" 
      :user="newUser"
      @update:showDialog="showAddDialog = $event"
      @save="addUser"
    />

    <ResetPasswordDialog
      :showDialog="showResetDialog"
      :user="selectedUser"
      @update:showDialog="showResetDialog = $event"
      @save="resetPassword"
    />

    <div class="card">
      <DataTable :value="filteredUsers" :loading="loading" tableStyle="min-width: 100%" :showGridlines="true"
        responsiveLayout="scroll" class="p-datatable-sm">
        <template #empty>
          <div class="text-center text-muted">No users found</div>
        </template>
        <Column field="email" header="Email" sortable style="min-width: 200px">
          <template #body="{ data }">
            {{ data.email }}
          </template>
        </Column>
        <Column field="username" header="Username" sortable style="min-width: 150px" />
        <Column field="phoneNumber" header="Phone Number" sortable style="min-width: 150px">
          <template #body="{ data }">
            {{ data.phoneNumber || 'N/A' }}
          </template>
        </Column>
        <Column :exportable="false" header="Reset Password"
          style="min-width: 20px; max-width: 40px; text-align: center">
          <template #body="slotProps">
            <Button icon="pi pi-key" class="p-button-rounded p-button-text reset-password-button"
              @click="openResetDialog(slotProps.data)" />
          </template>
        </Column>
      </DataTable>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import axios from 'axios';
import Button from 'primevue/button';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import InputText from 'primevue/inputtext';
import Message from 'primevue/message';
import Toolbar from 'primevue/toolbar';
import AddUserDialog from './componant/AddUserDialog.vue';
import ResetPasswordDialog from './componant/ResetPasswordDialog.vue';

const users = ref([]);
const loading = ref(false);
const error = ref(null);
const searchQuery = ref('');
const showAddDialog = ref(false);
const showResetDialog = ref(false);
const newUser = ref({
  email: '',
  username: '',
  phoneNumber: ''
});
const selectedUser = ref(null);
const successMessage = ref(null);

const filteredUsers = computed(() => {
  let filtered = users.value.filter(
    (user) => user.email?.toLowerCase() !== 'admin@example.com'
  );
  if (!searchQuery.value) return filtered;
  const query = searchQuery.value.toLowerCase();
  return filtered.filter(
    (user) =>
      user.email?.toLowerCase().includes(query) ||
      user.username?.toLowerCase().includes(query)
  );
});

const fetchUsers = async () => {
  try {
    loading.value = true;
    error.value = null;
    const token = localStorage.getItem('accessToken');

    if (!token) {
      throw new Error('No access token found');
    }

    const response = await axios.get('http://localhost:58169/api/user', {
      headers: {
        Authorization: `Bearer ${token}`,
        Accept: 'application/json'
      }
    });

    users.value = response.data;
  } catch (err) {
    console.error('Error fetching users:', err);
    error.value =
      'Failed to fetch users: ' +
      (err.response?.data?.message || err.message || 'Unknown error');
  } finally {
    loading.value = false;
  }
};

const addUser = async (userData) => {
  try {
    loading.value = true;
    error.value = null;
    const token = localStorage.getItem('accessToken');

    if (!token) {
      throw new Error('No access token found');
    }

    const response = await axios.post('http://localhost:58169/api/user', userData, {
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
        Accept: 'application/json'
      }
    });

    users.value = [...users.value, response.data];
    showAddDialog.value = false;
    newUser.value = {
      email: '',
      username: '',
      phoneNumber: ''
    };
  } catch (err) {
    console.error('Error adding user:', err);
    error.value =
      'Failed to add user: ' +
      (err.response?.data?.message || err.message || 'Unknown error');
  } finally {
    loading.value = false;
  }
};

const openResetDialog = (user) => {
  selectedUser.value = { ...user };
  showResetDialog.value = true;
};

const resetPassword = async (userData) => {
  try {
    loading.value = true;
    error.value = null;
    const token = localStorage.getItem('accessToken');

    if (!token) {
      throw new Error('No access token found');
    }

    const payload = {
      email: userData.email,
      username: userData.username,
      phoneNumber: userData.phoneNumber || ''
    };

    await axios.patch('http://localhost:58169/api/user/resetPass', payload, {
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
        Accept: 'application/json'
      }
    });

    successMessage.value = true;
    showResetDialog.value = false;
    setTimeout(() => (successMessage.value = null), 3000); // Auto-close after 3 seconds
  } catch (err) {
    console.error('Error resetting password:', err);
    error.value =
      'Failed to reset password: ' +
      (err.response?.data?.message || err.message || 'Unknown error');
  } finally {
    loading.value = false;
  }
};

onMounted(fetchUsers);
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
}

.search-input {
  width: 250px;
  border-radius: 6px;
  padding: 0.5rem 0.5rem 0.5rem 2rem;
}

.search-input:focus {
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.2);
}

.add-button {
  border-radius: 6px;
  padding: 0.5rem 1rem;
  font-weight: 500;
  transition: background-color 0.2s, transform 0.1s;
  color: #35D300 !important;
  background-color: transparent !important;
  border-color: #35D300 !important;
}

.add-button:hover {
  transform: translateY(-1px);
  background-color: #35D300 !important;
  color: #ffffff !important;
  box-shadow: none !important;
}

.add-button:disabled {
  color: #d1d5db;
  border-color: #d1d5db;
  cursor: not-allowed;
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
  padding: 1rem;
}

:deep(.p-datatable .p-datatable-tbody > tr > td) {
  padding: 0.75rem 1rem;
  color: #1f2937;
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

.success-message {
  margin: 1rem 0;
  border-radius: 8px;
  border-left: 4px solid #22c55e;
  background-color: #d1fae5;
}

:deep(.p-button.p-button-text.reset-password-button) {
  color: #35D300 !important;
}

:deep(.p-button.p-button-text.reset-password-button:hover) {
  color: #ffffff !important;
  background-color: #35D300 !important;
  box-shadow: 0 2px 4px rgba(53, 211, 0, 0.3) !important;
}

:deep(.p-button.p-button-text.p-button-warning) {
  color: #ef4444;
}

:deep(.p-button.p-button-text.p-button-danger) {
  color: #ef4444;
}

:deep(.p-button.p-button-text) {
  margin: 0 0.25rem;
}
</style>