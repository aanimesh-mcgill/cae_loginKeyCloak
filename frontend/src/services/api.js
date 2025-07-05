import axios from 'axios';

// Create axios instance with base configuration
const api = axios.create({
  baseURL: '', // Use relative URLs, proxy will handle /api
  headers: {
    'Content-Type': 'application/json',
  },
});

// Function to set the token for requests
export function setApiAuthToken(token) {
  if (token) {
    api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  } else {
    delete api.defaults.headers.common['Authorization'];
  }
}

// Response interceptor to handle authentication errors
api.interceptors.response.use(
  (response) => {
    console.log('API response:', {
      url: response.config.url,
      status: response.status,
      data: response.data
    });
    return response;
  },
  (error) => {
    if (error.response) {
      console.error('API error:', {
        url: error.config.url,
        status: error.response.status,
        data: error.response.data,
        headers: error.config.headers
      });
      if (error.response.status === 401) {
        console.error('401 error: Not redirecting for debug');
      }
    } else {
      console.error('API error (no response):', error);
    }
    return Promise.reject(error);
  }
);

// Request interceptor to log API requests
api.interceptors.request.use(
  (config) => {
    console.log('API request:', {
      url: config.url,
      method: config.method,
      headers: config.headers,
      data: config.data
    });
    return config;
  },
  (error) => Promise.reject(error)
);

// Users API calls
export const usersAPI = {
  getAllUsers: async () => {
    const response = await api.get('/api/Users');
    return response.data;
  },
  
  getUserById: async (id) => {
    const response = await api.get(`/api/Users/${id}`);
    return response.data;
  },
  
  updateUser: async (id, userData) => {
    const response = await api.put(`/api/Users/${id}`, userData);
    return response.data;
  },
  
  deleteUser: async (id) => {
    const response = await api.delete(`/api/Users/${id}`);
    return response.data;
  }
};

// Content API calls
export const contentAPI = {
  getAllContent: async () => {
    const response = await api.get('/api/Content');
    return response.data;
  },
  
  getPublishedContent: async () => {
    const response = await api.get('/api/Content/published');
    return response.data;
  },
  
  getContentById: async (id) => {
    const response = await api.get(`/api/Content/${id}`);
    return response.data;
  },
  
  createContent: async (contentData) => {
    const response = await api.post('/api/Content', contentData);
    return response.data;
  },
  
  updateContent: async (id, contentData) => {
    const response = await api.put(`/api/Content/${id}`, contentData);
    return response.data;
  },
  
  deleteContent: async (id) => {
    const response = await api.delete(`/api/Content/${id}`);
    return response.data;
  }
};

// Document API calls
export const documentAPI = {
  getAllDocuments: async () => {
    const response = await api.get('/api/Documents');
    return response.data;
  },
  getDocumentById: async (id) => {
    const response = await api.get(`/api/Documents/${id}`);
    return response.data;
  },
  createDocument: async (docData) => {
    const response = await api.post('/api/Documents', docData);
    return response.data;
  },
  updateDocument: async (id, docData) => {
    const response = await api.put(`/api/Documents/${id}`, docData);
    return response.data;
  },
  deleteDocument: async (id) => {
    const response = await api.delete(`/api/Documents/${id}`);
    return response.data;
  }
};

export default api; 