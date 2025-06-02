<template>
  <div class="users-page">
    <div class="header-container">
      <Toolbar class="header-toolbar">
        <template #start>
          <div class="flex align-items-center">
            <h2>Users</h2>
            <span class="p-input-icon-left search-container">
              <i class="pi pi-search" />
              <InputText
                v-model="searchQuery"
                placeholder="Search users..."
                class="search-input"
              />
            </span>
          </div>
        </template>
        <template #end>
          <Button
            label="Add User"
            icon="pi pi-plus"
            class="add-button"
            @click="showAddDialog = true"
            outlined
          />
        </template>
      </Toolbar>
    </div>

    <Message
      v-if="error"
      severity="error"
      :closable="true"
      class="error-message"
    >
      {{ error }}
    </Message>

    <Message
      v-if="successMessage"
      severity="success"
      :closable="true"
      class="success-message"
      @close="successMessage = null"
    >
      Password reset done with success
    </Message>

    <Dialog v-model:visible="showAddDialog" header="Add New User" :modal="true" class="p-fluid">
      <div class="p-field">
        <label for="email">Email</label>
        <InputText id="email" v-model="newUser.email" />
      </div>
      <div class="p-field">
        <label for="username">Username</label>
        <InputText id="username" v-model="newUser.username" />
      </div>
      <div class="p-field">
        <label for="phoneNumber">Phone Number</label>
        <InputText id="phoneNumber" v-model="newUser.phoneNumber" />
      </div>
      <template #footer>
        <Button label="Cancel" icon="pi pi-times" @click="showAddDialog = false" class="p-button-text" />
        <Button label="Save" icon="pi pi-check" @click="addUser" class="p-button-text p-button-success" />
      </template>
    </Dialog>

    <div class="card">
      <DataTable
        :value="filteredUsers"
        :loading="loading"
        tableStyle="min-width: 100%"
        :showGridlines="true"
        responsiveLayout="scroll"
        class="p-datatable-sm"
      >
        <template #empty>
          <div class="text-center text-muted">No users found</div>
        </template>
        <Column field="email" header="Email" sortable style="min-width: 200px">
          <template #body="{ data }">
            {{ data.email }}
          </template>
        </Column>
        <Column
          field="username"
          header="Username"
          sortable
          style="min-width: 150px"
        />
        <Column
          field="phoneNumber"
          header="Phone Number"
          sortable
          style="min-width: 150px"
        >
          <template #body="{ data }">
            {{ data.phoneNumber || 'N/A' }}
          </template>
        </Column>
        <Column
          :exportable="false"
          header="Actions"
          style="min-width: 120px; text-align: center"
        >
          <template #body="slotProps">
            <Button
              icon="pi pi-key"
              class="p-button-rounded p-button-warning p-button-text"
              @click="resetPassword(slotProps.data)"
            />
          </template>
        </Column>
      </DataTable>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, inject } from 'vue';
import axios from 'axios';
import Button from 'primevue/button';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import InputText from 'primevue/inputtext';
import Message from 'primevue/message';
import Toolbar from 'primevue/toolbar';
import Dialog from 'primevue/dialog';

const deleteDialogRef = inject('deleteDialog');
const users = ref([]);
const loading = ref(false);
const error = ref(null);
const searchQuery = ref('');
const showAddDialog = ref(false);
const newUser = ref({
  email: '',
  username: '',
  phoneNumber: ''
});
const successMessage = ref(null);

const filteredUsers = computed(() => {
  if (!searchQuery.value) return users.value;
  const query = searchQuery.value.toLowerCase();
  return users.value.filter(
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

const addUser = async () => {
  try {
    loading.value = true;
    error.value = null;
    const token = localStorage.getItem('accessToken');

    if (!token) {
      throw new Error('No access token found');
    }

    const response = await axios.post('http://localhost:58169/api/user', newUser.value, {
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

const resetPassword = async (user) => {
  try {
    loading.value = true;
    error.value = null;
    const token = localStorage.getItem('accessToken');

    if (!token) {
      throw new Error('No access token found');
    }

    const payload = {
      email: user.email,
      username: user.username,
      phoneNumber: user.phoneNumber || ''
    };

    await axios.patch('http://localhost:58169/api/user/resetPass', payload, {
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json',
        Accept: 'application/json'
      }
    });

    successMessage.value = true;
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
  color: #3b82f6;
  border-color: #3b82f6;
}

.add-button:hover {
  transform: translateY(-1px);
  background: #e6f0ff;
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

:deep(.p-button.p-button-text.p-button-warning) {
  color: #ef4444;
}

:deep(.p-button.p-button-text.p-button-danger) {
  color: #ef4444;
}

:deep(.p-button.p-button-text) {
  margin: 0 0.25rem;
}

:deep(.p-dialog) {
  width: 30rem;
}

:deep(.p-field) {
  margin-bottom: 1rem;
}

:deep(.p-field label) {
  font-weight: 500;
  color: #1f2937;
}

:deep(.p-button-success) {
  color: #22c55e;
  border-color: #22c55e;
}

:deep(.p-button-success:hover) {
  background: #d1fae5;
}
</style>