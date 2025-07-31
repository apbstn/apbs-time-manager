<template>
  <div class="logout-page">
    <p>Logging out...</p>

    <!-- Stop Tracking Reminder Popup -->
    <Dialog
      v-model:visible="showReminderDialog"
      header="Time Tracking Reminder"
      :modal="true"
      :closable="true"
      :style="{ width: '650px' }"
      @update:visible="logoutWithoutStopping"
    >
      <p>Don't forget to stop tracking time!</p>
      <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
      <div class="dialog-footer">
        
        <Button
          label="Logout Anyway"
          class="p-button-text"
          :style="{ color: '#35D300' }"
          @click="logoutWithoutStopping"
        />
        <Button
          label="Stop Tracking and logout"
          icon="pi pi-times"
          class="p-button-text1"
          :style="{ color: '#35D300' }"
          @click="stopTrackingAndLogout"
          :disabled="isStopping"
        />
      </div>
    </Dialog>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router';
import api from '@/api';
import { onMounted, ref } from 'vue';
import Dialog from 'primevue/dialog';
import Button from 'primevue/button';

const router = useRouter();
const showReminderDialog = ref(false);
const isStopping = ref(false);
const errorMessage = ref('');

const decodeJwt = (token) => {
  try {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    return JSON.parse(jsonPayload);
  } catch (error) {
    console.error('Error decoding JWT:', error);
    return null;
  }
};

const handleLogout = async () => {
  try {
    console.log('Initiating logout...');
    // Preserve chronometerStartTime
    const chronometerStartTime = localStorage.getItem('chronometerStartTime');
    // Clear localStorage
    localStorage.clear();
    console.log('Cleared localStorage items');
    if (chronometerStartTime) {
      localStorage.setItem('chronometerStartTime', chronometerStartTime);
      console.log('Preserved chronometerStartTime:', chronometerStartTime);
    }

    // Remove Axios authorization header
    delete api.defaults.headers.common['Authorization'];
    console.log('Removed Authorization header');

    // Redirect to login page
    await router.push('/login');
    console.log('Redirected to /login');
  } catch (error) {
    console.error('Logout failed:', error);
    await router.push('/login'); // Fallback redirect
  }
};

const stopTrackingAndLogout = async () => {
  isStopping.value = true;
  errorMessage.value = '';

  try {
    const token = localStorage.getItem('accessToken');
    if (!token) {
      errorMessage.value = 'No token found';
      console.error('stopTrackingAndLogout: No token found');
      await handleLogout();
      return;
    }

    const decoded = decodeJwt(token);
    if (!decoded) {
      errorMessage.value = 'Invalid token';
      console.error('stopTrackingAndLogout: Invalid token');
      await handleLogout();
      return;
    }

// Adjust to match your JWT's user ID key
    const emaill = localStorage.getItem('email');
    const response = await api.post(`/api/UserTenants/get-id-by-email`, emaill, {
      headers: { Authorization: `Bearer ${token}` }
    });
    const userIdKey = response.data;
    console.log('User ID:', userIdKey);

    if (!userIdKey) {
      errorMessage.value = 'User ID not found in token';
      console.error('stopTrackingAndLogout: User ID not found in token');
      await handleLogout();
      return;
    }

    await api.post(`/api/timelog/stop/${userIdKey}`, {}, {
      headers: { Authorization: `Bearer ${token}` }
    });
    console.log('Tracking stopped for user:', userIdKey);
    localStorage.clear();
    // Clear reminder flag and proceed with logout
    localStorage.setItem('showStopTrackingReminder', 'false');
    showReminderDialog.value = false;
    await handleLogout();
  } catch (error) {
    console.error('Error stopping tracking:', error.response?.data || error.message);
    errorMessage.value = error.response?.data?.message || 'Failed to stop tracking';
  } finally {
    isStopping.value = false;
  }
};

const logoutWithoutStopping = async () => {
  console.log('Logging out without stopping tracking');
  localStorage.setItem('showStopTrackingReminder', 'false');
  showReminderDialog.value = false;
  await handleLogout();
};

onMounted(() => {
  const showReminder = localStorage.getItem('showStopTrackingReminder') === 'true';
  if (showReminder) {
    showReminderDialog.value = true;
  } else {
    handleLogout();
  }
});
</script>

<style scoped>
.logout-page {
  text-align: center;
  padding: 2rem;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 1rem;
}

.error-message {
  color: red;
  font-size: 14px;
  margin-top: 10px;
}

:deep(.p-button.p-button-text) {
  color: #35D300 !important;
  background-color: transparent !important;
  border-color: #35D300 !important;
  border-radius: 1rem;
}

:deep(.p-button.p-button-text:hover) {
  background-color: #35D300 !important;
  color: white !important;
}

:deep(.p-button.p-button-text1) {
  color: #ff0000 !important;
  background-color: transparent !important;
  border-color: #ff0000 !important;
  border-radius: 1rem;
}

:deep(.p-button.p-button-text1:hover) {
  background-color: #ff0000 !important;
  color: white !important;
}

:deep(.p-dialog-header) {
  background: #f9fafb;
  color: #1f2937;
  font-weight: 600;
}

:deep(.p-dialog-content) {
  background: #ffffff;
  color: #1f2937;
  padding: 1rem;
}
</style>