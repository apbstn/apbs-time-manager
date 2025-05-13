<template>
    <div class="home">
      <h1>Welcome, </h1>
      <router-link to="/tenant-management">
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
    background-color: #007bff;
    color: white;
    border: none;
    border-radius: 5px;
  }
  
  button:hover {
    background-color: #0056b3;
  }
  </style>
  