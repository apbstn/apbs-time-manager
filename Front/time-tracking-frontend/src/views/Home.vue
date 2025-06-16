<template>
  <div class="home">
    <h1>Welcome, {{ aname }} </h1>
    <h1>To TimKing</h1>
    <router-link to="/time-tracking">
      <button>Start</button>
    </router-link>
  </div>
</template>

<script>
export default {
  name: "Home",
  data() {
    return {
      aname: localStorage.getItem('username') || 'Guest' // Default to 'Guest' if null
    };
  }
};
</script>

<style scoped>
.home {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  text-align: center;
}

h1 {
  font-size: 2rem;
  margin-bottom: 1.5rem;
}

button {
    padding: 0.7rem 1.5rem;
    font-size: 1.2rem;
    cursor: pointer;
    background-color: #007bff00 !important;
    color: #35D300 !important;
    border: 1px solid #35D300 !important;
    border-radius: 10px;
  }
  
  button:hover {
    background-color: #35D300 !important;
    color: white !important;
  }
</style>