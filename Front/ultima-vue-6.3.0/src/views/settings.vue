<template>
  <div class="settings-container">
    <h1>Settings</h1>
    <div v-if="isLoading" class="loading">Loading...</div>
    <div v-else-if="error" class="error">{{ error }}</div>
    <div v-else class="user-info">
      <p><strong>Username:</strong> Admin</p>
      <p><strong>Email:</strong> {{ email || 'Not set' }}</p>
    </div>
  </div>
</template>

<script>
// JWT decoding function
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

export default {
  name: 'Settings',
  data() {
    return {
      username: null,
      email: null,
      isLoading: true,
      error: null,
    };
  },
  created() {
    this.loadUserData();
  },
  methods: {
    loadUserData() {
      try {
        // Retrieve email and username from localStorage
        this.email = localStorage.getItem('email');
        this.username = localStorage.getItem('username');

        // Retrieve token from localStorage
        const token = localStorage.getItem('accessToken');
        if (!token) {
          this.error = 'No token found';
          this.isLoading = false;
          return;
        }

        // Decode token
        const decoded = decodeJwt(token);
        console.log('Decoded token:', decoded); // Debug log
        if (!decoded) {
          this.error = 'Invalid token';
          this.isLoading = false;
          return;
        }

        this.isLoading = false;
      } catch (error) {
        this.error = 'Error loading settings: ' + error.message;
        this.isLoading = false;
      }
    },
  },
};
</script>

<style scoped>
.settings-container {
  margin-bottom: 1.5rem;
}

h1 {
  font-size: 2rem;
  margin: 0;
}

.user-info {
  margin-top: 1rem;
  background: #f9f9f9;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.user-info p {
  margin: 10px 0;
  font-size: 16px;
}

.loading,
.error {
  text-align: center;
  color: #666;
  font-size: 18px;
  margin-top: 20px;
}

.error {
  color: #d32f2f;
}
</style>