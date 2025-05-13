<template>
    <div class="card">
        <!-- Add Tenant Button -->
        <button @click="openAddPopup" style="margin-bottom: 20px;">Add Tenant</button>

        <DataTable :value="users" tableStyle="min-width: 50rem">
            <Column field="code" header="Code"></Column>
            <Column field="tenantname" header="Tenant Name"></Column>
            <Column field="email" header="Email"></Column>
            <Column field="username" header="Username"></Column>
            <Column field="phonenumber" header="Phone Number"></Column>
            <Column :exportable="false" style="min-width: 12rem">
                <template #body="slotProps">
                    <Button icon="pi pi-pencil" outlined rounded class="mr-2" @click="openEditPopup(slotProps.data)" />
                    <Button icon="pi pi-trash" outlined rounded severity="danger" @click="deleteUser(slotProps.data)" />
                </template>
            </Column>
        </DataTable>

        <!-- Edit Tenant Popup -->
        <div v-if="isEditing" class="popup">
            <div class="popup-content">
                <h3>Edit Tenant</h3>
                <input v-model="editUserData.tenantname" placeholder="Tenant Name" />
                <input v-model="editUserData.email" placeholder="Email" />
                <input v-model="editUserData.username" placeholder="Username" />
                <input v-model="editUserData.phonenumber" placeholder="Phone Number" />
                <button @click="saveEditedUser">Save</button>
                <button @click="closeEditPopup">Cancel</button>
            </div>
        </div>

        <!-- Add Tenant Popup -->
        <div v-if="isAdding" class="popup">
            <div class="popup-content">
                <h3>Add Tenant</h3>
                <input v-model="newTenantData.tenantname" placeholder="Tenant Name" />
                <input v-model="newTenantData.email" placeholder="Email" />
                <input v-model="newTenantData.username" placeholder="Username" />
                <input v-model="newTenantData.phonenumber" placeholder="Phone Number" />
                <button @click="saveNewTenant">Save</button>
                <button @click="closeAddPopup">Cancel</button>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '../api'

const users = ref([])
const isEditing = ref(false)
const isAdding = ref(false)
const editUserData = ref({})
const newTenantData = ref({
    code: '',
    tenantname: '',
    email: '',
    username: '',
    phonenumber: ''
})

// Fetch tenants from backend
const fetchTenants = async () => {
    try {
        const response = await api.get('/api/tenant')
        users.value = response.data.map(t => ({
            code: t.code || t.T_CODE,
            tenantname: t.tenantName || t.T_NAME,
            email: t.email || t.Email,
            username: t.username || t.Username,
            phonenumber: t.phoneNumber || t.PhoneNumber
        }))
    } catch (error) {
        console.error('Error fetching tenants:', error.response?.data || error.message)
    }
}

// --- Edit logic ---
const openEditPopup = (user) => {
    isEditing.value = true
    editUserData.value = { ...user }
}

const closeEditPopup = () => {
    isEditing.value = false
    editUserData.value = {}
}

const saveEditedUser = async () => {
    try {
        await api.put(`/api/tenant/${editUserData.value.code}`, {
            tenantName: editUserData.value.tenantname,
            email: editUserData.value.email,
            username: editUserData.value.username,
            phoneNumber: editUserData.value.phonenumber
        })

        const index = users.value.findIndex(u => u.code === editUserData.value.code)
        if (index !== -1) {
            users.value[index] = { ...editUserData.value }
        }

        closeEditPopup()
    } catch (error) {
        console.error('Error updating tenant:', error.response?.data || error.message)
    }
}

// --- Add logic ---
const openAddPopup = () => {
    isAdding.value = true
    newTenantData.value = {
        tenantname: '',
        email: '',
        username: '',
        phonenumber: ''
    }
}

const closeAddPopup = () => {
    isAdding.value = false
}

const saveNewTenant = async () => {
    try {
        const response = await api.post('/api/tenant', {
            code : newTenantData.value.code,
            tenantName: newTenantData.value.tenantname,
            email: newTenantData.value.email,
            username: newTenantData.value.username,
            phoneNumber: newTenantData.value.phonenumber
        })

        users.value.push({
            code: response.data.code || response.data.T_CODE,
            tenantname: response.data.tenantName || response.data.T_NAME,
            email: response.data.email || response.data.Email,
            username: response.data.username || response.data.Username,
            phonenumber: response.data.phoneNumber || response.data.PhoneNumber
        })

        closeAddPopup()
    } catch (error) {
        console.error('Error adding tenant:', error.response?.data || error.message)
    }
}

const deleteUser = (user) => {
    console.log('Delete:', user)
    // TODO: Implement delete logic here
}

onMounted(() => {
    fetchTenants()
})
</script>


<style>
/* Layout for the entire page */
.wrapper {
    display: flex;
    min-height: 100vh;
}

/* Navbar styles */
.navbar {
    width: 250px;
    background-color: #2c3e50;
    color: white;
    padding: 30px 20px;
    height: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
    box-shadow: 2px 0 8px rgba(0, 0, 0, 0.1);
}

.navbar h3 {
    margin-bottom: 40px;
    font-size: 24px;
    text-transform: uppercase;
    letter-spacing: 1px;
}

.navbar ul {
    list-style-type: none;
    padding: 0;
    width: 100%;
}

.navbar ul li {
    width: 100%;
    margin: 10px 0;
}

.navbar ul li a {
    display: block;
    width: 100%;
    padding: 12px;
    background-color: #34495e;
    color: white;
    text-decoration: none;
    text-align: center;
    font-size: 16px;
    border-radius: 4px;
    transition: background-color 0.3s ease;
}

.navbar ul li a:hover {
    background-color: #1abc9c;
}

/* Main content styling */
.container {
    flex-grow: 1;
    max-width: 900px;
    margin: auto;
    padding: 40px;
    background-color: #f4f6f9;
    border-radius: 8px;
    box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
}

h2 {
    font-size: 28px;
    color: #2c3e50;
    margin-bottom: 20px;
    text-transform: uppercase;
    letter-spacing: 1px;
}

/* Input and button styling */
input {
    margin: 10px;
    padding: 12px;
    font-size: 16px;
    border: 1px solid #ccc;
    border-radius: 5px;
    width: 100%;
    box-sizing: border-box;
}

input:focus {
    border-color: #1abc9c;
    outline: none;
}

button {
    padding: 12px 20px;
    background-color: #1abc9c;
    color: white;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    font-size: 16px;
    transition: background-color 0.3s ease;
    width: 100%;
    margin-top: 20px;
}

button:hover {
    background-color: #16a085;
}

/* Table styling */
table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 20px;
    background-color: white;
    border-radius: 8px;
    overflow: hidden;
}

th,
td {
    padding: 15px;
    text-align: left;
    font-size: 16px;
    border-bottom: 1px solid #ddd;
}

th {
    background-color: #34495e;
    color: white;
}

td {
    background-color: #f9f9f9;
}

td button {
    padding: 8px 16px;
    background-color: #e67e22;
    color: white;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    font-size: 14px;
    margin-right: 10px;
    transition: background-color 0.3s ease;
}

td button:hover {
    background-color: #d35400;
}

/* Popup styling */
/* ... (use your existing styles here) ... */

.popup {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.5);
    display: flex;
    align-items: center;
    justify-content: center;
}

.popup-content {
    background: white;
    padding: 30px;
    border-radius: 8px;
    box-shadow: 0 0 20px rgba(0, 0, 0, 0.2);
    text-align: center;
    max-width: 500px;
    width: 100%;
}

.popup h3 {
    font-size: 24px;
    color: #2c3e50;
    margin-bottom: 20px;
}

.popup input {
    margin: 10px 0;
    padding: 12px;
    font-size: 16px;
    border: 1px solid #ccc;
    border-radius: 5px;
    width: 100%;
    box-sizing: border-box;
}

.popup button {
    padding: 12px 20px;
    margin-top: 10px;
    background-color: #1abc9c;
    color: white;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    font-size: 16px;
}

.popup button:hover {
    background-color: #16a085;
}
</style>