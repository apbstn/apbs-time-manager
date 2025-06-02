<script setup>
import { onMounted } from 'vue';
import { useRouter } from 'vue-router';
import api from '@/api'; // Import Axios instance as default export

// Use Vue Router for navigation
const router = useRouter();

// Run the logic when the component is mounted
onMounted(async () => {
  // Step 1: Get the current URL dynamically
  const url = new URL(window.location.href);

  // Step 2: Extract token and data from query parameters
  const token = url.searchParams.get('token');
  let data = url.searchParams.get('data');

  // Log URL and parameters for debugging
  console.log('Current URL:', window.location.href);
  console.log('Extracted token:', token);
  console.log('Extracted raw data:', data);

  // Step 3: Initialize flags to track successful storage
  let isTokenSaved = false;
  let isTenantIdSaved = false;

  // Step 4: Save the token to localStorage
  if (token) {
    localStorage.setItem('inviteToken', token);
    console.log('Token saved to localStorage:', token);
    isTokenSaved = true;
  } else {
    console.warn('No token found in the URL');
  }

  // Step 5: Decode the data (assuming Base64) and extract tenant ID
  let tenantId = null;
  if (data) {
    try {
      // Decode URL-encoded data before Base64 decoding
      data = decodeURIComponent(data);
      console.log('Decoded URL data:', data);

      // Decode Base64 data


      // Assuming the decoded data is a JSON string containing tenantId
      const parsedData = JSON.parse(data);

      const decodedData = atob(parsedData);
      console.log('Decoded Base64 data:', decodedData);

      if (tenantId) {
        localStorage.setItem('tenantId', tenantId);
        console.log('Tenant ID saved to localStorage:', tenantId);
        isTenantIdSaved = true;
      } else {
        console.warn('No tenantId found in decoded data');
      }
    } catch (error) {
      console.error('Error decoding data:', error.message);
      console.log('Invalid data value causing error:', data);
    }
  } else {
    console.warn('No data parameter found in the URL');
  }

  // Step 6: Send POST request to /api/auth/invite/confirmation
  if (isTokenSaved && isTenantIdSaved) {
    try {
      // Get email from localStorage (assumed to be stored as userEmail)
      const email = localStorage.getItem('userEmail');
      if (!email) {
        console.error('No email found in localStorage');
        return;
      }

      const response = await api.post('/api/auth/invite/confirmation', {
        email,
        accessToken: token,
        tenantId,
      });

      const newAccessToken = response.data.accessToken;

      if (newAccessToken) {
        // Step 7: Save new accessToken to localStorage, replacing the old one
        localStorage.removeItem('accessToken');
        localStorage.setItem('accessToken', newAccessToken);
        localStorage.removeItem('inviteToken');
        console.log('New accessToken saved to localStorage:', newAccessToken);

        // Step 8: Redirect to /home
        console.log('Redirecting to /home');
        router.push('/home');
      } else {
        console.error('No accessToken in API response');
      }
    } catch (error) {
      console.error('Error during API call:', error.message);
    }
  } else {
    console.warn('API call and redirect aborted: Missing token or tenantId');
  }
});
</script>
