import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const ProtectedRoute = ({ children, requiredRole, requiredPermission }) => {
  const { user, loading, hasRole, hasPermission, isAuthenticated } = useAuth();
  const location = useLocation();

  console.log('ProtectedRoute:', {
    path: location.pathname,
    isAuthenticated,
    user,
    loading,
    requiredRole,
    hasRole: requiredRole ? hasRole(requiredRole) : undefined,
    requiredPermission,
    hasPermission: requiredPermission ? hasPermission(requiredPermission) : undefined
  });

  if (loading) {
    return (
      <div style={{ 
        display: 'flex', 
        justifyContent: 'center', 
        alignItems: 'center', 
        height: '100vh',
        fontSize: '18px'
      }}>
        Loading...
      </div>
    );
  }

  if (!isAuthenticated) {
    return <div>Not authenticated (debug mode, login page should be accessible)</div>;
  }

  if (requiredRole && !hasRole(requiredRole)) {
    return (
      <div style={{ 
        display: 'flex', 
        justifyContent: 'center', 
        alignItems: 'center', 
        height: '100vh',
        flexDirection: 'column',
        gap: '20px'
      }}>
        <h2>Access Denied</h2>
        <p>You don't have the required role to access this page.</p>
        <p>Required role: {requiredRole}</p>
        <p>Your roles: {user?.realm_access?.roles?.join(', ')}</p>
        <button onClick={() => window.history.back()}>Go Back</button>
      </div>
    );
  }

  if (requiredPermission && !hasPermission(requiredPermission)) {
    return (
      <div style={{ 
        display: 'flex', 
        justifyContent: 'center', 
        alignItems: 'center', 
        height: '100vh',
        flexDirection: 'column',
        gap: '20px'
      }}>
        <h2>Access Denied</h2>
        <p>You don't have the required permission to access this page.</p>
        <p>Required permission: {requiredPermission}</p>
        <button onClick={() => window.history.back()}>Go Back</button>
      </div>
    );
  }

  return children;
};

export default ProtectedRoute; 