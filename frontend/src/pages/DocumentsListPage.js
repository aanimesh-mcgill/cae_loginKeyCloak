import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { documentAPI } from '../services/api';
import { useAuth } from '../contexts/AuthContext';

const DocumentsListPage = () => {
  const [documents, setDocuments] = useState([]);
  const [loading, setLoading] = useState(true);
  const { hasRole } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    loadDocuments();
  }, []);

  const loadDocuments = async () => {
    try {
      const data = await documentAPI.getAllDocuments();
      setDocuments(data);
    } catch (error) {
      // handle error
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this document?')) {
      await documentAPI.deleteDocument(id);
      await loadDocuments();
    }
  };

  return (
    <div style={{ maxWidth: 900, margin: '0 auto', padding: 24 }}>
      <h1>Documents</h1>
      {(hasRole('Admin') || hasRole('Editor')) && (
        <button
          style={{ marginBottom: 20, background: '#4caf50', color: 'white', padding: '10px 20px', border: 'none', borderRadius: 5, cursor: 'pointer' }}
          onClick={() => navigate('/documents/new')}
        >
          + New Document
        </button>
      )}
      {loading ? (
        <div>Loading...</div>
      ) : (
        <table style={{ width: '100%', borderCollapse: 'collapse' }}>
          <thead>
            <tr style={{ background: '#f5f5f5' }}>
              <th style={{ padding: 10, border: '1px solid #ddd' }}>Title</th>
              <th style={{ padding: 10, border: '1px solid #ddd' }}>Created By</th>
              <th style={{ padding: 10, border: '1px solid #ddd' }}>Created At</th>
              <th style={{ padding: 10, border: '1px solid #ddd' }}>Actions</th>
            </tr>
          </thead>
          <tbody>
            {documents.map((doc) => (
              <tr key={doc.id}>
                <td style={{ padding: 10, border: '1px solid #ddd' }}>
                  <Link to={`/documents/${doc.id}`}>{doc.title}</Link>
                </td>
                <td style={{ padding: 10, border: '1px solid #ddd' }}>{doc.createdBy}</td>
                <td style={{ padding: 10, border: '1px solid #ddd' }}>{new Date(doc.createdAt).toLocaleString()}</td>
                <td style={{ padding: 10, border: '1px solid #ddd' }}>
                  <Link to={`/documents/${doc.id}`}>View</Link>
                  {(hasRole('Admin') || hasRole('Editor')) && (
                    <>
                      {' | '}
                      <Link to={`/documents/${doc.id}/edit`}>Edit</Link>
                    </>
                  )}
                  {hasRole('Admin') && (
                    <>
                      {' | '}
                      <button style={{ color: 'red', background: 'none', border: 'none', cursor: 'pointer' }} onClick={() => handleDelete(doc.id)}>Delete</button>
                    </>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
};

export default DocumentsListPage; 