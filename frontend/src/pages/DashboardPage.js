import React, { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { contentAPI } from '../services/api';

const DashboardPage = () => {
  const { user, logout, hasPermission, isAuthenticated, error: authError, token } = useAuth();
  const [contents, setContents] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (isAuthenticated && token && hasPermission('content:read')) {
      loadContent();
    } else if (!isAuthenticated) {
      setLoading(false);
    }
    // eslint-disable-next-line
  }, [isAuthenticated, token]);

  const loadContent = async () => {
    try {
      setError(null);
      console.log('Dashboard: Fetching content...');
      const data = await contentAPI.getAllContent();
      console.log('Dashboard: Content fetched', data);
      setContents(data);
    } catch (err) {
      console.error('Dashboard: Error fetching content', err);
      setError(err?.response?.data || err.message || 'Failed to load content');
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = async () => {
    await logout();
  };

  if (authError) {
    return (
      <div style={{ padding: 32, color: 'red' }}>
        <h2>Login Error</h2>
        <p>{authError.toString()}</p>
      </div>
    );
  }

  if (!isAuthenticated) {
    return (
      <div style={{ padding: 32 }}>
        <h2>Not logged in</h2>
      </div>
    );
  }

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f5f5f5' }}>
      <div style={{ backgroundColor: 'white', padding: '20px', boxShadow: '0 2px 4px rgba(0,0,0,0.1)', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <div>
          <h1 style={{ margin: 0, color: '#333' }}>Dashboard</h1>
          <p style={{ margin: '5px 0 0 0', color: '#666' }}>
            Welcome, {user?.preferred_username || user?.name || user?.email || 'Unknown'}
          </p>
        </div>
        <button onClick={handleLogout} style={{ padding: '10px 20px', backgroundColor: '#f44336', color: 'white', border: 'none', borderRadius: '5px', cursor: 'pointer' }}>Logout</button>
      </div>

      <div style={{ padding: '20px', maxWidth: '1200px', margin: '0 auto' }}>
        <div style={{ marginBottom: 20 }}>
          <a href="/documents" style={{ background: '#1976d2', color: 'white', padding: '10px 20px', borderRadius: 5, textDecoration: 'none', fontWeight: 'bold' }}>Go to Documents</a>
        </div>

        <div style={{ backgroundColor: 'white', padding: '20px', borderRadius: '10px', marginBottom: '20px', boxShadow: '0 2px 4px rgba(0,0,0,0.1)' }}>
          <h2 style={{ marginTop: 0, color: '#333' }}>User Information</h2>
          <div><strong>Username:</strong> {user?.preferred_username || user?.name || user?.email || 'Unknown'}</div>
          <div><strong>Roles:</strong> {(user?.realm_access?.roles || []).join(', ') || 'None'}</div>
        </div>

        {error && (
          <div style={{ color: 'red', marginBottom: 20 }}>
            <strong>Error:</strong> {error}
          </div>
        )}

        {loading ? (
          <div>Loading content...</div>
        ) : hasPermission('content:read') ? (
          <div style={{ backgroundColor: 'white', padding: '20px', borderRadius: '10px', boxShadow: '0 2px 4px rgba(0,0,0,0.1)' }}>
            <h2 style={{ marginTop: 0, color: '#333' }}>Content Management</h2>
            <div style={{ display: 'grid', gap: '15px' }}>
              {contents.map((content) => (
                <div key={content.id} style={{ border: '1px solid #ddd', borderRadius: '5px', padding: '15px', backgroundColor: content.isPublished ? '#f9f9f9' : '#fff3e0' }}>
                  <h3 style={{ margin: '0 0 10px 0', color: '#333' }}>{content.title}</h3>
                  <p style={{ margin: '0 0 10px 0', color: '#666' }}>{content.body}</p>
                  <div style={{ fontSize: '12px', color: '#999', display: 'flex', gap: '20px' }}>
                    <span>Created by: {content.createdBy}</span>
                    <span>Created: {new Date(content.createdAt).toLocaleDateString()}</span>
                    {content.updatedAt && (<span>Updated: {new Date(content.updatedAt).toLocaleDateString()}</span>)}
                  </div>
                </div>
              ))}
            </div>
          </div>
        ) : (
          <div>You do not have permission to view content.</div>
        )}
      </div>
    </div>
  );
};

export default DashboardPage; 