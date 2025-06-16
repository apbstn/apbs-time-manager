<template>
  <div class="login-container">
    <h2>Login</h2>
    <form @submit.prevent="login" class="login-form">
      <input v-model="email" type="email" placeholder="Email" required />
      <input v-model="password" type="password" placeholder="Password" required />
      <button type="submit">Login</button>
    </form>
    <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/api'

const email = ref('')
const password = ref('')
const errorMessage = ref('')
const router = useRouter()

const login = async () => {
  try {
    console.log('Sending login data:', email.value, password.value)

    const response = await api.post('/api/auth/login', {
      email: email.value,
      
      password: password.value
    }, {
      headers: {
        'Content-Type': 'application/json',
        'Accept': '*/*'
      }
    })
    localStorage.setItem('email', email.value);

    // Check if token exists in response
    if (response.data && response.data.token) {
          
          // Store tokens in localStorage
          localStorage.setItem("accessToken", response.data.token.result);

      localStorage.setItem('username', response.data.username)
     

      // Store tenants (check for both 'tenants' and 'lisTen' to handle potential naming)
      
      // Set Authorization header for future API calls
      api.defaults.headers.common['Authorization'] = `Bearer ${response.data.accessToken}`

      // Redirect to Switch page to display tenants
      router.push('/phone')
    } else {
      console.error('No access token found in response!')
      errorMessage.value = 'Login failed: No token received.'
    }
  } catch (error) {
    console.error('Login Error:', error.response?.data || error.message)
    errorMessage.value = error.response?.data?.message || 'Invalid credentials!'
  }
}
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

.error-message {
  margin-top: 10px;
  color: red;
  font-size: 14px;
}
</style>