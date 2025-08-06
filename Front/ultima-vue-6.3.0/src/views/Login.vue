<template>
    <div class="container" :class="{ 'right-panel-active': isSignUp }">
        <div class="sign-up">
            <form @submit.prevent="signUp" class="form">
                <h1>To Create an account, you need to have an Invitation</h1>
            </form>
        </div>
        <div class="sign-in">
            <form @submit.prevent="login" class="form">
                <h1>Sign in</h1>
                <input type="email" v-model="email" placeholder="Email" required>
                <input type="password" v-model="password" placeholder="Password" required>
                <button type="button" @click="showModal = true" class="link-button">Forget your Password?</button>
                <button type="submit" :disabled="isLoading" class="add-button">
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
                    <!--<button class="ghost" @click="isSignUp = true" :disabled="isLoading">Sign Up</button>-->
                </div>
            </div>
        </div>
        <div v-if="showModal" class="modal-overlay">
            <div class="modal">
                <h2>Password Reset</h2>
                <p>Please contact the admin to reset your password.</p>
                <button class="add-button" @click="showModal = false">Close</button>
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
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/api'

const email = ref('')
const password = ref('')
const errorMessage = ref('')
const router = useRouter()
const isSignUp = ref(false)
const isLoading = ref(false)
const showModal = ref(false)

const login = async () => {
  isLoading.value = true
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

    if (response.data && response.data.token) {
      localStorage.setItem("accessToken", response.data.token.result);
      localStorage.setItem('username', response.data.username)
      
      api.defaults.headers.common['Authorization'] = `Bearer ${response.data.accessToken}`
      
      window.location.href = '/home';
    } else {
      console.error('No access token found in response!')
      errorMessage.value = 'Login failed: No token received.'
    }
  } catch (error) {
    console.error('Login Error:', error.response?.data || error.message)
    errorMessage.value = error.response?.data?.message || 'Invalid credentials!'
  } finally {
    isLoading.value = false
  }
}

const signUp = async () => {
  // Placeholder for signUp function
  errorMessage.value = 'Sign up is not available. Please contact admin for an invitation.'
}
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
    height: 5rem !important;
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
    border-radius: 3rem !important;
}

.sign-up {
    width: 50%;
    opacity: 0;
    z-index: 1;
    border-radius: 3rem !important;
}

.sign-in {
    width: 50%;
    z-index: 2;
    border-radius: 3rem !important;
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
    border-radius: 3rem !important;
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
    border-radius: 3rem !important;
    border: 1px;
    outline: none;
}

a {
    color: #333;
    font-size: 14px;
    text-decoration: none;
    margin: 15px 0;
}

.add-button {
    background-color: transparent !important;
    color: #35D300 !important;
    border-color: #35D300 !important;
}

.add-button:hover {
    background-color: #35D300 !important;
    color: white !important;
    border-color: white !important;
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
    background-color: white !important;
    color: #35D300 !important;
    border-color: #35D300 !important;
}

.container.right-panel-active .sign-in {
    transform: translateX(100%);
    border-radius: 3rem !important;
}

.container.right-panel-active .sign-up {
    transform: translateX(100%);
    opacity: 1;
    z-index: 5;
    animation: show 0s;
    border-radius: 3rem !important;
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
    border-radius: 3rem !important;
}

.container.right-panel-active .overlay-container {
    transform: translateX(-100%);
    border-radius: 3rem !important;
}

.overlay {
    position: relative;
    color: #fff;
    background: linear-gradient(to left, #35D300, #0f3d00);
    left: -100%;
    height: 100%;
    width: 200%;
    transform: translateX(0);
    border-radius: 3rem !important;
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
    border-radius: 3rem !important;
}

.overlay-left {
    transform: translateX(-20%);
    border-radius: 3rem !important;
}

.overlay-right {
    right: 0;
    transform: translateX(0);
    border-radius: 3rem !important;
}

.container.right-panel-active .overlay-left {
    transform: translateX(0);
    border-radius: 3rem !important;
}

.container.right-panel-active .overlay-right {
    transform: translateX(20%);
    border-radius: 3rem !important;
}

.social-container {
    margin: 20px 0;
    border-radius: 3rem !important;
}

.social-container a {
    height: 40px;
    width: 40px;
    margin: 0 5px;
    display: inline-flex;
    justify-content: center;
    align-items: center;
    border: 1px solid #ccc;
    border-radius: 3rem;
}

.error-message {
    color: red;
    font-size: 14px;
    margin-top: 10px;
}

.link-button {
    color: #000000;
    font-size: 10px;
    text-decoration: none;
    margin: 15px 0;
    border: none;
}

.link-button:hover {
    text-decoration: underline;
}

.modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.5);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
    border-radius: 3rem;
}

.modal {
    background: #fff;
    padding: 20px;
    border-radius: 1rem;
    box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
    text-align: center;
    max-width: 400px;
    width: 90%;
    font-family: 'Montserrat', sans-serif;
}

.modal h2 {
    margin: 0 0 15px;
    font-weight: bold;
    color: #333;
}

.modal p {
    margin: 0 0 20px;
    font-size: 14px;
    color: #333;
}

.modal-button {
    background-color: #35D300;
    color: white;
    border: none;
    padding: 10px 30px;
    border-radius: 3rem;
    font-size: 12px;
    font-weight: bold;
    text-transform: uppercase;
    cursor: pointer;
    transition: background-color 0.3s ease;
}

.modal-button:hover {
    background-color: #0f3d00;
}
</style>