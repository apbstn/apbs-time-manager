<template>
  <div class="time-tracking">
    <div class="header-container">
      <h2>Time Tracking</h2>
      <div class="buttons">
        <div class="buttonstart"><button class="pi pi-play" style="color: green" @click="startTracking"></button></div>
        <div class="buttonpause"><button class="pi pi-pause" style="color: orange" @click="pauseTracking"></button></div>
        <div class="buttonstop"><button class="pi pi-stop" style="color: red" @click="stopTracking"></button></div>
      </div>
    </div>
    <div class="content">
      <!-- Your main centered content would go here -->
      <p v-if="statusMessage" class="status">{{ statusMessage }}</p>
    </div>
  </div>
</template>


<script setup>
import { ref } from 'vue'
import api from '@/api'

// reactive state
const currentStatus = ref('')
const statusMessage = ref('')
const userId = localStorage.getItem('userId') || 'UnknownUser'

// 🟢 Start Tracking
const startTracking = async () => {
  console.log('Start button clicked')

  const token = localStorage.getItem('accessToken')
  if (!token) {
    statusMessage.value = "You're not authenticated. Please log in again."
    return
  }

  console.log('Stored Token:', token)

  const idInput = prompt('Enter the ID (number):')
  const id = idInput ? parseInt(idInput, 10) : null
  if (id === null || isNaN(id)) {
    statusMessage.value = 'Invalid ID. Please enter a valid number.'
    return
  }

  try {
    await api.post(`api/timelog/start/${id}`, {}, {
      headers: { Authorization: `Bearer ${token}` }
    })
    currentStatus.value = 'started'
    statusMessage.value = 'Timer started successfully!'

    const username = localStorage.getItem('username') || 'User'
    window.addNotification?.(`${id} - ${username} just started the timer`)
  } catch (error) {
    console.error('Error starting tracking:', error.response?.data || error.message)
    statusMessage.value = 'Error starting timer.'
  }
}

// 🟠 Pause Tracking
const pauseTracking = async () => {
  console.log('Pause button clicked')

  if (currentStatus.value !== 'started') {
    statusMessage.value = 'You can only pause after starting.'
    return
  }

  const token = localStorage.getItem('accessToken')
  if (!token) {
    statusMessage.value = "You're not authenticated. Please log in again."
    return
  }

  console.log('Stored Token:', token)

  const idInput = prompt('Enter the ID (number):')
  const id = idInput ? parseInt(idInput, 10) : null
  if (id === null || isNaN(id)) {
    statusMessage.value = 'Invalid ID. Please enter a valid number.'
    return
  }

  try {
    await api.post(`api/timelog/pause/${id}`, {}, {
      headers: { Authorization: `Bearer ${token}` }
    })
    currentStatus.value = 'paused'
    statusMessage.value = 'Timer paused successfully!'

    const username = localStorage.getItem('username') || 'User'
    window.addNotification?.(`${id} - ${username} just paused the timer`)
  } catch (error) {
    console.error('Error pausing tracking:', error.response?.data || error.message)
    statusMessage.value = 'Error pausing timer.'
  }
}

// 🔴 Stop Tracking
const stopTracking = async () => {
  console.log('Stop button clicked')

  if (!['started', 'paused'].includes(currentStatus.value)) {
    statusMessage.value = 'You can only stop after starting or pausing.'
    return
  }

  const token = localStorage.getItem('accessToken')
  if (!token) {
    statusMessage.value = "You're not authenticated. Please log in again."
    return
  }

  console.log('Stored Token:', token)

  const idInput = prompt('Enter the ID (number):')
  const id = idInput ? parseInt(idInput, 10) : null
  if (id === null || isNaN(id)) {
    statusMessage.value = 'Invalid ID. Please enter a valid number.'
    return
  }

  try {
    await api.post(`api/timelog/stop/${id}`, {}, {
      headers: { Authorization: `Bearer ${token}` }
    })
    currentStatus.value = 'stopped'
    statusMessage.value = 'Timer stopped successfully!'

    const username = localStorage.getItem('username') || 'User'
    window.addNotification?.(`${id} - ${username} just stopped the timer`)
  } catch (error) {
    console.error('Error stopping tracking:', error.response?.data || error.message)
    statusMessage.value = 'Error stopping timer.'
  }
}
</script>

<style scoped>
.time-tracking {
  position: relative;
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.header-container {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  width: 100%;
  box-sizing: border-box;
}

.content {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
  padding: 1rem;
  background-color: transparent;
}

.buttons {
  display: flex;
  gap: 1rem;
  background-color: transparent;
}

button {
  padding: 0.5rem 1rem;
  font-size: 1rem;
  cursor: pointer;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  background-color: transparent;
}

.buttonstart:hover {
  border: 1px solid green;
}
.buttonpause:hover{
  border: 1px solid orange;
}
.buttonstop:hover{
  border: 1px solid red;
}

.status {
  margin-top: 10px;
  font-weight: bold;
  color: #333;
}

h2 {
  font-size: 2rem;
  margin: 0;
}
</style>