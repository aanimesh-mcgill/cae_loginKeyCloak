import React from 'react';
import { useAuth } from '../contexts/AuthContext';
import { Navigate } from 'react-router-dom';

const LoginPage = () => {
  const { login, loading, isAuthenticated } = useAuth();

  if (isAuthenticated) {
    // Redirect authenticated users away from login page
    return <Navigate to="/dashboard" replace />;
  }

  return (
    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
      <div style={{ background: 'white', padding: 32, borderRadius: 12, boxShadow: '0 2px 12px rgba(0,0,0,0.1)' }}>
        <h2>Login with Keycloak</h2>
        <button onClick={login} disabled={loading} style={{ marginTop: 16, padding: '8px 24px', fontSize: 18 }}>
          {loading ? 'Loading...' : 'Login'}
        </button>
      </div>
    </div>
  );
};

export default LoginPage; 