import React, { createContext, useContext } from 'react';
import { useKeycloak } from '@react-keycloak/web';
import { setApiAuthToken } from '../services/api';

const AuthContext = createContext();

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

export const AuthProvider = ({ children }) => {
  const { keycloak, initialized } = useKeycloak();

  // Parse user info and roles from Keycloak token
  const user = keycloak?.tokenParsed || null;
  const loading = !initialized;
  const isAuthenticated = !!keycloak?.authenticated;

  React.useEffect(() => {
    if (isAuthenticated && keycloak?.token) {
      setApiAuthToken(keycloak.token);
      console.log('AuthContext: Set API auth token', keycloak.token);
    } else {
      setApiAuthToken(null);
      console.log('AuthContext: Cleared API auth token');
    }
  }, [isAuthenticated, keycloak?.token]);

  console.log('AuthContext:', {
    keycloak,
    keycloakAuthenticated: keycloak?.authenticated,
    tokenParsed: keycloak?.tokenParsed,
    user,
    loading,
    isAuthenticated
  });

  // Keycloak roles are in realm_access.roles or resource_access[client].roles
  const roles = user?.realm_access?.roles || [];

  const hasRole = (role) => roles.includes(role);

  // Permissions can be mapped to roles or custom claims as needed
  const hasPermission = (permission) => {
    // Example: map permissions to roles
    const permissions = {
      'Admin': ['content:read', 'content:create', 'content:update', 'content:delete', 'users:read', 'users:update', 'users:delete', 'system:admin'],
      'Editor': ['content:read', 'content:create', 'content:update'],
      'Viewer': ['content:read']
    };
    for (const role of roles) {
      if (permissions[role]?.includes(permission)) return true;
    }
    return false;
  };

  const login = () => keycloak?.login();
  const logout = () => keycloak?.logout();

  const value = {
    user,
    loading,
    error: null,
    login,
    logout,
    hasPermission,
    hasRole,
    isAuthenticated,
    token: keycloak?.token
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}; 