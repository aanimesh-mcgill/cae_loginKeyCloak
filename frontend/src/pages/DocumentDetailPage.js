import React, { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { documentAPI, setApiAuthToken } from '../services/api';
import { useAuth } from '../contexts/AuthContext';

const DocumentDetailPage = () => {
  const { id } = useParams();
  const [document, setDocument] = useState(null);
  const [loading, setLoading] = useState(true);
  const { hasRole, token, isAuthenticated } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    if (!token) {
      console.warn('[DocumentDetailPage] Token is missing, not loading document yet.');
      return;
    }
    setApiAuthToken(token);
    if (isAuthenticated) {
      loadDocument();
    }
    // eslint-disable-next-line
  }, [id, isAuthenticated, token]);

  const loadDocument = async () => {
    try {
      const data = await documentAPI.getDocumentById(id);
      setDocument(data);
    } catch (error) {
      setDocument(null);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (!token) {
      console.warn('[DocumentDetailPage] Token is missing, cannot delete document!');
      return;
    }
    setApiAuthToken(token);
    if (window.confirm('Are you sure you want to delete this document?')) {
      await documentAPI.deleteDocument(document.id);
      navigate('/documents');
    }
  };

  if (loading) return <div style={{ padding: 24 }}>Loading...</div>;
  if (!document) return <div style={{ padding: 24 }}>Document not found.</div>;

  return (
    <div style={{ maxWidth: 700, margin: '0 auto', padding: 24 }}>
      <h1>{document.title}</h1>
      <div style={{ color: '#888', marginBottom: 10 }}>
        Created by {document.createdBy} on {new Date(document.createdAt).toLocaleString()}
        {document.updatedAt && (
          <span> | Updated by {document.updatedBy} on {new Date(document.updatedAt).toLocaleString()}</span>
        )}
      </div>
      <div style={{ marginBottom: 30, whiteSpace: 'pre-wrap', background: '#f9f9f9', padding: 16, borderRadius: 6 }}>{document.content}</div>
      <Link to="/documents">Back to Documents</Link>
      {(hasRole('Admin') || hasRole('Editor')) && (
        <>
          {' | '}
          <Link to={`/documents/${document.id}/edit`}>Edit</Link>
        </>
      )}
      {hasRole('Admin') && (
        <>
          {' | '}
          <button style={{ color: 'red', background: 'none', border: 'none', cursor: 'pointer' }} onClick={handleDelete}>Delete</button>
        </>
      )}
    </div>
  );
};

export default DocumentDetailPage; 