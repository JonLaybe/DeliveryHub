import axios from "axios";

const BASE_URL = 'https://localhost:7225/';

const api = axios.create({
    baseURL: BASE_URL,
});

export default api;