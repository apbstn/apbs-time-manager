import axios from "axios";

const apiUrl = import.meta.env.VITE_API_URL;
console.log(apiUrl);

const api = axios.create({
  baseURL: apiUrl || "http://localhost:58169",
  headers: {
    Accept: "application/json", // This helps clarify the expected response
  },
});

// Load token from storage if available
const token = localStorage.getItem("accessToken");
if (token) {
  api.defaults.headers.common["Authorization"] = `Bearer ${token}`;
}

export default api;
