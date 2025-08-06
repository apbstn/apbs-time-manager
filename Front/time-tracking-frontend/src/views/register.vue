<template>
    <div class="container" :class="{ 'right-panel-active': isSignUp }">
        <div class="sign-up">
            <form @submit.prevent="submitForm" class="form">
                <h1>Sign Up</h1>
                <input type="email" v-model="email" placeholder="Email" required disabled>
                <input type="text" v-model="username" placeholder="Name" required disabled>
                <input type="tel" v-model="phoneNumber" placeholder="Phone Number" required>
                <input type="password" v-model="password" placeholder="Password" required>
                <button type="submit" :disabled="isLoading" class="add-button">
                    {{ isLoading ? 'Loading...' : 'Sign Up' }}
                </button>
                <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
            </form>
        </div>
        <div class="sign-in">
            <form @submit.prevent="login" class="form">
                <h1>If you already have an account :</h1>
                <!-- <input type="email" v-model="signInEmail" placeholder="Email" required>
                <input type="password" v-model="signInPassword" placeholder="Password" required>
                <a href="#">Forget your Password?</a> -->
                <button type="button" @click="router.push('/login')" :disabled="isLoading" class="add-button">
                    {{ isLoading ? 'Loading...' : 'Sign In' }}
                </button>
                <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
            </form>
        </div>
        <div class="overlay-container">
            <div class="overlay">
                <div class="overlay-left">
                    <h1>Hello Friend!</h1>
                    <p>To keep connected with us please login with your personal info</p>
                    <button class="ghost" @click="isSignUp = false" :disabled="isLoading">Sign In</button>
                </div>
                <div class="overlay-right">
                    <h1>Welcome Back!</h1>
                    <p>Enter your personal details and start journey with us</p>
                    <button class="ghost" @click="isSignUp = true" :disabled="isLoading">Sign Up</button>
                </div>
            </div>
        </div>
    </div>
    <div class="layout-footer">
        <span class="font-medium text-lg text-muted-color">AP Business Soft © Copyright 2025</span>
        <div class="flex gap-2">
            <Button icon="pi pi-github" rounded variant="text" severity="secondary"></Button>
            <Button icon="pi pi-facebook" rounded variant="text" severity="secondary"></Button>
            <Button icon="pi pi-twitter" rounded variant="text" severity="secondary"></Button>
        </div>
    </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import api from '@/api'; // Axios instance

const router = useRouter();
const route = useRoute();

const email = ref(route.query.email || '');
const username = ref(route.query.username || '');
const phoneNumber = ref(route.query.PhoneNumber || '');
const tenantId = ref(route.query.tenantId || '');
const password = ref('');
const errorMessage = ref('');
const isLoading = ref(false);
const isSignUp = ref(true); // Default to sign-up view
const signInEmail = ref('');
const signInPassword = ref('');

const submitForm = async () => {
  errorMessage.value = ''; // Clear previous errors
  isLoading.value = true;

  if (!password.value) {
    errorMessage.value = 'Password is required';
    console.warn('Password field is empty');
    isLoading.value = false;
    return;
  }

  const token = localStorage.getItem('inviteToken');
  if (!token) {
    errorMessage.value = 'No invitation token found';
    console.warn('inviteToken missing in localStorage');
    isLoading.value = false;
    return;
  }

  try {
    console.log('Starting registration for:', email.value);

    // Step 1: Register the user
    console.log('Calling POST /api/auth/register');
    const registerResponse = await api.post('/api/auth/register', {
      email: email.value,
      username: username.value,
      PhoneNumber: phoneNumber.value,
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
    localStorage.setItem('username', registerResponse.data.username);
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
    console.log("token : ", confirmResponse.data.token);

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
  } finally {
    isLoading.value = false;
  }
};

const login = async () => {
  errorMessage.value = '';
  isLoading.value = true;

  try {
    console.log('Starting login for:', signInEmail.value);
    const response = await api.post('/api/auth/login', {
      email: signInEmail.value,
      password: signInPassword.value
    });
    console.log('Login response:', response.data);
    localStorage.setItem('accessToken', response.data.accessToken);
    localStorage.setItem('username', response.data.username);
    localStorage.setItem('role', response.data.role);
    window.location.href = '/home';
  } catch (error) {
    const message = error.response?.data?.message || error.message || 'Login failed';
    errorMessage.value = message;
    console.error('Login error:', {
      message,
      status: error.response?.status,
      data: error.response?.data,
      error
    });
  } finally {
    isLoading.value = false;
  }
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css?family=Montserrat:400,800');

* {
    box-sizing: border-box;
}

.page-wrapper {
    display: flex;
    flex-direction: column;
    min-height: 100vh;
}

body {
    margin: 0;
    background: #f6f5f7;
}

.container {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    width: 768px;
    min-height: 480px;
    background: #fff;
    border-radius: 3rem;
    box-shadow: 0 14px 28px rgba(0, 0, 0, 0.25), 0 10px 10px rgba(0, 0, 0, 0.22);
}

.layout-footer {
    position: fixed;
    bottom: 0;
    left: 0;
    width: 100%;
    display: flex;
    justify-content: space-between;
    align-items: center;
    height: 5rem;
    background: linear-gradient(to top, #ffffff, #f1f5f9);
}

.text-muted-color {
    color: #6c757d;
}

.sign-up, .sign-in {
    position: absolute;
    top: 0;
    left: 0;
    height: 100%;
    transition: all 0.6s ease-in-out;
    border-radius: 3rem;
}

.sign-up {
    width: 50%;
    opacity: 0;
    z-index: 1;
    border-radius: 3rem;
}

.sign-in {
    width: 50%;
    z-index: 2;
    border-radius: 3rem;
}

.form {
    background: #fff;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-direction: column;
    padding: 0 50px;
    height: 100%;
    text-align: center;
    border-radius: 3rem;
}

h1 {
    font-weight: bold;
    margin: 0;
}

p {
    font-size: 14px;
    font-weight: 100;
    line-height: 20px;
    letter-spacing: 0.5px;
    margin: 15px 0 20px;
}

input {
    background: #eee;
    padding: 12px 15px;
    margin: 8px 15px;
    width: 100%;
    border-radius: 3rem;
    border: 1px;
    outline: none;
}

a {
    color: #333;
    font-size: 14px;
    text-decoration: none;
    margin: 15px 0;
}
a:hover{
  color: white !important;
}
.add-button {
    background-color: transparent;
    color: #35D300;
    border-color: #35D300;
    
}

.add-button:hover {
    background-color: #35D300;
    color: white !important;
    border-color: white;
}

button {
    color: #fff;
    font-size: 12px;
    font-weight: bold;
    padding: 12px 55px;
    margin: 20px;
    border-radius: 3rem;
    border: 1px solid #ff4b2b;
    outline: none;
    letter-spacing: 1px;
    text-transform: uppercase;
    transition: transform 80ms ease-in;
    cursor: pointer;
}

button:active {
    transform: scale(0.90);
}

button:disabled {
    background: #cccccc;
    border-color: #cccccc;
    color: #666666;
    cursor: not-allowed;
}

.ghost {
    background-color: transparent;
    border: 2px solid #ffffff;
}

.ghost:hover {
    background-color: white;
    color: #35D300;
    border-color: #35D300;
}

.container.right-panel-active .sign-in {
    transform: translateX(100%);
    border-radius: 3rem;
}

.container.right-panel-active .sign-up {
    transform: translateX(100%);
    opacity: 1;
    z-index: 5;
    animation: show 0s;
    border-radius: 3rem;
}

@keyframes show {
    0%, 49.99% {
        opacity: 0;
        z-index: 1;
    }
    50%, 100% {
        opacity: 1;
        z-index: 5;
    }
}

.overlay-container {
    position: absolute;
    top: 0;
    left: 50%;
    width: 50%;
    height: 100%;
    overflow: hidden;
    transition: transform 0.6s ease-in-out;
    z-index: 100;
    border-radius: 3rem;
}

.container.right-panel-active .overlay-container {
    transform: translateX(-100%);
    border-radius: 3rem;
}

.overlay {
    position: relative;
    color: #fff;
    background: linear-gradient(to left, #35D300, #0f3d00);
    left: -100%;
    height: 100%;
    width: 200%;
    transform: translateX(0);
    border-radius: 3rem;
    transition: transform 0.6s ease-in-out;
}

.container.right-panel-active .overlay {
    transform: translateX(50%);
}

.overlay-left, .overlay-right {
    position: absolute;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-direction: column;
    padding: 0 40px;
    text-align: center;
    top: 0;
    height: 100%;
    width: 50%;
    transform: translateX(0);
    transition: transform 0.6s ease-in-out;
    border-radius: 3rem;
}

.overlay-left {
    transform: translateX(-20%);
    border-radius: 3rem;
}

.overlay-right {
    right: 0;
    transform: translateX(0);
    border-radius: 3rem;
}

.container.right-panel-active .overlay-left {
    transform: translateX(0);
    border-radius: 3rem;
}

.container.right-panel-active .overlay-right {
    transform: translateX(20%);
    border-radius: 3rem;
}

.error-message {
    color: red;
    font-size: 14px;
    margin-top: 10px;
}
</style>