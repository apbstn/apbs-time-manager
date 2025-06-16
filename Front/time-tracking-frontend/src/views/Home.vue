<template>
  <div class="home">
    <div class="content">
      <h1>Welcome, {{ aname }} </h1>
      <h1>To Zontorno</h1>
      <router-link to="/time-tracking">
        <button>Start</button>
      </router-link>
    </div>
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
  text-align: center;
  padding: 25vh 0; /* Use percentage of viewport height for vertical spacing */
}

.content {
  display: flex;
  flex-direction: column;
  align-items: center;
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