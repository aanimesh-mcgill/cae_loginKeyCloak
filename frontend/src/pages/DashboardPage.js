import React, { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { contentAPI } from '../services/api';

const DashboardPage = () => {
  const { user, logout, hasPermission } = useAuth();
  const [contents, setContents] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadContent();
  }, []);

  const loadContent = async () => {
    try {
      const data = await contentAPI.getAllContent();
      setContents(data);
    } catch (error) {
      console.error('Failed to load content:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = async () => {
    await logout();
  };

  const getRoleColor = (role) => {
    switch (role) {
      case 'Admin': return '#4caf50';
      case 'Editor': return '#2196f3';
      case 'Viewer': return '#ff9800';
      default: return '#666';
    }
  };

  const getPermissions = () => {
    const permissions = [];
    if (hasPermission('content:read')) permissions.push('Read Content');
    if (hasPermission('content:create')) permissions.push('Create Content');
    if (hasPermission('content:update')) permissions.push('Update Content');
    if (hasPermission('content:delete')) permissions.push('Delete Content');
    if (hasPermission('users:read')) permissions.push('View Users');
    if (hasPermission('users:update')) permissions.push('Update Users');
    if (hasPermission('users:delete')) permissions.push('Delete Users');
    if (hasPermission('system:admin')) permissions.push('System Administration');
    return permissions;
  };

  if (loading) {
    return (
      <div style={{ padding: '20px', textAlign: 'center' }}>
        Loading dashboard...
      </div>
    );
  }

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f5f5f5' }}>
      {/* Header */}
      <div style={{
        backgroundColor: 'white',
        padding: '20px',
        boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center'
      }}>
        <div>
          <h1 style={{ margin: 0, color: '#333' }}>Dashboard</h1>
          <p style={{ margin: '5px 0 0 0', color: '#666' }}>
            Welcome, {user?.displayName} ({user?.username})
          </p>
        </div>
        <div style={{ display: 'flex', alignItems: 'center', gap: '20px' }}>
          <div style={{
            padding: '8px 16px',
            backgroundColor: getRoleColor(user?.role),
            color: 'white',
            borderRadius: '20px',
            fontSize: '14px',
            fontWeight: 'bold'
          }}>
            {user?.role}
          </div>
          <button
            onClick={handleLogout}
            style={{
              padding: '10px 20px',
              backgroundColor: '#f44336',
              color: 'white',
              border: 'none',
              borderRadius: '5px',
              cursor: 'pointer'
            }}
          >
            Logout
          </button>
        </div>
      </div>

      <div style={{ padding: '20px', maxWidth: '1200px', margin: '0 auto' }}>
        <div style={{ marginBottom: 20 }}>
          <a href="/documents" style={{ background: '#1976d2', color: 'white', padding: '10px 20px', borderRadius: 5, textDecoration: 'none', fontWeight: 'bold' }}>Go to Documents</a>
        </div>
        {/* User Info Card */}
        <div style={{
          backgroundColor: 'white',
          padding: '20px',
          borderRadius: '10px',
          marginBottom: '20px',
          boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
        }}>
          <h2 style={{ marginTop: 0, color: '#333' }}>User Information</h2>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '20px' }}>
            <div>
              <strong>Username:</strong> {user?.username}
            </div>
            <div>
              <strong>Display Name:</strong> {user?.displayName}
            </div>
            <div>
              <strong>Email:</strong> {user?.email}
            </div>
            <div>
              <strong>Role:</strong> 
              <span style={{
                marginLeft: '10px',
                padding: '4px 8px',
                backgroundColor: getRoleColor(user?.role),
                color: 'white',
                borderRadius: '4px',
                fontSize: '12px'
              }}>
                {user?.role}
              </span>
            </div>
            <div>
              <strong>Last Login:</strong> {new Date(user?.lastLogin).toLocaleString()}
            </div>
          </div>
        </div>

        {/* Permissions Card */}
        <div style={{
          backgroundColor: 'white',
          padding: '20px',
          borderRadius: '10px',
          marginBottom: '20px',
          boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
        }}>
          <h2 style={{ marginTop: 0, color: '#333' }}>Your Permissions</h2>
          <div style={{ display: 'flex', flexWrap: 'wrap', gap: '10px' }}>
            {getPermissions().map((permission, index) => (
              <span
                key={index}
                style={{
                  padding: '6px 12px',
                  backgroundColor: '#e3f2fd',
                  color: '#1976d2',
                  borderRadius: '15px',
                  fontSize: '14px',
                  border: '1px solid #bbdefb'
                }}
              >
                {permission}
              </span>
            ))}
          </div>
        </div>

        {/* Content Section */}
        <div style={{
          backgroundColor: 'white',
          padding: '20px',
          borderRadius: '10px',
          boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
        }}>
          <h2 style={{ marginTop: 0, color: '#333' }}>Content Management</h2>
          
          {hasPermission('content:create') && (
            <div style={{
              padding: '15px',
              backgroundColor: '#e8f5e8',
              borderRadius: '5px',
              marginBottom: '20px',
              border: '1px solid #c8e6c9'
            }}>
              <strong>Editor Access:</strong> You can create and edit content.
            </div>
          )}

          {hasPermission('content:delete') && (
            <div style={{
              padding: '15px',
              backgroundColor: '#fff3e0',
              borderRadius: '5px',
              marginBottom: '20px',
              border: '1px solid #ffcc02'
            }}>
              <strong>Admin Access:</strong> You have full control over all content and users.
            </div>
          )}

          <div style={{ display: 'grid', gap: '15px' }}>
            {contents.map((content) => (
              <div
                key={content.id}
                style={{
                  border: '1px solid #ddd',
                  borderRadius: '5px',
                  padding: '15px',
                  backgroundColor: content.isPublished ? '#f9f9f9' : '#fff3e0'
                }}
              >
                <h3 style={{ margin: '0 0 10px 0', color: '#333' }}>
                  {content.title}
                  {!content.isPublished && (
                    <span style={{
                      marginLeft: '10px',
                      padding: '2px 8px',
                      backgroundColor: '#ff9800',
                      color: 'white',
                      borderRadius: '10px',
                      fontSize: '12px'
                    }}>
                      Draft
                    </span>
                  )}
                </h3>
                <p style={{ margin: '0 0 10px 0', color: '#666' }}>
                  {content.body}
                </p>
                <div style={{
                  fontSize: '12px',
                  color: '#999',
                  display: 'flex',
                  gap: '20px'
                }}>
                  <span>Created by: {content.createdBy}</span>
                  <span>Created: {new Date(content.createdAt).toLocaleDateString()}</span>
                  {content.updatedAt && (
                    <span>Updated: {new Date(content.updatedAt).toLocaleDateString()}</span>
                  )}
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};

export default DashboardPage; 