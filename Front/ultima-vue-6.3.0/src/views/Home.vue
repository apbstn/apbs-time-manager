<template>
    <div class="home">
      <h1>Welcome To </h1>
      <h2>The Admin Console</h2>
      <router-link to="/user-management">
        <button>Go to Management</button>
      </router-link>
    </div>
  </template>
  
  <script>
  export default {
    name: "Home",
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
  
  h1 {
    font-size: 2rem;
    margin-bottom: 0.5rem;
  }
  h2{
    font-size: 1.5rem;
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
  