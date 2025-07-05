import React, { useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import LoginPage from './pages/LoginPage';
import DashboardPage from './pages/DashboardPage';
import AdminPage from './pages/AdminPage';
import DocumentsListPage from './pages/DocumentsListPage';
import DocumentDetailPage from './pages/DocumentDetailPage';
import DocumentEditPage from './pages/DocumentEditPage';
import TestPage from './pages/TestPage';
import { useKeycloak } from '@react-keycloak/web';
import { setApiAuthToken } from './services/api';

function App() {
  const { keycloak } = useKeycloak();

  useEffect(() => {
    setApiAuthToken(keycloak?.token);
  }, [keycloak?.token]);

  return (
    <AuthProvider>
      <Router>
        <div className="App">
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route 
              path="/dashboard" 
              element={
                <ProtectedRoute>
                  <DashboardPage />
                </ProtectedRoute>
              } 
            />
            <Route 
              path="/admin" 
              element={
                <ProtectedRoute requiredRole="Admin">
                  <AdminPage />
                </ProtectedRoute>
              } 
            />
            <Route 
              path="/documents" 
              element={
                <ProtectedRoute>
                  <DocumentsListPage />
                </ProtectedRoute>
              } 
            />
            <Route 
              path="/documents/new" 
              element={
                <ProtectedRoute requiredRole="Editor">
                  <DocumentEditPage isEdit={false} />
                </ProtectedRoute>
              } 
            />
            <Route 
              path="/documents/:id" 
              element={
                <ProtectedRoute>
                  <DocumentDetailPage />
                </ProtectedRoute>
              } 
            />
            <Route 
              path="/documents/:id/edit" 
              element={
                <ProtectedRoute requiredRole="Editor">
                  <DocumentEditPage isEdit={true} />
                </ProtectedRoute>
              } 
            />
            <Route 
              path="/test" 
              element={
                <ProtectedRoute>
                  <TestPage />
                </ProtectedRoute>
              } 
            />
            <Route path="*" element={<Navigate to="/login" replace />} />
          </Routes>
        </div>
      </Router>
    </AuthProvider>
  );
}

export default App; 