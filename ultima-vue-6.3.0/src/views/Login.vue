<template>
  <div class="login-container">
    <h2>Login</h2>
    <form @submit.prevent="login" class="login-form">
      <input v-model="email" type="email" placeholder="Email" required />
      <input v-model="password" type="password" placeholder="Password" required />
      <button type="submit">Login</button>
    </form>
    <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
  </div>
</template>

<script>
import api from "../api";

export default {
  data() {
    return {
      email: "",
      password: "",
      errorMessage: "",
    };
  },
  methods: {
    async login() {
      try {
        console.log("Sending login data:", this.email, this.password);
        const response = await api.post("/api/auth/login", {
          email: this.email,
          password: this.password,
        },
          {
            headers: {
              'Content-Type': 'application/json'
            }
      });
      
        // Verify if accessToken is present
        if (response.data && response.data.token) {
          
          // Store tokens in localStorage
          localStorage.setItem("accessToken", response.data.token.result);
          localStorage.setItem("refreshToken", response.data.Token);
          localStorage.setItem("token", response.data.token.result);


          // Set Authorization header
          api.defaults.headers.common["Authorization"] = `Bearer ${response.data.result}`;

          // Redirect to timetracking page
          this.$router.push("/home");
        } else {
          console.error("No access token found in response!");
          this.errorMessage = "Login failed: No token received.";
        }
      } catch (error) {
        console.error("Login Error:", error.response?.data || error.message);
        this.errorMessage = error.response?.data?.message || "Invalid credentials!";
      }
    },
  },
};
</script>

<style scoped>
/* Centering the login container */
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  flex-direction: column;
  background-color: #f5f5f5;
  /* Optional: set a background color */
}

h2 {
  margin-bottom: 20px;
  font-size: 24px;
  color: #333;
}

.login-form {
  display: flex;
  flex-direction: column;
  width: 300px;
  /* You can adjust the width as needed */
  padding: 20px;
  background-color: #fff;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

input {
  padding: 10px;
  margin-bottom: 15px;
  border: 1px solid #ccc;
  border-radius: 4px;
  font-size: 16px;
}

button {
  padding: 10px;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 16px;
  cursor: pointer;
}

button:hover {
  background-color: #0056b3;
}

.error-message {
  margin-top: 10px;
  color: red;
  font-size: 14px;
}
</style>