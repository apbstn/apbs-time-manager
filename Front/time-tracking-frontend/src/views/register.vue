<script setup>
import { ref } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import api from '@/api'; // Axios instance

const router = useRouter();
const route = useRoute();

const email = ref(route.query.email || '');
const username = ref(route.query.username || '');
const phoneNumber = ref(route.query.phoneNumber || '');
const tenantId = ref(route.query.tenantId || '');
const password = ref('');
const errorMessage = ref('');

const submitForm = async () => {
  errorMessage.value = ''; // Clear previous errors

  if (!password.value) {
    errorMessage.value = 'Password is required';
    console.warn('Password field is empty');
    return;
  }

  const token = localStorage.getItem('inviteToken');
  if (!token) {
    errorMessage.value = 'No invitation token found';
    console.warn('inviteToken missing in localStorage');
    return;
  }

  try {
    console.log('Starting registration for:', email.value);

    // Step 1: Register the user
    console.log('Calling POST /api/auth/register');
    const registerResponse = await api.post('/api/auth/register', {
      email: email.value,
      username: username.value,
      phoneNumber: phoneNumber.value,
      password: password.value
    });
    console.log('Register response:', registerResponse.data);
    localStorage.setItem('email', email.value);
    // Step 2: Auto-login
    console.log('Calling POST /api/auth/login');
    const loginResponse = await api.post('/api/auth/login', {
      email: email.value,
      password: password.value
    }); 
    localStorage.setItem('username', registerResponse.data.username)
    localStorage.setItem('accessToken', registerResponse.data.accessToken);
    console.log('accessToken : ', loginResponse.data.accessToken);
    console.log('Login response:', registerResponse.data);

    const loginAccessToken = localStorage.getItem('accessToken');

    if (!loginAccessToken) {
      throw new Error('Invalid login response: missing accessToken or role');
    }
    // Step 3: Confirm invitation
    console.log('Calling POST /api/auth/invite/confirmation');
    const eemail = localStorage.getItem('email');
    const tooken = localStorage.getItem('inviteToken');
    const daata = localStorage.getItem('inviteData');
    const confirmResponse = await api.post('/api/auth/invite/confirmation', {
      email: eemail,
      token: tooken,
      tenantId: daata
    }, {
      headers: {
        Authorization: `Bearer ${localStorage.getItem('accessToken')}`
      }
    });
    console.log('Confirmation response:', confirmResponse.data);
    console.log("token : ", confirmResponse.data.token)

    const { accessToken: confirmAccessToken } = confirmResponse.data;
    const toooken = confirmResponse.data.accessToken;
    const User = confirmResponse.data.role;
    if (!confirmAccessToken) {
      throw new Error('Invalid confirmation response: missing accessToken');
    }

    // Store tokens and data
    console.log('Storing tokens in localStorage');
    localStorage.removeItem('accessToken');
    localStorage.removeItem('role');
    localStorage.removeItem('inviteToken');
    localStorage.removeItem('inviteData');
    localStorage.setItem('accessToken', toooken);
    localStorage.setItem('role', User);


    // Redirect to /home
    console.log('Redirecting to /home');
    window.location.href = '/home';
  } catch (error) {
    const message = error.response?.data?.message || error.message || 'Registration failed';
    errorMessage.value = message;
    console.error('Registration error:', {
      message,
      status: error.response?.status,
      data: error.response?.data,
      error
    });
  }
};
</script>

<template>
  <div class="register-container">
    <h2>Register</h2>
    <form @submit.prevent="submitForm">
      <div class="form-group">
        <label for="email">Email</label>
        <input id="email" v-model="email" type="text" readonly="readonly" placeholder="Email" />
      </div>
      <div class="form-group">
        <label for="username">Username</label>
        <input id="username" v-model="username" type="text" readonly="readonly" placeholder="Username" />
      </div>
      <div class="form-group">
        <label for="phoneNumber">Phone Number</label>
        <input id="phoneNumber" v-model="phoneNumber" type="text" readonly="readonly" placeholder="Phone Number" />
      </div>
      <div class="form-group">
        <label for="password">Password</label>
        <input id="password" v-model="password" type="password" placeholder="Enter password" required />
      </div>
      <div v-if="errorMessage" class="error">{{ errorMessage }}</div>
      <button type="submit">Register</button>
    </form>
  </div>
</template>

<style scoped>
.register-container {
  max-width: 400px;
  margin: 50px auto;
  padding: 20px;
  border: 1px solid #ccc;
  border-radius: 5px;
}

.form-group {
  margin-bottom: 15px;
}

label {
  display: block;
  margin-bottom: 5px;
}

input {
  width: 100%;
  padding: 8px;
  border: 1px solid #ccc;
  border-radius: 4px;
}

input[readonly] {
  background-color: #f0f0f0;
  cursor: not-allowed;
}

button {
  width: 100%;
  padding: 10px;
  background-color: #3498db;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

button:hover {
  background-color: #2980b9;
}

.error {
  color: red;
  margin-bottom: 15px;
}
</style>