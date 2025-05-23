import axios from "axios";

const apiUrl = import.meta.env.VITE_API_URL;
console.log(apiUrl)
const api = axios.create({
  baseURL: apiUrl ?? "http://localhost:58169", // Ensure this is correct
  headers: {
    "Content-Type": "application/json",
  },
});

// Load token from storage if available
const token = localStorage.getItem("accessToken");
if (token) {
  api.defaults.headers.common["Authorization"] = `Bearer ${token}`;
}

export default api;