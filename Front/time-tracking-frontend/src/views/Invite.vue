<script setup>
import { onMounted } from 'vue';
import { useRouter } from 'vue-router';
import api from '@/api'; // Import Axios instance

const router = useRouter();

onMounted(async () => {
  // Step 1: Get the current URL
  const url = new URL(window.location.href);

  // Step 2: Extract token and data from query parameters
  const token = url.searchParams.get('token');
  const data = url.searchParams.get('data');

  // Step 3: Initialize flags
  let isTokenSaved = false;
  let isTenantIdSaved = false;

  // Step 4: Save token to localStorage
  if (token) {
    localStorage.setItem('inviteToken', token);
    isTokenSaved = true;
  } else {
    console.warn('No token found in the URL');
    router.push('/error?message=Missing token');
    return;
  }

  // Step 5: Fetch tenantId from API
  let tenantId = null;
  if (data) {
    try {
      const response = await api.get(`/api/auth/invite/check/${data}`);
      tenantId = response.data.tenantId;
      if (tenantId) {
        localStorage.setItem('tenantId', tenantId);
        isTenantIdSaved = true;
      } else {
        console.warn('No tenantId found in API response');
        router.push('/error?message=No tenantId in response');
      }
    } catch (error) {
      console.error('Error fetching tenantId:', error.message);
      router.push('/error?message=Failed to fetch tenantId');
    }
  } else {
    console.warn('No data parameter found in the URL');
    router.push('/error?message=Missing data parameter');
  }

  // Step 6: Send POST request to /api/auth/invite/confirmation
  if (isTokenSaved && isTenantIdSaved) {
    try {
      const email = localStorage.getItem('email');
      if (!email) {
        console.error('No email found in localStorage');
        router.push('/error?message=No email found');
        return;
      }

      const response = await api.post('/api/auth/invite/confirmation', {
        email,
        token: token,
        tenantId,
      });

      

      const newAccessToken = response.data.accessToken;
      const role = response.data.role;

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
const decoded = decodeJwt(newAccessToken);
const emailll = localStorage.getItem('email');
const Idd = await api.post('/api/UserTenants/get-id-by-email', emailll);
    console.log('Decoded token:', decoded);
    if (!decoded) {
      errorMessage.value = 'Invalid token';
      console.error('nametenant: Invalid token');
      return false;
    }
// Adjust to 'sub', 'tid', etc., if needed

      const leaveBalance = await api.post(`/api/LeaveRequests/balance/allocate/${Idd}`, 10.0);
      const leaveBalanceResponse = await leaveBalance.data;
      
      if (newAccessToken) {
        // Step 7: Save new accessToken
        localStorage.removeItem('accessToken');
        localStorage.setItem('accessToken', newAccessToken);
        localStorage.removeItem('inviteToken');
        localStorage.removeItem('role');
        localStorage.setItem('role', role);

        // Step 8: Redirect to /home with reload
        window.location.href = '/home';
      } else {
        console.error('No accessToken in API response');
        router.push('/error?message=Invalid API response');
      }
    } catch (error) {
      console.error('Error during API call:', error.message);
      router.push('/error?message=API call failed');
    }
  } else {
    console.warn('API call aborted: Missing token or tenantId');
    router.push('/error?message=Missing parameters');
  }
});
</script>