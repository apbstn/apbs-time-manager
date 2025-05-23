<template>
  <div class="register-container">
    <h2>Register</h2>
    <form @submit.prevent="register" class="register-form">
      <input v-model="username" type="text" placeholder="Username" required />
      <input v-model="email" type="email" placeholder="Email" required />
      <input v-model="phoneNumber" type="tel" placeholder="Phone Number" required />
      <input v-model="password" type="password" placeholder="Password" required />
      <input v-model="confirmPassword" type="password" placeholder="Confirm Password" required />
      <button type="submit">Register</button>
    </form>
    <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/api'

const username = ref('')
const email = ref('')
const phoneNumber = ref('')
const password = ref('')
const confirmPassword = ref('')
const errorMessage = ref('')
const router = useRouter()

const register = async () => {
  // Validate password match
  if (password.value !== confirmPassword.value) {
    errorMessage.value = 'Passwords do not match!'
    return
  }

  try {
    console.log('Sending registration data:', username.value, email.value, phoneNumber.value, password.value)

    const response = await api.post('/api/auth/register', {
      username: username.value,
      email: email.value,
      phoneNumber: phoneNumber.value,
      password: password.value
    }, {
      headers: {
        'Content-Type': 'application/json',
        'Accept': '*/*'
      }
    })

    // Check if registration was successful
    if (response.data && response.data.accessToken) {
      localStorage.setItem('accessToken', response.data.accessToken)
      localStorage.setItem('role', response.data.role)
      localStorage.setItem('username', username.value)
      localStorage.setItem('email', email.value)

      // Set Authorization header for future API calls
      api.defaults.headers.common['Authorization'] = `Bearer ${response.data.accessToken}`

      // Backend does not return tenants, so store empty array
      localStorage.setItem('tenants', JSON.stringify([]))

      // Redirect to home page after successful registration
      router.push('/home')
    } else {
      console.error('No access token found in response!')
      errorMessage.value = 'Registration failed: No token received.'
    }
  } catch (error) {
    console.error('Registration Error:', error.response?.data || error.message)
    errorMessage.value = error.response?.status === 401 
      ? 'Email already registered!'
      : error.response?.data?.message || 'Registration failed!'
  }
}
</script>

<style scoped>
.register-container {
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

.register-form {
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
  padding: 10px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 16px;
  cursor: pointer;
}

button:hover {
  background-color: #0056b3;
}

.error-mi {
  margin-top: 10px;
  color: red;
  font-size: 14px;
}
</style>