<script>
import { useRouter } from 'vue-router';
import api from '@/api/index'; // Your Axios instance

const router = useRouter();

const handleLogout = () => {
  // 1. Clear stored token and user data
  localStorage.removeItem('accessToken'); // Remove other auth-related items if needed

  // 2. Remove Axios authorization header
  delete api.defaults.headers.common['Authorization'];

  // 3. Redirect to login page
  router.push('/login'); 

  // (Optional) Force reload to reset app state
  window.location.reload(); // Only if needed
};
</script>