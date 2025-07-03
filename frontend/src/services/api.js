import axios from 'axios';

// Create axios instance with base configuration
const api = axios.create({
  baseURL: process.env.REACT_APP_API_URL || 'https://localhost:7001/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor to add JWT token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor to handle authentication errors
api.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    if (error.response?.status === 401) {
      // Token expired or invalid
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// Authentication API calls
export const authAPI = {
  login: async (username, password) => {
    const response = await api.post('/auth/login', { username, password });
    return response.data;
  },
  
  logout: async () => {
    const response = await api.post('/auth/logout');
    return response.data;
  },
  
  getCurrentUser: async () => {
    const response = await api.get('/auth/me');
    return response.data;
  }
};

// Users API calls
export const usersAPI = {
  getAllUsers: async () => {
    const response = await api.get('/users');
    return response.data;
  },
  
  getUserById: async (id) => {
    const response = await api.get(`/users/${id}`);
    return response.data;
  },
  
  updateUser: async (id, userData) => {
    const response = await api.put(`/users/${id}`, userData);
    return response.data;
  },
  
  deleteUser: async (id) => {
    const response = await api.delete(`/users/${id}`);
    return response.data;
  }
};

// Content API calls
export const contentAPI = {
  getAllContent: async () => {
    const response = await api.get('/content');
    return response.data;
  },
  
  getPublishedContent: async () => {
    const response = await api.get('/content/published');
    return response.data;
  },
  
  getContentById: async (id) => {
    const response = await api.get(`/content/${id}`);
    return response.data;
  },
  
  createContent: async (contentData) => {
    const response = await api.post('/content', contentData);
    return response.data;
  },
  
  updateContent: async (id, contentData) => {
    const response = await api.put(`/content/${id}`, contentData);
    return response.data;
  },
  
  deleteContent: async (id) => {
    const response = await api.delete(`/content/${id}`);
    return response.data;
  }
};

export default api; 