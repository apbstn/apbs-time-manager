<template>
  <div class="login-container">
    <h2>Login</h2>
    <form @submit.prevent="login" class="login-form">
      <input v-model="email" type="email" placeholder="Email" required />
      <input v-model="password" type="password" placeholder="Password" required />
      <button type="submit" :disabled="isLoading">
        {{ isLoading ? 'Loading...' : 'Login' }}
      </button>
    </form>
    <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import api from '@/api';

const email = ref('');
const password = ref('');
const errorMessage = ref('');
const isLoading = ref(false);
const tenantName = ref('');
const router = useRouter();

// Decode JWT token
const decodeJwt = (token) => {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => {
          return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        })
        .join('')
    );
    return JSON.parse(jsonPayload);
  } catch (error) {
    console.error('Error decoding JWT:', error);
    return null;
  }
};

// Fetch and set tenant name
const nametenant = async () => {
  try {
    isLoading.value = true;
    errorMessage.value = '';
    console.log('Starting nametenant at:', new Date().toLocaleString());

    const token = localStorage.getItem('accessToken');
    if (!token) {
      errorMessage.value = 'No token found';
      console.error('nametenant: No token found');
      return false;
    }

    // Decode token
    const decoded = decodeJwt(token);
    console.log('Decoded token:', decoded);
    if (!decoded) {
      errorMessage.value = 'Invalid token';
      console.error('nametenant: Invalid token');
      return false;
    }

    // Extract tenantId (configurable key)
    const tenantIdKey = 'tenant_id'; // Adjust to 'sub', 'tid', etc., if needed
    const tenantId = decoded[tenantIdKey];
    if (!tenantId) {
      errorMessage.value = `Missing ${tenantIdKey} in token`;
      console.error(`nametenant: Missing ${tenantIdKey} in token`, decoded);
      return false;
    }

    // Retrieve tenant list from localStorage
    const tenantList = JSON.parse(localStorage.getItem('tenants') || '[]');
    console.log('Tenant list:', tenantList);
    if (!tenantList || tenantList.length === 0) {
      errorMessage.value = 'No tenants found in localStorage';
      console.error('nametenant: No tenants found in localStorage');
      return false;
    }

    // Find tenant by matching tenantId with id
    const tenant = tenantList.find((t) => t.id === tenantId);
    if (!tenant) {
      errorMessage.value = `Tenant not found for tenantId: ${tenantId}`;
      console.error(`nametenant: Tenant not found for tenantId: ${tenantId}`);
      return false;
    }

    tenantName.value = tenant.name || 'Unknown Tenant';
    localStorage.setItem('Name_of_tenant', tenant.name);
    console.log('nametenant: Set Name_of_tenant to', tenant.name);
    return true;
  } catch (error) {
    console.error('nametenant Error:', error);
    errorMessage.value = 'Failed to fetch tenant name';
    return false;
  } finally {
    isLoading.value = false;
    console.log('nametenant: Completed, isLoading set to false');
  }
};

const login = async () => {
  try {
    isLoading.value = true;
    errorMessage.value = '';
    console.log('Sending login data:', email.value, password.value);

    const response = await api.post('/api/auth/login', {
      email: email.value,
      password: password.value,
    }, {
      headers: {
        'Content-Type': 'application/json',
        'Accept': '*/*',
      },
    });

    console.log('Login response:', response.data);

    localStorage.setItem('email', email.value);

    // Check if token exists in response
    if (response.data && response.data.accessToken) {
      localStorage.setItem('accessToken', response.data.accessToken);
      localStorage.setItem('role', response.data.role || '');
      localStorage.setItem('username', response.data.username || '');

      // Store tenants (check for both 'tenants' and 'listTen')
      if (response.data.tenants) {
        localStorage.setItem('tenants', JSON.stringify(response.data.tenants));
        console.log('Stored tenants:', response.data.tenants);
      } else if (response.data.listTen) {
        localStorage.setItem('tenants', JSON.stringify(response.data.listTen));
        console.log('Stored listTen:', response.data.listTen);
      } else {
        console.warn('No tenants or listTen found in response, storing empty array');
        localStorage.setItem('tenants', JSON.stringify([]));
      }

      // Set flag for tracking reminder popup
      localStorage.setItem('showTrackingReminder', 'true');

      // Set Authorization header for future API calls
      api.defaults.headers.common['Authorization'] = `Bearer ${response.data.accessToken}`;

      // Call nametenant and wait for it to complete
      const tenantSuccess = await nametenant();
      console.log('nametenant result:', tenantSuccess);

      // Redirect to home page
      console.log('Redirecting to /home at:', new Date().toLocaleString());
      window.location.href = '/home';
    } else {
      console.error('No access token found in response!');
      errorMessage.value = 'Login failed: No token received.';
    }
  } catch (error) {
    console.error('Login Error:', error.response?.data || error.message);
    errorMessage.value = error.response?.data?.message || 'Invalid credentials!';
  } finally {
    isLoading.value = false;
    console.log('login: Completed, isLoading set to false');
  }
};
</script>

<style scoped>
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  flex-direction: column;
  background-color: #f5f5f5;
}

h2 {
  margin-bottom: 20px;
  font-size: 24px;
  color: #333;
}

.login-form {
  display: flex;
  flex-direction: column;
  width: 300px;
  padding: 20px;
  background-color: #fff;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

input {
  padding: 10px;
  margin-bottom: 15px;
  border: 1px solid #ccc;
  border-radius: 4px;
  font-size: 16px;
}

button {
  padding: 0.7rem 1.5rem;
  font-size: 1.2rem;
  cursor: pointer;
  background-color: #007bff00 !important;
  color: #35D300 !important;
  border: 1px solid #35D300 !important;
  border-radius: 10px;
}

button:hover {
  background-color: #35D300 !important;
  color: white !important;
}

button:disabled {
  background-color: #cccccc !important;
  color: #666666 !important;
  cursor: not-allowed;
}

.error-message {
  margin-top: 10px;
  color: red;
  font-size: 14px;
}
</style>