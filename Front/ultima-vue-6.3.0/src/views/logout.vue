<template>
  <div class="logout-page">
    <p>Logging out...</p>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router';
import api from '@/api/index'; // Your Axios instance
import { onMounted } from 'vue';

const router = useRouter();

onMounted(() => {
  handleLogout();
});

const handleLogout = async () => {
  try {
    console.log('Initiating logout...');
    // 1. Clear stored token and user data
    localStorage.clear();// Clear tenantId as well
    console.log('Cleared localStorage items');

    // 2. Remove Axios authorization header
    delete api.defaults.headers.common['Authorization'];
    console.log('Removed Authorization header');

    // 3. Redirect to login page
    await router.push('/login');
    console.log('Redirected to /login');
  } catch (error) {
    console.error('Logout failed:', error);
    await router.push('/login'); // Fallback redirect
  }
};
</script>

<style scoped>
.logout-page {
  text-align: center;
  padding: 2rem;
}
</style>