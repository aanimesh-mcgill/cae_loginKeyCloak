import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { documentAPI } from '../services/api';
import { useAuth } from '../contexts/AuthContext';

const DocumentEditPage = ({ isEdit }) => {
  const { id } = useParams();
  const [title, setTitle] = useState('');
  const [content, setContent] = useState('');
  const [loading, setLoading] = useState(isEdit);
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const { hasRole } = useAuth();

  useEffect(() => {
    if (isEdit) {
      loadDocument();
    }
    // eslint-disable-next-line
  }, [id, isEdit]);

  const loadDocument = async () => {
    try {
      const data = await documentAPI.getDocumentById(id);
      setTitle(data.title);
      setContent(data.content);
    } catch (err) {
      setError('Document not found');
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    try {
      if (!title.trim() || !content.trim()) {
        setError('Title and content are required');
        return;
      }
      if (isEdit) {
        await documentAPI.updateDocument(id, { title, content });
        navigate(`/documents/${id}`);
      } else {
        const doc = await documentAPI.createDocument({ title, content });
        navigate(`/documents/${doc.id}`);
      }
    } catch (err) {
      setError('Failed to save document');
    }
  };

  if (loading) return <div style={{ padding: 24 }}>Loading...</div>;

  return (
    <div style={{ maxWidth: 700, margin: '0 auto', padding: 24 }}>
      <h1>{isEdit ? 'Edit Document' : 'New Document'}</h1>
      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: 16 }}>
          <label style={{ display: 'block', marginBottom: 6 }}>Title</label>
          <input
            type="text"
            value={title}
            onChange={e => setTitle(e.target.value)}
            style={{ width: '100%', padding: 10, fontSize: 16, borderRadius: 4, border: '1px solid #ccc' }}
            required
          />
        </div>
        <div style={{ marginBottom: 16 }}>
          <label style={{ display: 'block', marginBottom: 6 }}>Content</label>
          <textarea
            value={content}
            onChange={e => setContent(e.target.value)}
            style={{ width: '100%', minHeight: 120, padding: 10, fontSize: 16, borderRadius: 4, border: '1px solid #ccc' }}
            required
          />
        </div>
        {error && <div style={{ color: 'red', marginBottom: 12 }}>{error}</div>}
        <button type="submit" style={{ background: '#1976d2', color: 'white', padding: '10px 24px', border: 'none', borderRadius: 5, fontSize: 16, cursor: 'pointer' }}>
          {isEdit ? 'Update' : 'Create'}
        </button>
        <button type="button" style={{ marginLeft: 16, padding: '10px 24px', borderRadius: 5, border: '1px solid #ccc', background: '#f5f5f5', cursor: 'pointer' }} onClick={() => navigate('/documents')}>
          Cancel
        </button>
      </form>
    </div>
  );
};

export default DocumentEditPage; 