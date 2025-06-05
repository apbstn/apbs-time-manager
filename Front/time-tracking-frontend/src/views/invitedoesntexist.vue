<script setup>
import { onMounted } from 'vue';
import { useRouter } from 'vue-router';
import api from '@/api'; // Axios instance

const router = useRouter();

onMounted(async () => {
  // Get URL query parameters
  const url = new URL(window.location.href);
  const token = url.searchParams.get('token');
  const data = url.searchParams.get('data');

  if (!token || !data) {
    console.warn('Missing token or data in URL');
    router.push('/error?message=Missing invitation parameters');
    return;
  }

  try {
    // Fetch invitation data (including phoneNumber) using token
    const response = await api.get(`/api/auth/invite/check/${data}`);
    const { email, username, tenantId, phoneNumber } = response.data;

    if (!email || !username || !tenantId) {
      console.warn('Incomplete invitation data');
      router.push('/error?message=Invalid invitation data');
      return;
    }

    // Store token in localStorage
    localStorage.setItem('inviteToken', token);
    localStorage.setItem('inviteData', tenantId);

    // Redirect to register with invitation data
    router.push({
      path: '/register',
      query: { email, username, phoneNumber, tenantId }
    });
  } catch (error) {
    console.error('Error fetching invitation data:', error.message);
    router.push('/error?message=Failed to validate invitation');
  }
});
</script>

<template>
  <div>Loading invitation...</div>
</template>